using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        GameEvents.Publish(new CombatEvent.GenericMessage($"{Owner} received a barrier"));
    }

    public void OnRemove()
    {
        GameEvents.Publish(new CombatEvent.EffectExpired(Owner, Name));
    }

    public void OnTurnEnd()
    {
        
    }

    public void OnTurnStart()
    {
        // the shit we do for proper grammar
        GameEvents.Publish(new CombatEvent.GenericMessage($"The barrier protects {Owner} for {Duration} more turn{((Duration > 1) ? 's' : '\0')}."));
    }

    public DamageRecord OnIncomingDamage(DamageRecord damage)
    {
        // if it ain't true damage it ain't getting past
        if (damage.Type != DamageType.TRUE)
        {
            GameEvents.Publish(new CombatEvent.DamageBlocked(Owner, "barrier", damage.Amount));
            return damage with { Amount = 0 };
        }
        else
        {
            GameEvents.Publish(new CombatEvent.GenericMessage("The barrier fails to prevent true damage."));
        }

        return damage;
    }

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
    }
}
