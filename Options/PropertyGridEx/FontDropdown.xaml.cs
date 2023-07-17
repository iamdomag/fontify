using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Fontify.Options.PropertyGridEx
{
    /// <summary>
    /// Interaction logic for FontDropdown.xaml
    /// </summary>
    public partial class FontDropdown : UserControl
    {
        public List<string> fontFamilyNames;
        public event EventHandler SelectionChanged;
        public object SelectedItem => FontNamesList.SelectedItem;

        public FontDropdown()
        {
            fontFamilyNames = Fonts.SystemFontFamilies
                .Select(x => x.Source)
                .OrderBy(x => x)
                .ToList();
            InitializeComponent();
        }

        public void LoadFonts(string currentFont, string fontFamily = null)
        {
            List<string> sourceData;

            if (string.IsNullOrEmpty(fontFamily))
            {
                sourceData = fontFamilyNames;
            }
            else
            {
                var baseFont = new FontFamily(fontFamily);
                sourceData = baseFont.GetTypefaces()
                    .Select(x => $"{fontFamily} {x.FaceNames.FirstOrDefault().Value}")
                    .ToList();
            }
            
            FontNamesList.ItemsSource = sourceData;
            FontNamesList.ScrollIntoView(sourceData.Last());
            FontNamesList.SelectedIndex = sourceData.IndexOf(currentFont);
            FontNamesList.ScrollIntoView(FontNamesList.SelectedItem);
            FontNamesList.Focus();
        }

        private void listBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (SelectionChanged != null) SelectionChanged(FontNamesList, e);
        }
    }
}
