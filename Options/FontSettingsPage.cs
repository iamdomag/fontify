using Fontify.Services;
using Microsoft.VisualStudio.Shell;
using System.Windows.Forms;

namespace Fontify.Options
{
    internal class FontSettingsPage : DialogPage
    {
        private readonly SettingsStorage<FontSettings> _storage;
        private FontSettings _settings;

        public FontSettingsPage()
        {
            _storage = new SettingsStorage<FontSettings>();
            _settings = new FontSettings();
        }

        public override object AutomationObject => _settings;

        public override void LoadSettingsFromStorage()
        {
            base.LoadSettingsFromStorage();
            _ = ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                var storageValue = await _storage.GetSettingsAsync();
                _settings.Copy(storageValue);
            });
        }

        public override void SaveSettingsToStorage()
        {
            base.SaveSettingsToStorage();
            _ = ThreadHelper.JoinableTaskFactory.RunAsync(async () => 
            {
                await _storage.SaveSettingsAsync(_settings);
            });
        }

        protected override IWin32Window Window
        {
            get
            {
                var content = new FontSettingsPageContent();
                content.Initialize(_settings);
                return content;
            }
        }
    }
}
