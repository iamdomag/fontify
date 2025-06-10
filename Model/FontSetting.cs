using Fontify.Options.PropertyGridEx;
using System.ComponentModel;
using System.Drawing.Design;

namespace fontify.Model
{
    [DefaultProperty("BaseFontFamily")]
    internal class FontSetting
    {
        [Category("Font Defaults")]
        [DisplayName("Font Family")]
        [Editor(typeof(FontListEditor), typeof(UITypeEditor))]
        public string BaseFontFamily { get; set; }

        [Category("Font Defaults")]
        [DisplayName("Normal")]
        [Editor(typeof(FontListEditor), typeof(UITypeEditor))]
        public string NormalTypeface { get; set; }

        [Category("Font Defaults")]
        [DisplayName("Bold")]
        [Editor(typeof(FontListEditor), typeof(UITypeEditor))]
        public string BoldTypeface { get; set; }

        [Category("Font Defaults")]
        [DisplayName("Italic")]
        [Editor(typeof(FontListEditor), typeof(UITypeEditor))]
        public string ItalicTypeface { get; set; }

        [Category("Font Defaults")]
        [DisplayName("BoldItalic")]
        [Editor(typeof(FontListEditor), typeof(UITypeEditor))]
        public string BoldItalicTypeface { get; set; }

        [Category("Classification Fonts")]
        [DisplayName("Line Number")]
        [Editor(typeof(FontListEditor), typeof(UITypeEditor))]
        public string LineNumber { get; set; } = "Roboto";

        public void Copy(FontSetting source)
        {
            BaseFontFamily = source.BaseFontFamily;
            NormalTypeface = source.NormalTypeface;
            BoldTypeface = source.BoldTypeface;
            ItalicTypeface = source.ItalicTypeface;
            BoldItalicTypeface = source.BoldItalicTypeface;
        }
    }
}
