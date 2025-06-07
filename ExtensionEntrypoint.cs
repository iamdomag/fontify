using fontify.Contracts;
using fontify.Model;
using fontify.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;

namespace fontify
{
    /// <summary>
    /// Extension entrypoint for the VisualStudio.Extensibility extension.
    /// </summary>
    [VisualStudioContribution]
    internal class ExtensionEntrypoint : Extension
    {
        /// <inheritdoc />
        public override ExtensionConfiguration ExtensionConfiguration => new()
        {
            RequiresInProcessHosting = true,
            //Metadata = new(
                //id: "%package.name%.%package.id%",
                //version: this.ExtensionAssemblyVersion,
                //publisherName: "%package.publisher%",
                //displayName: "%package.displayName%",
                //description: "%package.description%")
        };

        /// <inheritdoc />
        protected override void InitializeServices(IServiceCollection serviceCollection)
        {
            
            base.InitializeServices(serviceCollection);
            
            serviceCollection.AddScoped<IFontCustomizationService, FontCustomizationService>();
            serviceCollection.AddScoped<IFontSettingProvider, FontSettingProvider>();
            serviceCollection.AddScoped<ISettingStorage<FontSetting>, SettingStorage<FontSetting>>();
            // You can configure dependency injection here by adding services to the serviceCollection.
        }
    }
}
