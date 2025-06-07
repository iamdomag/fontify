using fontify.Contracts;
using fontify.Model;
using System.Windows.Forms;

namespace fontify.Options
{
    internal partial class FontSettingsPageContent : UserControl
    {
        private FontSetting? _settings;
        public FontSettingsPageContent(FontSetting? fontSetting)
        {
            _settings = fontSetting;
            InitializeComponent();
        }

        private void SettingsGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.PropertyDescriptor.Name == "BaseFontFamily" && !e.OldValue.Equals(e.ChangedItem.Value))
            {
                var baseFontName = e.ChangedItem.Value.ToString();
                if (_settings != null)
                {
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
}
