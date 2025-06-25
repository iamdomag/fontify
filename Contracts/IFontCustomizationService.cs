using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Editor;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fontify.Contracts
{
    internal interface IFontCustomizationService
    {
        Task InitializeAsync();
        Task ApplyAsync(ITextViewSnapshot textView);
        Task ClosedAsync();
        Task OverrideFormatMapAsync(bool unlock = false);
    }
}
