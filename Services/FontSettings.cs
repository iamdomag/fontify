using Fontify.Options;
using Fontify.Tools;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Fontify.Services
{
    [DefaultProperty("BaseFontFamily")]
    internal class FontSettings
    {        
        [Category("Font Defaults")]
        [DisplayName("Font Family")]
        [TypeConverter(typeof(FontFamilyNames))]
        public string BaseFontFamily { get; set; }

        [Category("Font Defaults")]
        [DisplayName("Normal")]
        [TypeConverter(typeof(TypefaceNames))]
        public string NormalTypeface { get; set; }

        [Category("Font Defaults")]
        [DisplayName("Bold")]
        [TypeConverter(typeof(TypefaceNames))]
        public string BoldTypeface { get; set; }

        [Category("Font Defaults")]
        [DisplayName("Italic")]
        [TypeConverter(typeof(TypefaceNames))]
        public string ItalicTypeface { get; set; }

        [Category("Font Defaults")]
        [DisplayName("BolcItalic")]
        [TypeConverter(typeof(TypefaceNames))]
        public string BoldItalicTypeface { get; set; }

        [Category("Settings")]
        [DisplayName("Italic Classifiers")]
        [DefaultValue("comment,string,interface name,keyword - control,method name,namespace name")]
        public string ItalicClassifiers { get; set; } = "comment,string,interface name,keyword - control,method name,namespace name";
    }
}
