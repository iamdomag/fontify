using Fontify.Services;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

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
                content.Initialize(this, service);
                return content;
            }
        }
    }
}
