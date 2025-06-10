using fontify.Contracts;
using fontify.Model;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

namespace fontify.Services
{
    internal class FontSettingProvider : IFontSettingProvider
    {
        private FontSetting? _settings = null;
        private ISettingStorage<FontSetting> _fontSettingsStorage;
        public FontSetting? Settings
        {
            get => _settings;
            private set => _settings = value;
        }

        public FontSettingProvider(ISettingStorage<FontSetting> settingStorage)
        {
            _fontSettingsStorage = settingStorage;
            //ThreadHelper.JoinableTaskFactory.Run(async () => _settings = await settingStorage.GetSettingsAsync());
        }

        public async Task<Typeface?> GetTypefaceAsync(FontOverrideType type)
        {
            _settings ??= await GetSettingsAsync();

            return GetTypeface(type);
        }

        private Typeface? GetTypeface(FontOverrideType? type) => type switch
        {
            FontOverrideType.Normal => new Typeface(_settings?.NormalTypeface),
            FontOverrideType.Bold => new Typeface(_settings?.BoldTypeface),
            FontOverrideType.Italic => new Typeface(_settings?.ItalicTypeface),
            FontOverrideType.BoldItalic => new Typeface(_settings?.BoldItalicTypeface),
            FontOverrideType.LineNumber => new Typeface(_settings?.LineNumber),
            _ => new Typeface(new FontFamily(_settings.BaseFontFamily), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal)
        };


        public async Task<Dictionary<FontOverrideType?, Typeface?>> GetFontOverridesAsync()
        {
            _settings ??= await GetSettingsAsync();
            var result = new Dictionary<FontOverrideType?, Typeface?>();
            foreach (var key in Enum.GetNames(typeof(FontOverrideType)))
            {
                FontOverrideType? overrideType = Enum.Parse(typeof(FontOverrideType), key) as FontOverrideType?;
                result.Add(overrideType, GetTypeface(overrideType));
            }
            
            return result;
        }

        private FontSetting GetDefaultSettings()
        {
            const string DefaultFontFamilyName = "JetBrains Mono";
            const string LineNumberDefault = "Roboto";
            var defaultFontFamily = new FontFamily(DefaultFontFamilyName);
            var typefaces = defaultFontFamily.FamilyTypefaces?
                .ToDictionary(x => (x.Style, x.Weight), x => string.Format("{0} {1}", DefaultFontFamilyName, x.AdjustedFaceNames?.FirstOrDefault().Value));

            return new FontSetting
            {
                BaseFontFamily = defaultFontFamily.ToString(),
                NormalTypeface = typefaces[(FontStyles.Normal, FontWeights.Regular)],
                ItalicTypeface = typefaces[(FontStyles.Italic, FontWeights.SemiBold)],
                BoldTypeface = typefaces[(FontStyles.Normal, FontWeights.ExtraBold)],
                BoldItalicTypeface = typefaces[(FontStyles.Italic, FontWeights.ExtraBold)],
                LineNumber = LineNumberDefault
            };
        }

        public async Task<FontSetting> GetSettingsAsync()
        {
            _settings = await _fontSettingsStorage.GetSettingsAsync();

            if (_settings == null)
            {
                _settings = GetDefaultSettings();
                await SaveSettingsAsync();
            }
            
            return _settings;
        }

        public async Task SaveSettingsAsync()
        {
            await _fontSettingsStorage.SaveSettingsAsync(_settings);
        }
    }

}
