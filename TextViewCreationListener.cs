using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace Fontify
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType(StandardContentTypeNames.Code)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal class TextViewCreationListener : IWpfTextViewCreationListener
    {
        [Import] private IClassificationTypeRegistryService ctrs { get; set; }
        [Import] private IClassificationFormatMapService cfms { get; set; }

        private readonly ClassifierExtension service = ClassifierExtension.GetInstance();

        public void TextViewCreated(IWpfTextView textView)
        {
            service.OverrideFormatMap(ctrs, cfms);
        }
    }
}
