using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Spells;

/// <summary>
/// Refills the caster's ammo to max. Refused when it's already full, so the skill isn't wasted.
/// </summary>
public class Reload : CastSpell
{
    /// <summary>
    /// Fight feed line when there's nothing to reload. {caster} gets filled in.
    /// </summary>
    public string FullMessage { get; init; } = "{caster} is already loaded.";

    public Reload(params ResourceCost[] costs)
        : base("Reload", "Refills ammo.", ActionType.SKILL, costs, Targeting.FACING, 1, new Pattern())
    {
    }

    public override string Refusal(GameObject caster)
    {
        ResourceComponent ammo = caster.GetResource(ResourceType.AMMO);
        if (ammo is null) return $"{caster.Name} has nothing to reload.";
        if (ammo.Current >= ammo.Max) return FullMessage.Replace("{caster}", caster.Name);

        return null;
    }

    public override void Resolve(GameObject caster, Point target, ICombatContext context)
    {
        ResourceComponent ammo = caster.GetResource(ResourceType.AMMO);
        if (ammo is not null)
            ammo.Current = ammo.Max;
    }
}
