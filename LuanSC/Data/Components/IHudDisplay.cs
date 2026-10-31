using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components;

/// <summary>
/// Components that show up in the HUD. The HUD collects bars from every component on the current object,
/// sorts them by Order and fills its slots top to bottom. No component, no bar.
/// </summary>
public interface IHudDisplay
{
    IEnumerable<HudBar> GetHudBars();
}

/// <summary>
/// One row in the HUD.
/// </summary>
/// <param name="Order">Lower goes higher up. HP 0, MP 10, SP 20, OD 30, resources 40, actions 50.</param>
/// <param name="Label">Up to 4 characters, e.g. "HP :".</param>
/// <param name="Progress">0 to 1.</param>
/// <param name="Text">Drawn over the bar.</param>
/// <param name="TextColor">null keeps the progress bar's default.</param>
/// <param name="TrackGlyph">Drawn on the empty part of the bar, e.g. 176 for a light shade. null keeps the progress bar's default.</param>
/// <param name="TrackColor">Colour of TrackGlyph. null keeps the theme's colour.</param>
/// <param name="TrackBackground">Background of the empty part, text included. null keeps the theme's (black).
/// This is the one that fixes dark TextColors vanishing once the bar empties: the text replaces the glyph under it, so only the background shows behind it.</param>
public record HudBar(int Order, string Label, Color LabelColor, Color BarColor, float Progress, string Text, Color? TextColor = null,
    int? TrackGlyph = null, Color? TrackColor = null, Color? TrackBackground = null)
{
    public static float Ratio(int current, int max) => max > 0 ? (float)current / max : 0f;

    public const int TRACK_GLYPH = 176; // light shade

    /// <summary>
    /// The standard track for bars with dark text: light shade in a dim version of the bar colour, on a slightly brighter dim background.
    /// Same proportions as Metal's (Silver over DimGray on Gray).
    /// </summary>
    public static (int Glyph, Color Color, Color Background) DimTrack(Color barColor)
        => (TRACK_GLYPH, Dim(barColor, 0.55f), Dim(barColor, 0.67f));

    public static Color Dim(Color color, float factor)
        => new((int)(color.R * factor), (int)(color.G * factor), (int)(color.B * factor));
}
