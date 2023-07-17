using Fontify.Services;
using Microsoft.VisualStudio.Shell;
using System.Windows.Forms;

namespace Fontify.Options
{
    internal class FontSettingsPage : DialogPage
    {
        private FontSettingsService service = FontSettingsService.GetInstance();
        public override object AutomationObject => service.Settings;

        public override void LoadSettingsFromStorage()
        {
            base.LoadSettingsFromStorage();
            _ = ThreadHelper.JoinableTaskFactory.RunAsync(service.LoadStorageAsync);
        }

        public override void SaveSettingsToStorage()
        {
            base.SaveSettingsToStorage();
            _ = ThreadHelper.JoinableTaskFactory.RunAsync(service.SaveStorageAsync);
        }

        protected override IWin32Window Window
        {
            get
            {
                var content = new FontSettingsPageContent();
                content.Initialize(service);
                return content;
            }
        }
    }
}
