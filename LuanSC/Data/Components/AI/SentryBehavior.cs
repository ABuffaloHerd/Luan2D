using LuanSC.Objects;
using LuanSC.Objects.Weapons;
using System;
using System.Collections.Generic;
using System.Text;
using Direction = LuanSC.Data.Direction;

namespace LuanSC.Data.Components.AI;

/// <summary>
/// Never moves. Turns to face the first living hostile in weapon range and shoots it.
/// </summary>
public class SentryBehavior : IAIBehavior
{
    public IReadOnlyList<TurnAction> PlanTurn(GameObject self, ICombatContext context)
    {
        List<TurnAction> plan = new();

        Weapon weapon = self.GetComponent<WeaponComponent>()?.Weapon;
        if (weapon is null) return plan;

        ActionComponent actions = self.GetComponent<ActionComponent>();
        if (actions is not null && !actions.CanSpend(ActionType.ATTACK)) return plan;

        foreach (Direction direction in Enum.GetValues<Direction>())
        {
            foreach (Point cell in weapon.Range.GetRotated(direction))
            {
                Point spot = self.Position + cell;

                bool hostileThere = context.Objects.Any(obj =>
                    obj.Position == spot
                    && TeamComponent.AreHostile(self, obj)
                    && obj.GetComponent<HPComponent>() is { IsAlive: true });

                if (hostileThere)
                {
                    plan.Add(new TurnAction.Face(direction));
                    plan.Add(new TurnAction.Attack());
                    return plan;
                }
            }
        }

        return plan;
    }
}
