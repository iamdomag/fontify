using fontify.Contracts;
using fontify.Model;
using Microsoft.VisualStudio.Shell;
using System.Windows.Forms;

namespace fontify.Options
{
    internal class FontSettingsPage : DialogPage
    {
        private IFontSettingProvider _settingProvider;

        public FontSettingsPage(IFontSettingProvider settingProvider)
        {
            _settingProvider = settingProvider;
        }

        public override object AutomationObject => _settingProvider.Settings ?? new();

        public override void LoadSettingsFromStorage()
        {
            base.LoadSettingsFromStorage();
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await _settingProvider.GetSettingsAsync();
            });
        }

        public override void SaveSettingsToStorage()
        {
            base.SaveSettingsToStorage();
            ThreadHelper.JoinableTaskFactory.Run(async () => 
            {
                await _settingProvider.SaveSettingsAsync();
            });
        }

        protected override IWin32Window Window
        {
            get
            {
                return new FontSettingsPageContent(_settingProvider.Settings);
            }
        }
    }
}
