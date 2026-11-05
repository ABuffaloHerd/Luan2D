using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using Direction = LuanSC.Data.Direction;

namespace LuanSC.Data.Components.AI;

/// <summary>
/// Summons a random unit from its list into a free adjacent cell every so many of its own turns. Does nothing else.
/// There's no cap on units, it just waits when every adjacent cell is full. Its units vanish with it like any other summon.
/// </summary>
public class SpawnerBehavior : IAIBehavior
{
    private readonly IReadOnlyList<Func<GameObject, SummonedGameObject>> spawns;
    private readonly int interval;

    private int turnsUntilSpawn;

    public ActionType Cost { get; set; } = ActionType.SPELL;

    /// <summary>
    /// Fight feed line for each spawn, {caster} and {summon} get filled in. Null uses the default, "{caster} summoned {summon}."
    /// </summary>
    public string Message { get; set; }

    /// <param name="spawns">Picked from at random. List one twice to make it twice as likely.</param>
    /// <param name="interval">Its own turns between spawns. 3 spawns on turns 1, 4, 7.</param>
    /// <param name="initialDelay">Its own turns to wait before the first spawn. 0 spawns on its first turn.</param>
    public SpawnerBehavior(IReadOnlyList<Func<GameObject, SummonedGameObject>> spawns, int interval = 1, int initialDelay = 0)
    {
        if (spawns is null || spawns.Count == 0) throw new ArgumentException("Spawner needs at least one unit to spawn.");
        if (interval < 1) throw new ArgumentException($"Spawner interval must be at least 1, got {interval}.");
        if (initialDelay < 0) throw new ArgumentException($"Spawner delay can't be negative, got {initialDelay}.");

        this.spawns = spawns;
        this.interval = interval;
        turnsUntilSpawn = initialDelay;
    }

    public IReadOnlyList<TurnAction> PlanTurn(GameObject self, ICombatContext context)
    {
        if (turnsUntilSpawn > 0)
        {
            turnsUntilSpawn--;
            return [];
        }

        ActionComponent actions = self.GetComponent<ActionComponent>();
        if (actions is not null && !actions.CanSpend(Cost)) return [];

        foreach (Direction direction in Enum.GetValues<Direction>())
        {
            Point spot = self.Position + direction.ToVector();
            if (!context.IsWalkable(spot)) continue;

            turnsUntilSpawn = interval - 1;
            return [new TurnAction.Summon(spawns[Random.Shared.Next(spawns.Count)], spot, Cost, Message: Message)];
        }

        return []; // boxed in. Still due, so it tries again next turn instead of losing the spawn
    }
}
