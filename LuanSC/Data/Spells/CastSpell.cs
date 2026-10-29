using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Spells;

/// <summary>
/// Base for spells that do their own thing when cast: damage, healing, effects.
/// (Summons go through SummonSpell and TurnAction.Summon instead, since the scene has to add the new object.)
///
/// TEMPLATE FOR NEW SPELLS:
///   1. Subclass this, pass name/description/type/costs/targeting/range/area to the base constructor.
///   2. Make Area match what Resolve actually hits. The targeting highlight draws Area, so it should never lie.
///   3. Implement Resolve. By the time it's called the scene has already checked the action counter, the range,
///      and taken the costs. Just apply the effect.
///   4. Add a factory to SpellRegistry and put it in someone's SpellbookComponent.
/// See Fireball.
/// </summary>
public abstract class CastSpell : ISpell
{
    public string Name { get; }
    public string Description { get; }
    public ActionType Type { get; }
    public IReadOnlyList<ResourceCost> Costs { get; }
    public Targeting Targeting { get; }
    public int Range { get; }
    public Pattern Area { get; }

    protected CastSpell(string name, string description, ActionType type, IReadOnlyList<ResourceCost> costs, Targeting targeting, int range, Pattern area)
    {
        Name = name;
        Description = description;
        Type = type;
        Costs = costs;
        Targeting = targeting;
        Range = range;
        Area = area;
    }

    public TurnAction CreateAction(GameObject caster, Point target) => new TurnAction.Cast(this, target);

    /// <summary>
    /// Applies the spell. Costs are already paid and the target is already in range.
    /// </summary>
    /// <param name="target">The arena cell that was picked (or the facing cell for FACING spells).</param>
    public abstract void Resolve(GameObject caster, Point target, ICombatContext context);

    /// <summary>
    /// Arena cells covered by Area when cast at target, rotated to the caster like the targeting highlight.
    /// </summary>
    protected ICollection<Point> AreaAt(GameObject caster, Point target)
    {
        HashSet<Point> cells = new();
        foreach (Point offset in Area.GetRotated(caster.Direction))
            cells.Add(target + offset);

        return cells;
    }
}
