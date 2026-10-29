using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using Direction = LuanSC.Data.Direction;

namespace LuanSC.Data.Components.AI;

public abstract record TurnAction
{
    public sealed record Move(Direction Direction) : TurnAction;
    public sealed record Face(Direction Direction) : TurnAction;
    public sealed record Attack : TurnAction;
    public sealed record EndTurn : TurnAction;

    /// <summary>
    /// Creates a summon at an absolute arena position.
    /// </summary>
    /// <param name="Create">Builds the summon from its owner. See SummonRegistry.</param>
    /// <param name="Position">Where it appears. Must be walkable.</param>
    /// <param name="Cost">Which action counter it uses, e.g. SKILL for buildings or SPELL for magic.</param>
    /// <param name="Costs">Resources taken from the owner if the summon succeeds, e.g. MP or metal. null is free.</param>
    public sealed record Summon(Func<GameObject, SummonedGameObject> Create, Point Position, ActionType Cost, IReadOnlyList<ResourceCost> Costs = null) : TurnAction;
}
