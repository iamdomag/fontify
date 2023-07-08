﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Fontify.Tools
{
    internal class FontFamilyNames : StringConverter
    {
        List<string> _familyNames;
        public FontFamilyNames()
        {
            _familyNames = Fonts.SystemFontFamilies
                .Select(x => x.Source)
                .ToList();
        }
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) => new StandardValuesCollection(_familyNames);
    }
}
