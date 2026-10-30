using LuanSC.Data;
using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes;

public partial class CombatScene
{
    private bool Summon(GameObject owner, TurnAction.Summon summon)
    {
        if (!IsWalkable(summon.Position))
        {
            GameEvents.Publish(new CombatEvent.GenericMessage($"There's no room for {owner.Name}'s summon there."));
            return false;
        }

        if (!TryPayCosts(owner, summon.Costs)) return false;

        SummonedGameObject minion = summon.Create(owner);
        minion.Position = summon.Position;
        AddGameObject(minion);

        string message = summon.Message?.Replace("{caster}", owner.Name).Replace("{summon}", minion.Name);
        GameEvents.Publish(new CombatEvent.Summoned(owner, minion, message));
        return true;
    }

    /// <summary>
    /// Lets the owner's summons do their thing at the start of the owner's turn.
    /// </summary>
    private void NotifyOwnerTurnStart(GameObject owner)
    {
        List<SummonedGameObject> summons = gameObjects
            .OfType<SummonedGameObject>()
            .Where(s => s.Owner == owner)
            .ToList();

        foreach (SummonedGameObject summon in summons)
        {
            foreach (IOwnerTurnStartListener listener in summon.Components.OfType<IOwnerTurnStartListener>())
            {
                listener.OnOwnerTurnStart(owner);
            }
        }
    }

    /// <summary>
    /// Adds an object mid fight. If it has speed it slots into the rest of this round.
    /// </summary>
    private void AddGameObject(GameObject obj)
    {
        gameObjects.Add(obj);
        entityManager.Add(obj);

        if (obj.GetComponent<SpeedComponent>() is null) return;

        // stable sort, so on a speed tie the newcomer goes last
        queue = new Queue<GameObject>(queue
            .Append(obj)
            .OrderByDescending(e => e.GetComponent<SpeedComponent>().Speed));

        RefreshTurnOrder();
    }

    /// <summary>
    /// Removes an object and everything it summoned, and their summons too.
    /// </summary>
    private void RemoveGameObject(GameObject obj)
    {
        if (!gameObjects.Remove(obj)) return;

        entityManager.Remove(obj);
        queue = new Queue<GameObject>(queue.Where(e => e != obj));

        // summons go down with their owner
        List<SummonedGameObject> summons = gameObjects
            .OfType<SummonedGameObject>()
            .Where(s => s.Owner == obj)
            .ToList();

        foreach (SummonedGameObject summon in summons)
        {
            GameEvents.Publish(new CombatEvent.GenericMessage($"{summon.Name} vanishes with {obj.Name}."));
            RemoveGameObject(summon);
        }

        RefreshTurnOrder();
    }
}
