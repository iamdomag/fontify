using Microsoft.Internal.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
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
                    if (settingsStore.PropertyExists(CollectionPath, props.Name))
                    {
                        var type = props.PropertyType;
                        object propertyValue = null;
                        var typeName = type.Name;

                        if (typeName == typeof(Nullable<>).Name)
                        {
                            typeName = Nullable.GetUnderlyingType(type).Name;
                        }

                        switch (typeName)
                        {
                            case nameof(Boolean):
                                propertyValue = settingsStore.GetBoolean(CollectionPath, props.Name);
                                break;
                            case nameof(Int32):
                                propertyValue = settingsStore.GetInt32(CollectionPath, props.Name);
                                break;
                            default:
                                propertyValue = settingsStore.GetString(CollectionPath, props.Name);
                                break;
                        }

                        props.SetValue(settings, propertyValue);
                    }
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
                var propertyValue = props.GetValue(settings);

                switch (propertyValue)
                {
                    case bool booleanValue:
                        settingsStore.SetBoolean(CollectionPath, props.Name, booleanValue);
                        break;
                    case int intValue:
                        settingsStore.SetInt32(CollectionPath, props.Name, intValue);
                        break;
                    case double doubleValue:
                        settingsStore.SetString(CollectionPath, props.Name, doubleValue.ToString());
                        break;
                    default:
                        settingsStore.SetString(CollectionPath, props.Name, propertyValue.ToString());
                        break;
                }
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
                            new Typeface(defaultFontFamily, FontStyles.Normal, FontWeights.Regular, FontStretches.Normal);
                        break;
                    case FontOverrides.Bold:
                        typeface = !string.IsNullOrEmpty(settings.BoldTypeface) ?
                            new Typeface(settings.BoldTypeface) :
                            new Typeface(defaultFontFamily, FontStyles.Normal, FontWeights.ExtraBold, FontStretches.Normal);
                        break;
                    case FontOverrides.Italic:
                        typeface = !string.IsNullOrEmpty(settings.ItalicTypeface) ?
                            new Typeface(settings.ItalicTypeface) :
                            new Typeface(defaultFontFamily, FontStyles.Italic, FontWeights.Normal, FontStretches.Normal);
                        break;
                    case FontOverrides.BoldItalic:
                        typeface = !string.IsNullOrEmpty(settings.BoldItalicTypeface) ?
                            new Typeface(settings.BoldItalicTypeface) :
                            new Typeface(defaultFontFamily, FontStyles.Italic, FontWeights.SemiBold, FontStretches.Normal);
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
