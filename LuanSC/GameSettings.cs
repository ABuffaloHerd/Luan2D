internal static class GameSettings
{
    public const int GAME_WIDTH = 90;
    public const int GAME_HEIGHT = 47;
    public static readonly string FONT = FONTS.Bisasam.GetFont();

    public static string GetFont(this FONTS font)
        => font switch
        {
            FONTS.Bisasam => "Resources\\Font\\Bisasam.font",
            FONTS.Cheepicus16 => "Resources\\Font\\Cheepicus16.font",
            FONTS.Cheepicus12 => "Resources\\Font\\Cheepicus12.font",
            _ => throw new ArgumentOutOfRangeException(nameof(font), font, null)
        };
}

public enum FONTS
{
    Bisasam,
    Cheepicus16,
    Cheepicus12
}

