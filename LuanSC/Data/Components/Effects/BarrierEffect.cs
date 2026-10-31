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
    public EffectTag Tags => EffectTag.Defensive | EffectTag.Buff;

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

    /// <summary>
    /// Blocks every hit, true damage included. Each block costs a turn of duration, so a barrier under fire breaks early.
    /// </summary>
    public DamageRecord OnIncomingDamage(DamageRecord damage)
    {
        // already broken, it's only waiting for the next tick to be removed
        if (Duration == 0) return damage;

        // nothing got this far, don't wear the barrier down for it
        if (damage.Amount <= 0) return damage;

        GameEvents.Publish(new CombatEvent.DamageBlocked(Owner, "barrier", damage.Amount));

        if (Duration > 0) // -1 is permanent
        {
            Duration--;
            if (Duration == 0)
                GameEvents.Publish(new CombatEvent.GenericMessage($"{Owner.Name}'s barrier shatters."));
        }

        return damage with { Amount = 0 };
    }

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
    }
}
