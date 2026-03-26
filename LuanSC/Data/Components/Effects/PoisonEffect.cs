using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Effects;

public class PoisonEffect : IEffect
{
    public string Name => "Poison";
    public int Duration { get; set; }
    public int Damage { get; set; }

    public GameObject Owner { get; private set; }

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
    }
    public PoisonEffect(int duration, int damage)
    {
        Duration = duration;
        Damage = damage;
    }

    public void OnApply()
    {
        // do nothing
    }

    public void OnIncomingDamage()
    {
        // do nothing
    }

    public void OnRemove()
    {
        // do nothing
    }

    public void OnTurnEnd()
    {
        // do nothing
    }

    public void OnTurnStart()
    {
        // produce a new damage record for the poison damage and apply it to the owner
        DamageRecord damage = new()
        {
            Attacker = null, // no attacker for poison damage
            Target = Owner,
            Amount = Damage,
            Type = DamageType.MAGIC
        };

        int taken = Owner.GetComponent<HPComponent>().TakeDamage(damage);
        GameEvents.CombatMessage($"{Owner.Name} takes {taken} poison damage.");
        GameEvents.CombatMessage($"{Duration} turns remaining.");
    }
}
