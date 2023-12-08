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
        private FontSettings _settings;
        public FontSettingsPageContent()
        {
            InitializeComponent();
        }

        internal void Initialize(FontSettings settings)
        {
            _settings = settings;
            SettingsGrid.SelectedObject = settings;
        }

        private void SettingsGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.PropertyDescriptor.Name == "BaseFontFamily" && !e.OldValue.Equals(e.ChangedItem.Value))
            {
                var baseFontName = e.ChangedItem.Value.ToString();
                if (!string.IsNullOrEmpty(baseFontName))
                {
                    _settings.NormalTypeface = baseFontName;
                    _settings.BoldTypeface = baseFontName;
                    _settings.ItalicTypeface = baseFontName;
                    _settings.BoldItalicTypeface = baseFontName;
                }
                else
                {
                    _settings.BoldTypeface = string.Empty;
                    _settings.ItalicTypeface = string.Empty;
                    _settings.NormalTypeface = string.Empty;
                    _settings.BoldItalicTypeface = string.Empty;
                }
            }
        }
    }
}
