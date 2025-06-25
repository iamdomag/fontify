using fontify.Model;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
//using System.Windows.Forms.Integration;

namespace Fontify.Options.PropertyGridEx
{
    public class FontListEditor : UITypeEditor
    {
        private IWindowsFormsEditorService _editorService;
        //private ElementHost? wrapper = null;
        
        public FontListEditor(IWindowsFormsEditorService editorService)
        {
            //wrapper = new ElementHost
            //{
            //    Child = new FontDropdown(),
            //    Height = 350,
            //    Width = 300,
            //    Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom
            //};
            //((FontDropdown)wrapper.Child).SelectionChanged += delegate (object sender, EventArgs e)
            //{
            //    editorService?.CloseDropDown();
            //};
            _editorService = editorService;
        }
        public override bool IsDropDownResizable => true;
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.DropDown;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            //var fontList = wrapper?.Child as FontDropdown;

            //if (_editorService == null)
            //{
            //    _editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            //}

            //if (context.PropertyDescriptor.Name == "BaseFontFamily")
            //{
            //    fontList?.LoadFonts(value?.ToString() ?? string.Empty);
            //}
            //else
            //{
            //    var fontFamily = (context.Instance as FontSetting)?.BaseFontFamily;
            //    fontList?.LoadFonts(value?.ToString() ?? string.Empty, fontFamily);
            //}

            //_editorService.DropDownControl(wrapper);
            //if (fontList?.SelectedItem != null)
            //{
            //    value = fontList.SelectedItem;
            //}

            return base.EditValue(context, provider, value);
        }
    }
}
