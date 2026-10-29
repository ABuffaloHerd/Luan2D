using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using Direction = LuanSC.Data.Direction;

namespace LuanSC.Data.Components.AI;

/// <summary>
/// Summons one minion per turn into a free adjacent cell until it has MaxMinions. Does nothing else.
/// </summary>
public class SummonerBehavior : IAIBehavior
{
    public Func<GameObject, SummonedGameObject> Minion { get; set; }
    public int MaxMinions { get; set; } = 2;
    public ActionType Cost { get; set; } = ActionType.SPELL;

    public SummonerBehavior(Func<GameObject, SummonedGameObject> minion)
    {
        Minion = minion;
    }

    public IReadOnlyList<TurnAction> PlanTurn(GameObject self, ICombatContext context)
    {
        List<TurnAction> plan = new();

        int minions = context.Objects.OfType<SummonedGameObject>().Count(s => s.Owner == self);
        if (minions >= MaxMinions) return plan;

        ActionComponent actions = self.GetComponent<ActionComponent>();
        if (actions is not null && !actions.CanSpend(Cost)) return plan;

        foreach (Direction direction in Enum.GetValues<Direction>())
        {
            Point spot = self.Position + direction.ToVector();
            if (context.IsWalkable(spot))
            {
                plan.Add(new TurnAction.Summon(Minion, spot, Cost));
                break;
            }
        }

        return plan;
    }
}
