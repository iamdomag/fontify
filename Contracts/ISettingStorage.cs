using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fontify.Contracts
{
    internal interface ISettingStorage<T> where T : class, new()
    {
        Task<T?> GetSettingsAsync();
        Task SaveSettingsAsync(T settings);
    }
}
