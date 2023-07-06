using Fontify.Services;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Fontify.Listeners
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType(StandardContentTypeNames.Code)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal class TextViewCreationListener : IWpfTextViewCreationListener
    {
        [Import] private IClassificationTypeRegistryService ctrs { get; set; }
        [Import] private IClassificationFormatMapService cfms { get; set; }

        private bool isDone = false;

        public void TextViewCreated(IWpfTextView textView)
        {
            if (!isDone)
            {
                _ = ThreadHelper.JoinableTaskFactory.RunAsync(OverrideAsync);
            }
        }

        private async Task OverrideAsync()
        {
            var service = await ClassifierExtension.GetInstanceAsync();
            await service.OverrideFormatMapAsync(ctrs, cfms);
            isDone = true;
        }
    }
}
