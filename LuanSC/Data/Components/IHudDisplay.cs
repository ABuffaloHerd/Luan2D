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
public record HudBar(int Order, string Label, Color LabelColor, Color BarColor, float Progress, string Text, Color? TextColor = null)
{
    public static float Ratio(int current, int max) => max > 0 ? (float)current / max : 0f;
}
