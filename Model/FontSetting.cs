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
        public string BaseFontFamily { get; set; } = "JetBrains Mono";

        [Category("Font Defaults")]
        [DisplayName("Normal")]
        [Editor(typeof(FontListEditor), typeof(UITypeEditor))]
        public string NormalTypeface { get; set; } = "JetBrains Mono Italic";

        [Category("Font Defaults")]
        [DisplayName("Bold")]
        [Editor(typeof(FontListEditor), typeof(UITypeEditor))]
        public string BoldTypeface { get; set; } = "JetBrains Mono Italic";

        [Category("Font Defaults")]
        [DisplayName("Italic")]
        [Editor(typeof(FontListEditor), typeof(UITypeEditor))]
        public string ItalicTypeface { get; set; } = "JetBrains Mono Italic";

        [Category("Font Defaults")]
        [DisplayName("BoldItalic")]
        [Editor(typeof(FontListEditor), typeof(UITypeEditor))]
        public string BoldItalicTypeface { get; set; } = "JetBrains Mono Italic";

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
