using fontify.Contracts;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Extensibility.Editor;
using Microsoft.VisualStudio.Extensibility.VSSdkCompatibility;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace fontify.Services
{
    internal class FontCustomizationService : IFontCustomizationService
    {
        private Dictionary<FontOverrideType?, Typeface?> fontOverrides;
        private bool isLocked;
        private readonly MefInjection<IClassificationFormatMapService> _cfmsInjector;
        private readonly IFontSettingProvider _settingsProvider;

        public FontCustomizationService(IFontSettingProvider settingsProvider, 
            MefInjection<IClassificationFormatMapService> cfmsInjector)
        {
            isLocked = false;
            _cfmsInjector = cfmsInjector;
            _settingsProvider = settingsProvider;
        }

        public async Task OverrideFormatMapAsync(bool unlock = false)
        {
            try
            {
                if (!isLocked || unlock)
                {
                    isLocked = true;
                    if (fontOverrides == null)
                    {
                        await InitializeAsync();
                    }
                    var cfms = await _cfmsInjector.GetServiceAsync();
                    var cfm = cfms.GetClassificationFormatMap(category: "text");
                    var propertyUpdates = await GetUpdatedPropertiesAsync(cfm);

                    if (cfm != null && (propertyUpdates?.Any() ?? false))
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        cfm?.BeginBatchUpdate();

                        var defaultProps = cfm.DefaultTextProperties
                            .SetTypeface(fontOverrides[FontOverrideType.Normal]);

                        cfm.DefaultTextProperties = defaultProps;

                        foreach (var item in propertyUpdates)
                        {
                            cfm?.SetTextProperties(item.Key, item.Value);
                        }
                        cfm?.EndBatchUpdate();
                    }
                    else
                    {
                        isLocked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                isLocked = false;
                var hr = ErrorHandler.ExceptionToHResult(new Exception(
                    message: $"[Fontify] Exception occured on {ex.Source} with message {ex.Message}",
                    innerException: ex
                ));
                ErrorHandler.ThrowOnFailure(hr);
            }
        }

        private async Task<Dictionary<IClassificationType?, TextFormattingRunProperties?>> GetUpdatedPropertiesAsync(IClassificationFormatMap? cfm)
        {
            var updatedItems = new Dictionary<IClassificationType?, TextFormattingRunProperties?>();
            string[] lineNumberClassifiers = [ "line number", "Selected Line Number" ];

            if (cfm != null)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                var classifiers = cfm.CurrentPriorityOrder
                    .Where(x => x != default)
                    .Select(x => new
                    {
                        Name = x.Classification,
                        Properties = cfm?.GetTextProperties(x),
                        ClassificationType = x
                    })
                    .Where(x => (x.Properties?.Bold ?? false) || (x.Properties?.Italic ?? false) || lineNumberClassifiers.Contains(x.Name));
                
                foreach (var item in classifiers)
                {
                    try
                    {
                        var isItalic = item?.Properties?.Italic ?? false;
                        var isBold = item?.Properties?.Bold ?? false;
                        var isBoldItalic = isBold && isItalic;
                        var typeface =
                                isBoldItalic ? fontOverrides[FontOverrideType.BoldItalic] :
                                isBold ? fontOverrides[FontOverrideType.Bold] :
                                isItalic ? fontOverrides[FontOverrideType.Italic] :
                                default;
                        var props = item?.Properties;

                        if (lineNumberClassifiers.Contains(item.Name))
                        {
                            props = props?
                                .SetTypeface(fontOverrides[FontOverrideType.LineNumber])?
                                .SetFontRenderingEmSize(props.FontRenderingEmSize * 0.75)?
                                .SetFontHintingEmSize(props.FontHintingEmSize * 0.75);
                        }

                        if (typeface != default)
                        {
                            props = props?.SetTypeface(typeface);
                        }

                        if (props != item?.Properties)
                        {
                            updatedItems.Add(item?.ClassificationType, props);
                        }
                    }
                    catch (Exception ex)
                    {
                        var hr = ErrorHandler.ExceptionToHResult(new Exception(
                            message: $"Unable to apply font to ClassifierType: {item.Name} due to an error.",
                            innerException: ex
                        ));
                        ErrorHandler.ThrowOnFailure(hr);
                    }
                }
            }

            return updatedItems;
        }

        public async Task InitializeAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var cfms = await _cfmsInjector.GetServiceAsync();

            fontOverrides = await _settingsProvider.GetFontOverridesAsync();
            await this.OverrideFormatMapAsync();
        }

        public async Task ApplyAsync(ITextViewSnapshot textView)
        {
            await this.InitializeAsync();       
        }

        public Task ClosedAsync()
        {
            //Cleanup tasks here
            return Task.FromResult(0);
        }
    }
}