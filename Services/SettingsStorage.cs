using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Fontify.Services
{
    public class SettingsStorage<T> where T: class, new()
    {
        private ShellSettingsManager _shellSettingsCache;
        private PropertyInfo[] _properties;
        private string _collectionPath;

        public SettingsStorage()
        {
            _properties = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToArray();
            _collectionPath = $"{FontifyPackage.PackageName}\\{nameof(T)}";
        }

        private async Task<ShellSettingsManager> GetShellSettingsManagerAsync()
        {
            if (_shellSettingsCache == null)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                var service = await AsyncServiceProvider.GlobalProvider.GetServiceAsync(typeof(SVsSettingsManager)) as IVsSettingsManager;

                _shellSettingsCache = new ShellSettingsManager(service);
            }

            return _shellSettingsCache;
        }

        public async Task<T> GetSettingsAsync()
        {
            var settingsManager = await GetShellSettingsManagerAsync();
            var settingsStore = settingsManager.GetReadOnlySettingsStore(SettingsScope.UserSettings);
            var settings = new T();

            if (settingsStore.CollectionExists(_collectionPath))
            {
                Array.ForEach(_properties, props =>
                {
                    if (settingsStore.PropertyExists(_collectionPath, props.Name))
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
                                propertyValue = settingsStore.GetBoolean(_collectionPath, props.Name);
                                break;
                            case nameof(Int32):
                                propertyValue = settingsStore.GetInt32(_collectionPath, props.Name);
                                break;
                            default:
                                propertyValue = settingsStore.GetString(_collectionPath, props.Name);
                                break;
                        }

                        props.SetValue(settings, propertyValue);
                    }
                });
            }

            return settings;
        }

        public async Task SaveSettingsAsync(T settings)
        {
            var settingsManager = await GetShellSettingsManagerAsync();
            var settingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            if (!settingsStore.CollectionExists(_collectionPath))
            {
                settingsStore.CreateCollection(_collectionPath);
            }

            Array.ForEach(_properties, props =>
            {
                var propertyValue = props.GetValue(settings);

                switch (propertyValue)
                {
                    case bool booleanValue:
                        settingsStore.SetBoolean(_collectionPath, props.Name, booleanValue);
                        break;
                    case int intValue:
                        settingsStore.SetInt32(_collectionPath, props.Name, intValue);
                        break;
                    case double doubleValue:
                        settingsStore.SetString(_collectionPath, props.Name, doubleValue.ToString());
                        break;
                    default:
                        settingsStore.SetString(_collectionPath, props.Name, propertyValue.ToString());
                        break;
                }
            });
        }
    }
}
