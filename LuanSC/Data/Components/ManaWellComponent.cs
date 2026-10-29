using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components;

/// <summary>
/// Gives the summon's owner MP at the start of the owner's turn.
/// </summary>
public class ManaWellComponent : IComponent, IOwnerTurnStartListener
{
    public string Name => "Mana Well Component";
    public GameObject Owner { get; private set; }

    public int Amount { get; set; }

    public ManaWellComponent(int amount)
    {
        Amount = amount;
    }

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
    }

    public void OnOwnerTurnStart(GameObject owner)
    {
        MPComponent mp = owner.GetComponent<MPComponent>();
        if (mp is null) return; // can't fill a cup that doesn't exist

        int before = mp.Current;
        mp.Current += Amount; // clamped to max
        int restored = mp.Current - before;

        if (restored > 0)
            GameEvents.Publish(new CombatEvent.GenericMessage($"{Owner.Name} restored {restored} MP to {owner.Name}."));
    }
}
