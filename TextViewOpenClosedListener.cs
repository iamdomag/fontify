using fontify.Contracts;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Fontify
{
    [VisualStudioContribution]
    internal class TextViewOpenClosedListener : ExtensionPart, ITextViewOpenClosedListener
    {
        private bool isRunning = false;
        public TextViewExtensionConfiguration TextViewExtensionConfiguration => new() {
            AppliesTo= new[] { DocumentFilter.FromDocumentType(StandardContentTypeNames.Code)}
        };

        private IFontCustomizationService _fontService { get; }

        public TextViewOpenClosedListener(IFontCustomizationService fontService)
        {            
            _fontService = fontService;
        }

        public async Task TextViewClosedAsync(ITextViewSnapshot textView, CancellationToken cancellationToken)
        {
            await _fontService.ClosedAsync();
        }

        public async Task TextViewOpenedAsync(ITextViewSnapshot textView, CancellationToken cancellationToken)
        {
            if (!isRunning)
            {
                try
                {
                    isRunning = true;
                    await _fontService.ApplyAsync(textView);
                    isRunning = false;
                }
                catch (Exception)
                {
                    isRunning = false;
                }
            }
        }
    }
}
