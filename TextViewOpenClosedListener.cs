﻿using fontify.Contracts;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Editor;
using Microsoft.VisualStudio.Extensibility.VSSdkCompatibility;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Fontify
{
    [VisualStudioContribution]
    internal class TextViewOpenClosedListener : ExtensionPart, ITextViewOpenClosedListener
    {
        private MefInjection<IClassificationFormatMapService> _injector { get; }
        private bool isRunning = false;
        public TextViewExtensionConfiguration TextViewExtensionConfiguration => new() {
            AppliesTo= new[] { DocumentFilter.FromDocumentType(StandardContentTypeNames.Code)}
        };

        private IFontCustomizationService _fontService { get; }

        public TextViewOpenClosedListener(IFontCustomizationService fontService, MefInjection<IClassificationFormatMapService> injector)
        {            
            _fontService = fontService;
            _injector = injector;
        }

        public async Task TextViewClosedAsync(ITextViewSnapshot textView, CancellationToken cancellationToken)
        {
            await _fontService.ClosedAsync(this.Extensibility.Editor());
        }

        public async Task TextViewOpenedAsync(ITextViewSnapshot textView, CancellationToken cancellationToken)
        {
            if (!isRunning)
            {
                try
                {
                    isRunning = true;
                    var cfms = await _injector.GetServiceAsync();
                    await _fontService.ApplyAsync(cfms?.GetClassificationFormatMap("text"));
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
