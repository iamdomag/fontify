using fontify.Model;
using System.Windows.Media;

namespace fontify.Contracts
{
    internal interface IFontSettingProvider
    {
        Task<Typeface> GetTypefaceAsync(FontOverrides type);
        FontSetting? Settings { get; }
        Task<FontSetting> GetSettingsAsync();
        Task SaveSettingsAsync();
    }

    internal enum FontOverrides
    {
        Normal,
        Bold,
        Italic,
        BoldItalic
    }
}
