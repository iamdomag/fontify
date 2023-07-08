using Fontify.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using System.Composition.Convention;

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
                var fontFamily = new FontFamily(settings.BaseFontFamily);
                typefaces = fontFamily.GetTypefaces()
                    .Select(x => $"{settings.BaseFontFamily} {x.FaceNames.FirstOrDefault().Value}")
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }

            return new StandardValuesCollection(typefaces);
        }
    }
}
