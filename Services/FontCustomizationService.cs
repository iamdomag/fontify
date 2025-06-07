using fontify.Contracts;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Extensibility.Editor;
using Microsoft.VisualStudio.Extensibility.VSSdkCompatibility;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Media;

namespace fontify.Services
{
    internal class FontCustomizationService : IFontCustomizationService
    {
        private Typeface normalTypeface;
        private Typeface boldTypeface;
        private Typeface italicTypeface;
        private Typeface bolditalicTypeface;
        private bool isLocked;
        private readonly MefInjection<IClassificationFormatMapService> _injector;
        private readonly IFontSettingProvider _settingsProvider;

        public FontCustomizationService(IFontSettingProvider settingProvider, MefInjection<IClassificationFormatMapService> injector)
        {
            isLocked = false;
            _injector = injector;
            _settingsProvider = settingProvider;
        }

        private async Task OverrideFormatMapAsync()
        {
            try
            {
                //var defaultProps = cfm?.DefaultTextProperties
                //    .SetTypeface(normalTypeface);
                //cfm.DefaultTextProperties = defaultProps;
                if (!isLocked)
                {
                    isLocked = true;
                    if (normalTypeface == null)
                    {
                        await InitializeAsync();
                    }
                    var cfms = await _injector.GetServiceAsync();
                    var cfm = cfms.GetClassificationFormatMap(category: "text");
                    var propertyUpdates = await GetUpdatedPropertiesAsync(cfm);

                    if (cfm != null && (propertyUpdates?.Any() ?? false))
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        cfm?.BeginBatchUpdate();

                        var defaultProps = cfm.DefaultTextProperties
                            .SetTypeface(normalTypeface);

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
                ErrorHandler.ExceptionToHResult(new Exception(
                    message: $"[Fontify] Exception occured on {ex.Source} with message {ex.Message}",
                    innerException: ex
                ));
            }
        }

        private async Task<Dictionary<IClassificationType?, TextFormattingRunProperties?>> GetUpdatedPropertiesAsync(IClassificationFormatMap? cfm)
        {
            var updatedItems = new Dictionary<IClassificationType?, TextFormattingRunProperties?>();

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
                    .Where(x => (x.Properties?.Bold ?? false) || (x.Properties?.Italic ?? false));

                foreach (var item in classifiers)
                {
                    try
                    {
                        var isItalic = item?.Properties?.Italic ?? false;
                        var isBold = item?.Properties?.Bold ?? false;
                        var isBoldItalic = isBold && isItalic;
                        var typeface =
                                isBoldItalic ? bolditalicTypeface :
                                isBold ? boldTypeface :
                                isItalic ? italicTypeface :
                                default;

                        if (typeface != default)
                        {
                            var props = item?.Properties?.SetTypeface(typeface);

                            props = isItalic ? props?.SetItalic(true) : props;
                            updatedItems.Add(item?.ClassificationType, props);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(
                            message: $"Unable to apply font to ClassifierType: {item.Name} due to an error.",
                            innerException: ex
                        );
                    }
                }
            }

            return updatedItems;
        }

        private async Task InitializeAsync()
        {
            normalTypeface = await _settingsProvider.GetTypefaceAsync(FontOverrides.Italic);
            boldTypeface = await _settingsProvider.GetTypefaceAsync(FontOverrides.Italic);
            italicTypeface = await _settingsProvider.GetTypefaceAsync(FontOverrides.Italic);
            bolditalicTypeface = await _settingsProvider.GetTypefaceAsync(FontOverrides.Italic);
        }

        public async Task ApplyAsync(IClassificationFormatMap? cfm)
        {
            await this.OverrideFormatMapAsync();            
        }

        public Task ClosedAsync(EditorExtensibility editor)
        {
            //Cleanup tasks here
            return Task.FromResult(0);
        }
    }
}