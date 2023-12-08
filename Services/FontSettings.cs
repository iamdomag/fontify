using Fontify.Options.PropertyGridEx;
using System.ComponentModel;
using System.Drawing.Design;

namespace Fontify.Services
{
    [DefaultProperty("BaseFontFamily")]
    internal class FontSettings
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

        [Category("Settings")]
        [DisplayName("Italic Classifiers")]
        [DefaultValue("comment,string,interface name,keyword - control,method name,namespace name")]
        public string ItalicClassifiers { get; set; } = "comment,string,interface name,keyword - control,method name,namespace name";

        public void Copy(FontSettings source)
        {
            BaseFontFamily = source.BaseFontFamily;
            NormalTypeface = source.NormalTypeface;
            BoldTypeface = source.BoldTypeface;
            ItalicTypeface = source.ItalicTypeface;
            BoldItalicTypeface = source.BoldItalicTypeface;
            ItalicClassifiers = source.ItalicClassifiers;
        }
    }
}
