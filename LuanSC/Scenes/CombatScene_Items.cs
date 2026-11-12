using LuanSC.Data.Combat;
using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Objects;

namespace LuanSC.Scenes;

/// <summary>
/// Using items, picking them up and putting them in the arena.
/// </summary>
public partial class CombatScene
{
    /// <summary>
    /// Runs the item's own action and takes one from the stack if it worked. The ITEM action is spent by Execute.
    /// </summary>
    /// <returns>false if nothing happened, so Execute doesn't spend the action.</returns>
    private bool UseItem(GameObject actor, TurnAction.UseItem use)
    {
        InventoryComponent bag = actor.GetComponent<InventoryComponent>();
        if (bag is null || bag.Count(use.Item) <= 0)
        {
            GameEvents.Publish(new CombatEvent.GenericMessage($"{actor.Name} doesn't have a {use.Item.Name}."));
            return false;
        }

        if (!Perform(actor, use.Inner)) return false;

        if (use.Item.Consumable)
            bag.Remove(use.Item);

        return true;
    }

    /// <summary>
    /// Picks up everything on the actor's cell that can be. Free, and does nothing for units without an inventory.
    /// </summary>
    private void PickUp(GameObject actor)
    {
        InventoryComponent bag = actor.GetComponent<InventoryComponent>();
        if (bag is null) return;

        List<ItemGameObject> found = gameObjects
            .OfType<ItemGameObject>()
            .Where(drop => drop.Position == actor.Position && drop.Item.CanPickUp)
            .ToList();

        foreach (ItemGameObject drop in found)
        {
            bag.Add(drop.Item, drop.Count);
            RemoveGameObject(drop);
            GameEvents.Publish(new CombatEvent.GenericMessage($"{actor.Name} picks up {drop.Name}."));
        }
    }

    public void Spawn(GameObject obj)
    {
        AddGameObject(obj);
    }
}
