using LuanSC.Data.Components;
using LuanSC.Objects.Mechs;
using LuanSC.Objects.Summons;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Spells;

/// <summary>
/// Spell factories. Put the results in a SpellbookComponent.
///
/// FIGHT FEED MESSAGES:
///   Set CastMessage in the initialiser to change what the fight feed says when it's cast, e.g.
///     new SummonSpell(...) { CastMessage = "{caster} rips open a portal and a {summon} crawls out." }
///   {caster} and {spell} get filled in, summons also get {summon}. Leave it out for the default,
///   "{caster} casts {spell}!" or "{caster} summoned {summon}." See TitanAplu.
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

    // flat magic damage to everything in the circle, friends included unless friendlyFire is off
    public static ISpell Fireball(int damage = 30, int radius = 2, int range = 10, int mpCost = 50, bool friendlyFire = true)
    {
        return new Fireball(damage, radius, range, new ResourceCost(ResourceType.MP, mpCost))
        {
            FriendlyFire = friendlyFire
        };
    }

    // single target heal on whoever is under the cursor
    public static ISpell Heal(int amount = 50, int range = 5, int mpCost = 20)
    {
        return new Heal(amount, range, new ResourceCost(ResourceType.MP, mpCost));
    }

    // overdrive special: magic damage across the current weapon's range
    public static ISpell Unsheath(int damage = 50, int spCost = 10, int odCost = 100)
    {
        return new Unsheath(damage, new ResourceCost(ResourceType.SP, spCost), new ResourceCost(ResourceType.OD, odCost));
    }

    // free skill, refills ammo. Refused when already full
    public static ISpell Reload()
    {
        return new Reload
        {
            CastMessage = "{caster} reloads."
        };
    }

    // Luna's overdrive: she pilots the mech until it's destroyed
    public static ISpell TitanAplu(int odCost = 100)
    {
        return new TransformSpell("Titanfall", "Protocol 3: Protect the Pilot", ActionType.SPECIAL, "TITAN APLU", 'T',
            MechRegistry.TitanAplu, new ResourceCost(ResourceType.GB, odCost))
        {
            CastMessage = "Prepare for Titanfall!",
            FormType = UnitType.MECHANICAL // back to human when she ejects
        };
    }
}
