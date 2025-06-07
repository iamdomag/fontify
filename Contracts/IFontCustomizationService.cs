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
        Task ApplyAsync(IClassificationFormatMap? cfm);
        Task ClosedAsync(EditorExtensibility editor);
    }
}
