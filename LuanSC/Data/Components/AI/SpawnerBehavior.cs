using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using Direction = LuanSC.Data.Direction;

namespace LuanSC.Data.Components.AI;

/// <summary>
/// Summons random units from its list into free adjacent cells every so many of its own turns. Does nothing else.
/// There's no cap on units, it just waits when every adjacent cell is full. Its units vanish with it like any other summon.
/// </summary>
/// <remarks>
/// Each spawn costs one SPELL action (Cost), and the default action component only gives one a turn.
/// So a Count above 1 needs the SPELL limit raised to match, e.g. ActionLimits = { [ActionType.SPELL] = 3 } for Count = 3,
/// otherwise it only spawns as many as it has SPELL actions left. With no SPELL actions at all it never spawns.
/// </remarks>
public class SpawnerBehavior : IAIBehavior
{
    private readonly IReadOnlyList<Func<GameObject, SummonedGameObject>> spawns;
    private readonly int interval;
    private readonly int count;

    private int turnsUntilSpawn;

    /// <summary>
    /// What each spawn costs. One action of this type per unit spawned.
    /// </summary>
    public ActionType Cost { get; set; } = ActionType.SPELL;

    /// <summary>
    /// Fight feed line for each spawn, {caster} and {summon} get filled in. Null uses the default, "{caster} summoned {summon}."
    /// </summary>
    public string Message { get; set; }

    /// <param name="spawns">Picked from at random for each unit. List one twice to make it twice as likely.</param>
    /// <param name="interval">Its own turns between spawns. 3 spawns on turns 1, 4, 7.</param>
    /// <param name="initialDelay">Its own turns to wait before the first spawn. 0 spawns on its first turn.</param>
    /// <param name="count">Units per spawn, at most 4 (one per adjacent cell). Each costs one SPELL action, see the remarks.</param>
    public SpawnerBehavior(IReadOnlyList<Func<GameObject, SummonedGameObject>> spawns, int interval = 1, int initialDelay = 0, int count = 1)
    {
        if (spawns is null || spawns.Count == 0) throw new ArgumentException("Spawner needs at least one unit to spawn.");
        if (interval < 1) throw new ArgumentException($"Spawner interval must be at least 1, got {interval}.");
        if (initialDelay < 0) throw new ArgumentException($"Spawner delay can't be negative, got {initialDelay}.");
        if (count < 1) throw new ArgumentException($"Spawner count must be at least 1, got {count}.");

        this.spawns = spawns;
        this.interval = interval;
        this.count = count;
        turnsUntilSpawn = initialDelay;
    }

    public IReadOnlyList<TurnAction> PlanTurn(GameObject self, ICombatContext context)
    {
        if (turnsUntilSpawn > 0)
        {
            turnsUntilSpawn--;
            return [];
        }

        // one action per unit, so it can't plan more than it can pay for
        ActionComponent actions = self.GetComponent<ActionComponent>();
        int affordable = Math.Min(count, actions?.Remaining(Cost) ?? count);
        if (affordable <= 0) return [];

        List<TurnAction> plan = new();
        foreach (Direction direction in Enum.GetValues<Direction>())
        {
            if (plan.Count >= affordable) break;

            Point spot = self.Position + direction.ToVector();
            if (!context.IsWalkable(spot)) continue;

            plan.Add(new TurnAction.Summon(spawns[Random.Shared.Next(spawns.Count)], spot, Cost, Message: Message));
        }

        // boxed in. Still due, so it tries again next turn instead of losing the spawn
        if (plan.Count == 0) return [];

        // a partial spawn still counts as the spawn, the ones that didn't fit are lost
        turnsUntilSpawn = interval - 1;
        return plan;
    }
}
