using Fontify.Services;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using WinMedia = System.Windows.Media;

namespace Fontify.Options.PropertyGridEx
{
    public class FontListEditor : UITypeEditor
    {
        private IWindowsFormsEditorService editorService;
        private RenderedFontList renderedFontList = null;

        public override bool IsDropDownResizable => true;
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.DropDown;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            
            if (renderedFontList == null)
            {
                renderedFontList = new RenderedFontList();
                renderedFontList.MouseClick += delegate (object sender, MouseEventArgs e)
                {
                    if (e.Button == MouseButtons.Left) 
                    {
                        editorService.CloseDropDown();
                    }
                };
            }
            
            if (context.PropertyDescriptor.Name == "BaseFontFamily")
            {
                renderedFontList.LoadFonts(value.ToString());
            } else
            {
                var fontFamily = (context.Instance as FontSettings).BaseFontFamily;
                renderedFontList.LoadFonts(value.ToString(), fontFamily);
            }

            editorService.DropDownControl(renderedFontList);
            value = renderedFontList.SelectedItem ?? value;
            
            return base.EditValue(context, provider, value);
        }
    }
    public class RenderedFontList : ListBox
    {

        private List<string> fonts = new List<string>();
        private List<string> fontFamilyNames;
        private const int DefaultFontSize = 12;
        private const int TextPadding = 3;
        private string baseFontFamily = "";
        private StringFormat format;

        public RenderedFontList() : base()
        {
            format = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoFontFallback)
            {
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter                
            };
            DrawMode = DrawMode.OwnerDrawVariable;            
            BorderStyle = BorderStyle.None;
            Dock = DockStyle.Fill;
            SelectionMode = SelectionMode.One;
            Height = 300;
            Width = 350;
            fontFamilyNames = WinMedia.Fonts.SystemFontFamilies
                .Select(x => x.Source)
                .OrderBy(x => x)
                .ToList();
        }

        public void LoadFonts(string selectedItem, string fontFamily = null)
        {
            if (string.IsNullOrEmpty(fontFamily))
            {
                fonts = fontFamilyNames;
                baseFontFamily = selectedItem;
            }
            else
            {
                var baseFont = new WinMedia.FontFamily(fontFamily);                
                fonts = baseFont.GetTypefaces()
                    .Select(x => $"{fontFamily} {x.FaceNames.FirstOrDefault().Value}")
                    .ToList();
                baseFontFamily = fontFamily;
            }

            SetItemsCore(fonts);
            var idx = FindString(selectedItem);
            if (idx != -1)
            {
                SetSelected(idx, true);
            }
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            base.OnMeasureItem(e);

            e.Graphics.PageUnit = GraphicsUnit.Display;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var text = fonts[e.Index];
            var style = GetFontStyle(text);
            var fontName = GetProperFontName(text, baseFontFamily);

            using (var font = new Font(fontName, DefaultFontSize, style))
            {                
                var fontSize = e.Graphics.MeasureString(text, font, e.ItemWidth, format);                
                e.ItemHeight = (int)fontSize.Height + (TextPadding * 2);
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);
            
            if (e.Index < 0) return;

            var text = fonts[e.Index];
            var style = GetFontStyle(text);
            var fontName = GetProperFontName(text, baseFontFamily);

            using (var font = new Font(fontName, DefaultFontSize, style))
            {
                e.Graphics.PageUnit = GraphicsUnit.Display;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                e.DrawBackground();
                e.Graphics.DrawString(text, font, new SolidBrush(e.ForeColor), e.Bounds, format);
                e.DrawFocusRectangle();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Refresh();
        }

        private string GetProperFontName(string fontName, string fontFamily)
        {
            if (fontName != fontFamily)
            {
                fontName = fontName.Replace(" Regular", string.Empty)
                    .Replace(" Bold", string.Empty)
                    .Replace(" Italic", string.Empty)
                    .Replace(" Oblique", string.Empty);
            }

            return fontName;
        }

        private string[] BoldWeightFonts = new[]
        {
            "Medium",
            "SemiBold",
            "Bold",
            "ExtraBold",
            "Heavy",
            "Bold"
        };

        private string[] ItalicFonts = new[]
        {
            "Italic",
            "Oblique"
        };

        private FontStyle GetFontStyle(string fontName)
        {
            FontStyle style = FontStyle.Regular;
            var fontNameItems = fontName.Split(' ');

            if (fontNameItems.Any(c => BoldWeightFonts.Contains(c)))
            {
                style = FontStyle.Bold;
            }

            if (fontNameItems.Any(x => ItalicFonts.Contains(x)))
            {
                style = style | FontStyle.Italic;
            }

            return style;
        }
    }
}
