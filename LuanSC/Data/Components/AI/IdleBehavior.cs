using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.AI;

internal class IdleBehavior : IAIBehavior
{
    public IReadOnlyList<TurnAction> PlanTurn(GameObject self, ICombatContext context)
    {
        return new List<TurnAction>(); // Return an empty list of actions, indicating that the AI will do nothing during its turn.
    }
}
