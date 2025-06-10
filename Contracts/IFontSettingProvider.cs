using fontify.Model;
using System.Windows.Media;

namespace fontify.Contracts
{
    internal interface IFontSettingProvider
    {
        Task<Typeface?> GetTypefaceAsync(FontOverrideType type);
        Task<Dictionary<FontOverrideType?, Typeface?>> GetFontOverridesAsync();
        FontSetting? Settings { get; }
        Task<FontSetting> GetSettingsAsync();
        Task SaveSettingsAsync();
    }

    internal enum FontOverrideType
    {
        Normal,
        Bold,
        Italic,
        BoldItalic,
        LineNumber
    }
}
