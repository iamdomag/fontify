using Fontify.Services;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Fontify
{
    [Export(typeof(ITextViewCreationListener))]
    [ContentType(StandardContentTypeNames.Code)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal class TextViewCreationListener : ITextViewCreationListener
    {
        [Import] private IClassificationTypeRegistryService ctrs { get; set; }
        [Import] private IClassificationFormatMapService cfms { get; set; }

        private bool hasExecuted = false;

        private async Task OverrideAsync()
        {
            var service = await ClassifierExtension.GetInstanceAsync();
            await service.OverrideFormatMapAsync(ctrs, cfms);
        }

        public void TextViewCreated(ITextView textView)
        {
            if (!hasExecuted)
            {
                _ = ThreadHelper.JoinableTaskFactory.RunAsync(OverrideAsync);
                hasExecuted = true;
            }
        }
    }
}
