using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Threading;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Fontify.Services
{
    internal class FontSettingsService
    {
        private static readonly Lazy<FontSettingsService> _lazy;
        static FontSettingsService() => _lazy = new Lazy<FontSettingsService>(() => new FontSettingsService());

        public static async Task<FontSettingsService> GetInstanceAsync(bool loadStorage = false, FontSettings settings = null)
        {
            var instance = _lazy.Value;

            instance.Settings = settings ?? instance.Settings;
            if (loadStorage && instance.Settings == null)
            {
                await instance.LoadSettingsAsync();
            }

            return instance;
        }

        private FontSettings _settings = null;
        private SettingsStorage<FontSettings> _fontSettingsStorage;
        public FontSettings Settings
        {
            get => _settings;
            private set => _settings = value;
        }

        private FontSettingsService()
        {
            _fontSettingsStorage = new SettingsStorage<FontSettings>();
        }

        private async Task LoadSettingsAsync()
        {
            _settings = await _fontSettingsStorage.GetSettingsAsync();
        }

        public Typeface GetTypeface(FontOverrides type)
        {
            var defaultFontFamily = new FontFamily("Consolas");
            var typeface = new Typeface(defaultFontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

            if (!string.IsNullOrEmpty(_settings.BaseFontFamily))
            {
                defaultFontFamily = new FontFamily(_settings.BaseFontFamily);

                switch (type)
                {
                    case FontOverrides.Normal:
                        typeface = !string.IsNullOrEmpty(_settings.NormalTypeface) ?
                            new Typeface(_settings.NormalTypeface) :
                            new Typeface(defaultFontFamily, FontStyles.Normal, FontWeights.Regular, FontStretches.Normal);
                        break;
                    case FontOverrides.Bold:
                        typeface = !string.IsNullOrEmpty(_settings.BoldTypeface) ?
                            new Typeface(_settings.BoldTypeface) :
                            new Typeface(defaultFontFamily, FontStyles.Normal, FontWeights.ExtraBold, FontStretches.Normal);
                        break;
                    case FontOverrides.Italic:
                        typeface = !string.IsNullOrEmpty(_settings.ItalicTypeface) ?
                            new Typeface(_settings.ItalicTypeface) :
                            new Typeface(defaultFontFamily, FontStyles.Italic, FontWeights.Normal, FontStretches.Normal);
                        break;
                    case FontOverrides.BoldItalic:
                        typeface = !string.IsNullOrEmpty(_settings.BoldItalicTypeface) ?
                            new Typeface(_settings.BoldItalicTypeface) :
                            new Typeface(defaultFontFamily, FontStyles.Italic, FontWeights.SemiBold, FontStretches.Normal);
                        break;
                }
            }

            return typeface;
        }

        public string[] GetItalicClassifiers()
        {
            var classifiers = _settings?.ItalicClassifiers.Split(',');
            Array.ForEach(classifiers, x => x.Trim());
            return classifiers;
        }
    }

    internal enum FontOverrides
    {
        Normal,
        Bold,
        Italic,
        BoldItalic
    }
}
