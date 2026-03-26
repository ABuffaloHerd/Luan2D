using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Effects;

public class BarrierEffect : IEffect
{
    public int Duration { get; set; }

    public string Name => "Barrier";

    public GameObject Owner { get; private set; }

    public void OnApply() // bonus: heal 10
    {
        HealingRecord healthy = new()
        { Amount = 10, Healer = Owner, Target = Owner };
        Owner.GetComponent<HPComponent>().Heal(healthy);
    }

    public void OnRemove()
    {
        
    }

    public void OnTurnEnd()
    {
        
    }

    public void OnTurnStart()
    {
        
    }

    public DamageRecord OnIncomingDamage(DamageRecord damage)
    {
        // if it ain't true damage it ain't getting past
        if (damage.Type != DamageType.TRUE)
        {
            GameEvents.CombatMessage($"Barrier blocked an instance of damage!");
            return damage with { Amount = 0 };
        }
        else
        {
            GameEvents.CombatMessage($"{Owner}'s barrier couldn't block an instance of true damage");
        }

        return damage;
    }

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
    }
}
