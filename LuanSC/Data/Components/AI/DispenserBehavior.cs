using LuanSC.Data.Spells;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.AI;

/// <summary>
/// Never moves. Casts Dispense if anyone next to it can use it, otherwise stays quiet so the fight feed does too.
/// </summary>
public class DispenserBehavior(Dispense dispense) : IAIBehavior
{
    public IReadOnlyList<TurnAction> PlanTurn(GameObject self, ICombatContext context)
    {
        if (!dispense.Recipients(self, context).Any()) return [];

        return [dispense.CreateAction(self, self.Position)];
    }
}
