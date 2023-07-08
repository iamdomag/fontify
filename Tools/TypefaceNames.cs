using Fontify.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.ComponentModel.TypeConverter;
using System.Windows.Media;

namespace Fontify.Tools
{
    internal class TypefaceNames : StringConverter
    {
        FontSettingsService _settingsService = FontSettingsService.GetInstance();
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var settings = _settingsService.Settings;
            var typefaces = new List<string>();

            if (!string.IsNullOrEmpty(settings.BaseFontFamily))
            {
                typefaces = Fonts.SystemTypefaces
                    .Where(x => x.FontFamily.Source != settings.BaseFontFamily)
                    .Select(x => $"{settings.BaseFontFamily} {x.FaceNames.FirstOrDefault().Value}")
                    .ToList();
            }

            return new StandardValuesCollection(typefaces);
        }
    }
}
