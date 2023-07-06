using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Fontify
{
    internal class ClassifierExtension
    {
        private static readonly Lazy<ClassifierExtension> instance = new Lazy<ClassifierExtension>(CreateInstance, true);
        static ClassifierExtension() { }
        private static ClassifierExtension CreateInstance() => new ClassifierExtension();
        public static ClassifierExtension GetInstance() => instance.Value;

        private bool IsDone = false;
        private Typeface regularTypeface;
        private Typeface boldTypeface;
        private Typeface italicTypeface;
        private Typeface bolditalicTypeface;
        private string[] italicClassifiers;
        private ClassifierExtension()
        {
            regularTypeface = new Typeface("JetBrains Mono Regular");
            boldTypeface = new Typeface("JetBrains Mono ExtraBold");
            italicTypeface = new Typeface("JetBrains Mono ExtraLight Italic");
            bolditalicTypeface = new Typeface("JetBrains Mono Italic");
            italicClassifiers = new string[] {
                PredefinedClassificationTypeNames.Comment,
                PredefinedClassificationTypeNames.String,
                "interface name",
                "keyword - control",
                "method name",
                "namespace name"
            };
        }

        public void OverrideFormatMap(IClassificationTypeRegistryService ctrs, IClassificationFormatMapService cfms)
        {
            if (IsDone) return;
            try
            {
                var cfm = cfms.GetClassificationFormatMap(category: "text");

                var defaultProps = cfm.DefaultTextProperties
                    .SetTypeface(regularTypeface);
                cfm.DefaultTextProperties = defaultProps;

                var propertyUpdates = ThreadHelper.JoinableTaskFactory.Run(async() => await UpdatePropertiesAsync(cfm));

                cfm.BeginBatchUpdate();
                propertyUpdates.ForEach(item => cfm.SetExplicitTextProperties(item.ct, item.props));
                cfm.EndBatchUpdate();

                IsDone = true;
            }
            catch (Exception ex)
            {
                ErrorHandler.ExceptionToHResult(new Exception(
                    message: $"[Fontify] Exception occured on {ex.Source} with message {ex.Message}",
                    innerException: ex
                ));
            }
        }

        private async Task<List<(IClassificationType ct, TextFormattingRunProperties props)>> UpdatePropertiesAsync(IClassificationFormatMap cfm)
        {
            var updatedItems = new List<(IClassificationType ct, TextFormattingRunProperties props)>();
            var classifiers = cfm.CurrentPriorityOrder
                .Where(x => x != default)
                .Select(x => new { 
                    Name = x.Classification, 
                    Properties = cfm.GetExplicitTextProperties(x), 
                    ClassificationType = x })
                .Where(x => x.Properties.Bold || x.Properties.Italic || italicClassifiers.Contains(x.Name));

            foreach (var item in classifiers)
            {
                try
                {
                    var isItalic = italicClassifiers.Contains(item.Name);
                    var isBold = item.Properties.Bold;
                    var isBoldItalic = isBold && isItalic;
                    var typeface =
                            isBoldItalic ? bolditalicTypeface :
                            isBold ? boldTypeface :
                            isItalic ? italicTypeface :
                            default;

                    if (typeface != default)
                    {
                        updatedItems.Add((item.ClassificationType, item.Properties.SetTypeface(typeface)));
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.ExceptionToHResult(new Exception(
                        message: $"Unable to apply font to ClassifierType: {item.Name} due to an error.",
                        innerException: ex
                    ));
                }
            }

            return await Task.FromResult(updatedItems);
        }
    }
}
