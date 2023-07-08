using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Threading;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Fontify.Services
{
    internal class FontSettingsService
    {
        private static readonly AsyncLazy<FontSettingsService> liveInstance;

        static FontSettingsService() => liveInstance = new AsyncLazy<FontSettingsService>(CreateInstanceAsync, ThreadHelper.JoinableTaskFactory);

        private static async Task<FontSettingsService> CreateInstanceAsync() => await Task.FromResult(new FontSettingsService());

        public static FontSettingsService GetInstance() => liveInstance.GetValue();

        public static async Task<FontSettingsService> GetInstanceAsync(bool loadStorage = false)
        {
            var instance = await liveInstance.GetValueAsync();
            if (loadStorage && !instance.settingsLoaded)
            {
                await instance.LoadStorageAsync();
            }

            return instance;
        }

        private readonly AsyncLazy<ShellSettingsManager> settingsManager;
        private readonly FontSettings settings;
        private const string CollectionPath = "Fontify\\FontSettings";
        private bool settingsLoaded = false;
        public string DefaultFontFamily { get; set; } = "Consolas";
        public FontSettings Settings => settings;
        private PropertyInfo[] FontSettingsProperties;

        private FontSettingsService()
        {
            settings = new FontSettings();
            settingsManager = new AsyncLazy<ShellSettingsManager>(GetShellSettingsManagerAsync, ThreadHelper.JoinableTaskFactory);
            FontSettingsProperties = typeof(FontSettings)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToArray();
        }

        private async Task<ShellSettingsManager> GetShellSettingsManagerAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var service = await AsyncServiceProvider.GlobalProvider.GetServiceAsync(typeof(SVsSettingsManager)) as IVsSettingsManager;

            return new ShellSettingsManager(service);
        }

        public async Task LoadStorageAsync()
        {
            var settingsManager = await this.settingsManager.GetValueAsync();
            var settingsStore = settingsManager.GetReadOnlySettingsStore(SettingsScope.UserSettings);

            if (settingsStore.CollectionExists(CollectionPath))
            {
                Array.ForEach(FontSettingsProperties, props =>
                {
                    var propertyValue = settingsStore.GetString(CollectionPath, props.Name);
                    props.SetValue(settings, propertyValue);
                });
            }
        }

        public async Task SaveStorageAsync()
        {
            var settingsManager = await this.settingsManager.GetValueAsync();
            var settingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            if (!settingsStore.CollectionExists(CollectionPath))
            {
                settingsStore.CreateCollection(CollectionPath);
            }

            Array.ForEach(FontSettingsProperties, props =>
            {
                var propertyValue = props.GetValue(settings) as string;
                settingsStore.SetString(CollectionPath, props.Name, propertyValue);
            });
        }

        public Typeface GetTypeface(FontOverrides type)
        {
            var defaultFontFamily = new FontFamily("Consolas");
            var typeface = new Typeface(defaultFontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

            if (!string.IsNullOrEmpty(settings.BaseFontFamily))
            {
                defaultFontFamily = new FontFamily(settings.BaseFontFamily);

                switch (type)
                {
                    case FontOverrides.Normal:
                        typeface = !string.IsNullOrEmpty(settings.NormalTypeface) ?
                            new Typeface(settings.NormalTypeface) :
                            new Typeface(defaultFontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                        break;
                    case FontOverrides.Bold:
                        typeface = !string.IsNullOrEmpty(settings.BoldTypeface) ?
                            new Typeface(settings.BoldTypeface) :
                            new Typeface(defaultFontFamily, FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
                        break;
                    case FontOverrides.Italic:
                        typeface = !string.IsNullOrEmpty(settings.ItalicTypeface) ?
                            new Typeface(settings.ItalicTypeface) :
                            new Typeface(defaultFontFamily, FontStyles.Italic, FontWeights.Normal, FontStretches.Normal);
                        break;
                    case FontOverrides.BoldItalic:
                        typeface = !string.IsNullOrEmpty(settings.BoldItalicTypeface) ?
                            new Typeface(settings.BoldItalicTypeface) :
                            new Typeface(defaultFontFamily, FontStyles.Italic, FontWeights.Bold, FontStretches.Normal);
                        break;
                }
            }

            return typeface;
        }

        public string[] GetItalicClassifiers()
        {
            var classifiers = settings?.ItalicClassifiers.Split(',');
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
