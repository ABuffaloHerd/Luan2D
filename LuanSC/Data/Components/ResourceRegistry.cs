using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components;

/// <summary>
/// Resource factories. MP lives in MPComponent since every caster has it.
/// </summary>
public static class ResourceRegistry
{
    // Engineer build material. Trickles in every turn.
    public static ResourceComponent Metal(int max = 200, int start = 100, int regen = 25)
    {
        ResourceComponent metal = new(ResourceType.METAL, "Metal", "MT", Color.Silver, max)
        {
            TextColor = Color.Black,
            RegenPerTurn = regen
        };
        metal.Current = start;
        return metal;
    }
}
