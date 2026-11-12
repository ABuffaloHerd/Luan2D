internal static class GameSettings
{
    // How much of the arena is on screen, in cells. The combat layout is built around this.
    // Levels can be bigger than it, the arena scrolls (see CombatScene_Zoom).
    public const int ARENA_VIEW_SIZE = 45;
    public const int SIDEBAR_WIDTH = 45; // fight feed, turn order, HUD and ticker, borders included

    // the window: arena plus its border, then the sidebar. Boss bar on top of the arena
    public const int GAME_WIDTH = ARENA_VIEW_SIZE + 2 + SIDEBAR_WIDTH;
    public const int GAME_HEIGHT = 2 + ARENA_VIEW_SIZE + 2;
    public static readonly string FONT = FONTS.Bisasam.GetFont();

    // OD gained per point of damage taken, after DEF, RES and effects, for every unit. The fraction is dropped,
    // so at 0.1 a hit under 10 gives nothing, and neither does a blocked one. That's intended.
    public const float OD_CHARGE_RATE = 0.1f;

    // How many times faster AI turns run while Tab is held. 1 is normal speed.
    // Each frame runs one AI tick at most, so very high values all end up at one action per frame.
    public const float FAST_FORWARD_SPEED = 4f;

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

