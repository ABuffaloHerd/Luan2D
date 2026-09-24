using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.AI;

using LuanSC.Objects;

public interface ICombatContext
{
    IReadOnlyList<GameObject> Objects { get; }
    bool IsWalkable(Point position);
}
