using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Spells;

public enum Targeting
{
    FACING, // the cell in front of the caster, casts straight away
    CURSOR  // pick a cell within Range with the cursor
}

/// <summary>
/// Anything castable from the spellbook. Spells and skills both, Type decides which tab.
/// </summary>
public interface ISpell
{
    string Name { get; }
    string Description { get; }

    /// <summary>
    /// SPELL or SKILL. Picks the spellbook tab and which action counter it uses.
    /// </summary>
    ActionType Type { get; }
    IReadOnlyList<ResourceCost> Costs { get; }

    Targeting Targeting { get; }

    /// <summary>
    /// Max distance (manhattan) from the caster to the target. CURSOR only.
    /// </summary>
    int Range { get; }

    /// <summary>
    /// Cells affected around the target, (0, 0) being the target itself. Defined facing up and rotated to the caster.
    /// Used for the AoE highlight.
    /// </summary>
    Pattern Area { get; }

    /// <summary>
    /// Fight feed line when it's cast. {caster} and {spell} get filled in, summons also get {summon}.
    /// {caster} is the name from before the cast, so transforms read right.
    /// Null uses the default: "{caster} casts {spell}!", or "{caster} summoned {summon}." for summons.
    /// </summary>
    string CastMessage { get; }

    TurnAction CreateAction(GameObject caster, Point target);

    // checks resources and action counters, same idea as IAbility.CanUse
    bool CanCast(GameObject caster)
    {
        ActionComponent actions = caster.GetComponent<ActionComponent>();
        if (actions is not null && !actions.CanSpend(Type)) return false;

        foreach (ResourceCost cost in Costs)
        {
            ResourceComponent resource = caster.GetResource(cost.Type);
            if (resource is null || !resource.CanSpend(cost.Amount)) return false;
        }

        return true;
    }
}
