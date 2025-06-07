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

        public async Task<Typeface> GetTypefaceAsync(FontOverrides type)
        {
            _settings ??= await GetSettingsAsync();

            var typeface = new Typeface(new FontFamily(_settings.BaseFontFamily), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

            if (!string.IsNullOrEmpty(_settings?.BaseFontFamily))
            {
                switch (type)
                {
                    case FontOverrides.Normal:
                        typeface = new Typeface(_settings?.NormalTypeface);
                        break;
                    case FontOverrides.Bold:
                        typeface = new Typeface(_settings?.BoldTypeface);
                        break;
                    case FontOverrides.Italic:
                        typeface = new Typeface(_settings?.ItalicTypeface);
                        break;
                    case FontOverrides.BoldItalic:
                        typeface = new Typeface(_settings?.BoldItalicTypeface);
                        break;
                }
            }

            return typeface;
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
                NormalTypeface = typefaces[(FontStyles.Normal, FontWeights.Normal)],
                ItalicTypeface = typefaces[(FontStyles.Italic, FontWeights.Normal)],
                BoldTypeface = typefaces[(FontStyles.Normal, FontWeights.Bold)],
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
