using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components;

public enum ResourceType
{
    MP,
    METAL
    // add more here, then a factory in ResourceRegistry
}

/// <summary>
/// An amount of some resource that abilities can cost. Look one up with GameObject.GetResource.
/// </summary>
public sealed record ResourceCost(ResourceType Type, int Amount);

/// <summary>
/// A spendable pool like MP or metal. One component per resource type.
/// </summary>
public class ResourceComponent : StatComponent, IHudDisplay, IOnTurnStartListener
{
    public override string Name => $"{DisplayName} Component";

    public ResourceType Type { get; }
    public string DisplayName { get; set; }  // used in the fight feed, e.g. "Metal"
    public string Label { get; set; }        // used in the HUD, 3 characters max, e.g. "MT"

    public Color BarColor { get; set; }
    public Color LabelColor { get; set; }
    public Color? TextColor { get; set; }
    public int HudOrder { get; set; } = 40;

    /// <summary>
    /// Added at the start of the owner's turn. Can be negative for things that drain.
    /// </summary>
    public int RegenPerTurn { get; set; }

    public ResourceComponent(ResourceType type, string displayName, string label, Color color, int max)
    {
        Type = type;
        DisplayName = displayName;
        Label = label;
        BarColor = color;
        LabelColor = color;

        Max = max;
        Current = max;
    }

    public bool CanSpend(int amount) => Current >= amount;

    /// <returns>false if there isn't enough. Nothing is spent in this case.</returns>
    public bool TrySpend(int amount)
    {
        if (!CanSpend(amount)) return false;

        Current -= amount;
        return true;
    }

    public void OnTurnStart()
    {
        if (RegenPerTurn != 0)
            Current += RegenPerTurn; // clamped
    }

    public IEnumerable<HudBar> GetHudBars()
    {
        yield return new HudBar(HudOrder, $"{Label,-3}:", LabelColor, BarColor, HudBar.Ratio(Current, Max), $"{Current} / {Max}", TextColor);
    }
}
