using fontify.Contracts;
using fontify.Model;
using Microsoft.VisualStudio.Extensibility.VSSdkCompatibility;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using System.Reflection;

namespace fontify.Services
{
    public class SettingStorage<T> : ISettingStorage<T> where T: class, new()
    {
        private ShellSettingsManager? _shellSettingsCache;
        private PropertyInfo[] _properties;
        private string _collectionPath;
        private MefInjection<IAsyncServiceProvider> _injector;
        private readonly IServiceProvider _serviceProvider;

        public SettingStorage(MefInjection<IAsyncServiceProvider> injector, IServiceProvider serviceProvider)
        {
            _collectionPath = $"Fontify\\{nameof(T)}";
            _injector = injector;
            _serviceProvider = serviceProvider;
        }

        private async Task<ShellSettingsManager> GetShellSettingsManagerAsync()
        {
            if (_shellSettingsCache == null)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                var sp = _serviceProvider.GetService(typeof(IAsyncServiceProvider2)) as IAsyncServiceProvider2;
                var service = await sp.GetServiceAsync(typeof(SVsSettingsManager), true) as IVsSettingsManager;

                _shellSettingsCache = new ShellSettingsManager(service);
            }

            return _shellSettingsCache;
        }

        public async Task<T?> GetSettingsAsync()
        {
            var settingsManager = await GetShellSettingsManagerAsync();
            var settingsStore = settingsManager.GetReadOnlySettingsStore(SettingsScope.UserSettings);
            T? settings = null;

            _properties = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToArray();

            if (settingsStore.CollectionExists(_collectionPath))
            {
                settings = new T();
                Array.ForEach(_properties, props =>
                {
                    if (settingsStore.PropertyExists(_collectionPath, props.Name))
                    {
                        var type = props.PropertyType;
                        object? propertyValue = null;
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
