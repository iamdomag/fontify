using fontify.Contracts;
using fontify.Model;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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

        public async Task<Typeface?> GetTypefaceAsync(FontOverride type)
        {
            _settings ??= await GetSettingsAsync();

            return GetTypeface(type);
        }

        private Typeface? GetTypeface(FontOverride type) => type switch
        {
            FontOverride.Normal => new Typeface(_settings?.NormalTypeface),
            FontOverride.Bold => new Typeface(_settings?.BoldTypeface),
            FontOverride.Italic => new Typeface(_settings?.ItalicTypeface),
            FontOverride.BoldItalic => new Typeface(_settings?.BoldItalicTypeface),
            FontOverride.LineNumber => new Typeface(new FontFamily("Roboto"), FontStyles.Normal, FontWeights.Light, FontStretches.Normal),
            _ => new Typeface(new FontFamily(_settings.BaseFontFamily), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal)
        };


        public async Task<Dictionary<FontOverride, Typeface>> GetFontOverrides()
        {
            _settings ??= await GetSettingsAsync();
            var result = new Dictionary<FontOverride, Typeface>();
            
            //foreach(var item in FontOverride[])
            //{
            //    var typeface = GetTypeface(item)
            //}
            throw new NotImplementedException();
        }

        private FontSetting GetDefaultSettings()
        {
            const string DefaultFontFamilyName = "JetBrains Mono";
            var defaultFontFamily = new FontFamily(DefaultFontFamilyName);
            var typefaces = defaultFontFamily.FamilyTypefaces?
                .ToDictionary(x => (x.Style, x.Weight), x => string.Format("{0} {1}", DefaultFontFamilyName, x.AdjustedFaceNames?.First().Value));
            return new FontSetting
            {
                BaseFontFamily = defaultFontFamily.ToString(),
                NormalTypeface = typefaces[(FontStyles.Normal, FontWeights.Regular)],
                ItalicTypeface = typefaces[(FontStyles.Italic, FontWeights.SemiBold)],
                BoldTypeface = typefaces[(FontStyles.Normal, FontWeights.ExtraBold)],
                BoldItalicTypeface = typefaces[(FontStyles.Italic, FontWeights.Bold)]
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
