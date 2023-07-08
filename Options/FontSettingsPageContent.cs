using Fontify.Services;
using System.Windows.Media;
using System.Windows.Forms;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Fontify.Tools;

namespace Fontify.Options
{
    public partial class FontSettingsPageContent : UserControl
    {
        private FontSettings settings;
        private FontSettingsPage page;
        private FontSettingsService service;
        internal List<string> TypefaceNames;
        internal List<string> FontFamilyNames;

        public FontSettingsPageContent()
        {
            InitializeComponent();
        }

        internal void Initialize(FontSettingsPage dialogPage, FontSettingsService settingsService) {
            (page, service, settings) = (dialogPage, settingsService, settingsService.Settings);
            SettingsGrid.SelectedObject = settings;
        }


        private void SettingsGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.PropertyDescriptor.Name == "BaseFontFamily")
            {
                var baseFontName = e.ChangedItem.Value.ToString();
                if (!string.IsNullOrEmpty(baseFontName))
                {
                    settings.NormalTypeface = baseFontName;
                    settings.BoldTypeface = baseFontName;
                    settings.ItalicTypeface = baseFontName;
                    settings.BoldItalicTypeface = baseFontName;
                }
                else
                {
                    settings.BoldTypeface = string.Empty;
                    settings.ItalicTypeface = string.Empty;
                    settings.NormalTypeface = string.Empty;
                    settings.BoldItalicTypeface = string.Empty;
                }
            }
        }
    }
}
