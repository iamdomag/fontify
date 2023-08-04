using Fontify.Services;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Integration;

namespace Fontify.Options.PropertyGridEx
{
    public class FontListEditor : UITypeEditor
    {
        private IWindowsFormsEditorService editorService;
        private ElementHost wrapper = null;

        public FontListEditor()
        {
            wrapper = new ElementHost
            {
                Child = new FontDropdown(),
                Height = 350,
                Width = 300,
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom
            };
            ((FontDropdown)wrapper.Child).SelectionChanged += delegate (object sender, EventArgs e)
            {
                editorService.CloseDropDown();
            };
        }
        public override bool IsDropDownResizable => true;
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.DropDown;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var fontList = wrapper.Child as FontDropdown;

            if (editorService == null)
            {
                editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            }

            if (context.PropertyDescriptor.Name == "BaseFontFamily")
            {
                fontList.LoadFonts((string)value);
            }
            else
            {
                var fontFamily = (context.Instance as FontSettings).BaseFontFamily;
                fontList.LoadFonts((string)value, fontFamily);
            }

            editorService.DropDownControl(wrapper);
            value = fontList.SelectedItem ?? value;

            return base.EditValue(context, provider, value);
        }
    }
}
