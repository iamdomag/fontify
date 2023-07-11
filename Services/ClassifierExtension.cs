using Microsoft.VisualStudio;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Fontify.Services
{
    internal class ClassifierExtension
    {
        private static readonly AsyncLazy<ClassifierExtension> instance;
        static ClassifierExtension() => instance = new AsyncLazy<ClassifierExtension>(CreateInstanceAsync, ThreadHelper.JoinableTaskFactory);
        private static async Task<ClassifierExtension> CreateInstanceAsync()
        {
            var settingsProvider = await FontSettingsService.GetInstanceAsync(true);
            return await Task.FromResult(new ClassifierExtension
            {
                normalTypeface = settingsProvider.GetTypeface(FontOverrides.Normal),
                boldTypeface = settingsProvider.GetTypeface(FontOverrides.Bold),
                italicTypeface = settingsProvider.GetTypeface(FontOverrides.Italic),
                bolditalicTypeface = settingsProvider.GetTypeface(FontOverrides.BoldItalic),
                italicClassifiers = settingsProvider.GetItalicClassifiers()
            });
        }

        public static async Task<ClassifierExtension> GetInstanceAsync() => await instance.GetValueAsync();
        public static ClassifierExtension Instance => instance.GetValue();

        private bool IsExecuting = false;
        private Typeface normalTypeface;
        private Typeface boldTypeface;
        private Typeface italicTypeface;
        private Typeface bolditalicTypeface;
        private string[] italicClassifiers;
        private ClassifierExtension() { }

        public async Task OverrideFormatMapAsync(IClassificationFormatMapService cfms)
        {
            if (!IsExecuting)
            {
                try
                {
                    IsExecuting = true;

                    var cfm = cfms.GetClassificationFormatMap(category: "text");
                    var defaultProps = cfm.DefaultTextProperties
                        .SetTypeface(normalTypeface);
                    cfm.DefaultTextProperties = defaultProps;
                    
                    var propertyUpdates = await UpdatePropertiesAsync(cfm);

                    cfm.BeginBatchUpdate();
                    propertyUpdates.ForEach(item => cfm.SetTextProperties(item.ct, item.props));
                    cfm.EndBatchUpdate();
                }
                catch (Exception ex)
                {
                    ErrorHandler.ExceptionToHResult(new Exception(
                        message: $"[Fontify] Exception occured on {ex.Source} with message {ex.Message}",
                        innerException: ex
                    ));
                } finally
                {
                    IsExecuting = false;
                }
            }
        }

        private async Task<List<(IClassificationType ct, TextFormattingRunProperties props)>> UpdatePropertiesAsync(IClassificationFormatMap cfm)
        {
            var updatedItems = new List<(IClassificationType ct, TextFormattingRunProperties props)>();
            var classifiers = cfm.CurrentPriorityOrder
                .Where(x => x != default)
                .Select(x => new {
                    Name = x.Classification,
                    Properties = cfm.GetTextProperties(x),
                    ClassificationType = x })
                .Where(x => x.Properties.Bold || x.Properties.Italic || italicClassifiers.Contains(x.Name));

            foreach (var item in classifiers)
            {
                try
                {
                    var isItalic = italicClassifiers.Contains(item.Name) || item.Properties.Italic;
                    var isBold = item.Properties.Bold;
                    var isBoldItalic = isBold && isItalic;
                    var typeface =
                            isBoldItalic ? bolditalicTypeface :
                            isBold ? boldTypeface :
                            isItalic ? italicTypeface :
                            default;

                    if (typeface != default)
                    {
                        var props = item.Properties
                            .SetTypeface(typeface);

                        if (isItalic)
                        {
                            props = props.SetItalic(true);
                        }

                        updatedItems.Add((item.ClassificationType, props));                        
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

            return await Task.FromResult(updatedItems);
        }
    }
}