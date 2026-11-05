using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components;

/// <summary>
/// Resource factories. MP lives in MPComponent since every caster has it.
/// char 176 for 1/3 gradient
/// char 177 for 2/3 gradient
/// </summary>
public static class ResourceRegistry
{
    // Engineer build material. Trickles in every turn.
    public static ResourceComponent Metal(int max = 200, int start = 100, int regen = 25)
    {
        ResourceComponent metal = new(ResourceType.METAL, "Metal", "MT", Color.Silver, max)
        {
            TextColor = Color.Black,
            RegenPerTurn = regen,

            // grey track so the black numbers stay readable once the bar runs low
            TrackGlyph = 176,
            TrackColor = Color.DimGray,
            TrackBackground = Color.Gray,
        };
        metal.Current = start;
        return metal;
    }

    // Shells, bullets, whatever the weapon eats. Starts full, only Reload refills it.
    public static ResourceComponent Ammo(int max = 2)
    {
        // HUD labels are 3 characters, AMMO doesn't fit
        return new ResourceComponent(ResourceType.AMMO, "Ammo", "A", Color.DarkGreen, max);
    }
}
