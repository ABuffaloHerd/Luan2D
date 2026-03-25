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

    public void OnApply(GameObject owner)
    {
        // do nothing
    }

    public void OnIncomingDamage(GameObject owner, DamageRecord damage)
    {
        // do nothing
    }

    public void OnRemove(GameObject owner)
    {
        // do nothing
    }

    public void OnTurnEnd(GameObject owner)
    {
        // do nothing
    }

    public void OnTurnStart(GameObject owner)
    {
        // produce a new damage record for the poison damage and apply it to the owner
        DamageRecord damage = new()
        {
            Attacker = null, // no attacker for poison damage
            Target = owner,
            Amount = Damage,
            Type = DamageType.MAGIC
        };

        owner.GetComponent<HPComponent>().TakeDamage(damage);
    }
}
