using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Spells;

/// <summary>
/// Puts a summon from SummonRegistry on the target cell.
/// </summary>
public class SummonSpell : ISpell
{
    public string Name { get; }
    public string Description { get; }
    public ActionType Type { get; }
    public IReadOnlyList<ResourceCost> Costs { get; }

    public Targeting Targeting { get; init; } = Targeting.FACING;
    public int Range { get; init; } = 1;
    public Pattern Area { get; init; } = new Pattern().Mark(0, 0); // summons take one cell
    public string CastMessage { get; init; }

    public Func<GameObject, SummonedGameObject> Create { get; }

    public SummonSpell(string name, string description, ActionType type, Func<GameObject, SummonedGameObject> create, params ResourceCost[] costs)
    {
        Name = name;
        Description = description;
        Type = type;
        Create = create;
        Costs = costs;
    }

    public TurnAction CreateAction(GameObject caster, Point target)
        => new TurnAction.Summon(Create, target, Type, Costs, CastMessage?.Replace("{spell}", Name)); // the action doesn't know the spell
}
