using Fontify.Services;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Fontify
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType(StandardContentTypeNames.Code)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal class TextViewCreationListener : IWpfTextViewCreationListener
    {
        [Import] private IClassificationFormatMapService cfms { get; set; }

        private bool hasExecuted = false;

        private async Task OverrideAsync()
        {
            var service = await ClassifierExtension.GetInstanceAsync();
            await service.OverrideFormatMapAsync(cfms);
        }

        public void TextViewCreated(IWpfTextView textView)
        {
            if (!hasExecuted)
            {
                _ = ThreadHelper.JoinableTaskFactory.RunAsync(OverrideAsync);
                hasExecuted = true;
            }
        }
    }
}
