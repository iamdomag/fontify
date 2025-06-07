using fontify.Model;
using System.Windows.Media;

namespace fontify.Contracts
{
    internal interface IFontSettingProvider
    {
        Task<Typeface?> GetTypefaceAsync(FontOverride type);
        FontSetting? Settings { get; }
        Task<FontSetting> GetSettingsAsync();
        Task SaveSettingsAsync();
    }

    internal enum FontOverride
    {
        Normal,
        Bold,
        Italic,
        BoldItalic,
        LineNumber
    }
}
