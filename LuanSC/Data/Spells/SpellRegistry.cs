using LuanSC.Data.Components;
using LuanSC.Objects.Summons;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Spells;

/// <summary>
/// Spell factories. Put the results in a SpellbookComponent.
/// </summary>
public static class SpellRegistry
{
    public static ISpell SummonFamiliar()
    {
        return new SummonSpell("Familiar", "A helper you control.", ActionType.SPELL, SummonRegistry.Familiar);
    }

    public static ISpell MagicCircle()
    {
        return new SummonSpell("Magic Circle", "+10 MP every turn.", ActionType.SPELL, SummonRegistry.MagicCircle, new ResourceCost(ResourceType.MP, 50))
        {
            Targeting = Targeting.CURSOR,
            Range = 5
        };
    }

    // engineer building, goes right in front so it can be hit straight away
    public static ISpell SentryGun(int hitsRequired = 3, char builtGlyph = 'Y')
    {
        return new SummonSpell("Sentry Gun", $"Hit {hitsRequired}x to build.", ActionType.SKILL,
            owner => SummonRegistry.SentryGun(owner, hitsRequired, builtGlyph),
            new ResourceCost(ResourceType.METAL, 100));
    }
}
