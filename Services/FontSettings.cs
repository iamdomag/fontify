using Fontify.Options.PropertyGridEx;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;

namespace Fontify.Services
{
    [DefaultProperty("BaseFontFamily")]
    internal class FontSettings
    {
        [Browsable(false)]
        private bool _enabled = true;

        [Category("\t\t\tGeneral")]
        [DisplayName("Enable")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(this, new[] { new ReadOnlyAttribute(_enabled) }))
                {
                    if (prop.Name != "Enabled")
                    {
                        var att = (ReadOnlyAttribute)prop.Attributes[typeof(ReadOnlyAttribute)];
                        var field = att.GetType().GetField("isReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);

                        field.SetValue(att, !_enabled);
                    }
                }
            }
        }

        [Category("\t\tFont Defaults")]
        [DisplayName("Font Family")]
        [ReadOnly(false)]
        [Editor(typeof(FontListEditor), typeof(UITypeEditor))]
        public string BaseFontFamily { get; set; }

        [Category("\t\tFont Defaults")]
        [DisplayName("Normal")]
        [ReadOnly(false)]
        [Editor(typeof(FontListEditor), typeof(UITypeEditor))]
        public string NormalTypeface { get; set; }

        [Category("\t\tFont Defaults")]
        [DisplayName("Bold")]
        [ReadOnly(false)]
        [Editor(typeof(FontListEditor), typeof(UITypeEditor))]
        public string BoldTypeface { get; set; }

        [Category("\t\tFont Defaults")]
        [DisplayName("Italic")]
        [ReadOnly(false)]
        [Editor(typeof(FontListEditor), typeof(UITypeEditor))]
        public string ItalicTypeface { get; set; }

        [Category("\t\tFont Defaults")]
        [DisplayName("BolcItalic")]
        [ReadOnly(false)]
        [Editor(typeof(FontListEditor), typeof(UITypeEditor))]
        public string BoldItalicTypeface { get; set; }

        [Category("\tAdvanced")]
        [DisplayName("Italic Classifiers")]
        [ReadOnly(false)]
        [DefaultValue("comment,string,interface name,keyword - control,method name,namespace name")]
        public string ItalicClassifiers { get; set; } = "comment,string,interface name,keyword - control,method name,namespace name";
    }
}
