namespace IntroBot3.BotLogic;

public enum ThemeType
{
    Intro,
    Outro,
}

public static class ThemeTypeExtensions
{
    public static string GetThemeTypeString(this ThemeType themeType)
    {
        return themeType switch
        {
            ThemeType.Intro => "intro",
            ThemeType.Outro => "outro",
            _ => throw new ArgumentOutOfRangeException(nameof(themeType), themeType, null)
        };
    }
}