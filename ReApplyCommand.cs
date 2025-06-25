using System.Diagnostics;
using fontify.Contracts;
using Microsoft;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Shell;

namespace fontify
{
    /// <summary>
    /// Command1 handler.
    /// </summary>
    [VisualStudioContribution]
    internal class ReApplyCommand : Command
    {
        private readonly TraceSource logger;
        private readonly IFontCustomizationService _fontService;
        /// <summary>
        /// Initializes a new instance of the <see cref="ReApplyCommand"/> class.
        /// </summary>
        /// <param name="traceSource">Trace source instance to utilize.</param>
        public ReApplyCommand(TraceSource traceSource, IFontCustomizationService fontService)
        {
            // This optional TraceSource can be used for logging in the command. You can use dependency injection to access
            // other services here as well.
            //this.logger = Requires.NotNull(traceSource, nameof(traceSource));
            _fontService = fontService;
        }

        /// <inheritdoc />
        public override CommandConfiguration CommandConfiguration => new("%fontify.ReApplyCommand.DisplayName%")
        {
            // Use this object initializer to set optional parameters for the command. The required parameter,
            // displayName, is set above. DisplayName is localized and references an entry in .vsextension\string-resources.json.
            Icon = new(ImageMoniker.KnownValues.Extension, IconSettings.IconAndText),
            Placements = [CommandPlacement.KnownPlacements.ExtensionsMenu],
        };

        /// <inheritdoc />
        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            // Use InitializeAsync for any one-time setup or initialization.
            await _fontService.InitializeAsync();
            await base.InitializeAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
        {
            await _fontService.OverrideFormatMapAsync(true);
        }
    }
}
