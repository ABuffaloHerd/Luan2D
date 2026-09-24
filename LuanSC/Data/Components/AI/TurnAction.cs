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
}
