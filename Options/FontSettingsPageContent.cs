using Fontify.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Fontify.Options
{
    public partial class FontSettingsPageContent : UserControl
    {
        private FontSettings settings;
        public FontSettingsPageContent()
        {
            InitializeComponent();
        }

        internal void Initialize(FontSettingsService settingsService)
        {
            settings = settingsService.Settings;
            SettingsGrid.SelectedObject = settings;
        }

        private void SettingsGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.PropertyDescriptor.Name == "BaseFontFamily" && !e.OldValue.Equals(e.ChangedItem.Value))
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
