using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Effects;

/// <summary>
/// Base for effects that only change damage numbers: Empower, Weaken, Armour, Vulnerable.
/// Override OnOutgoingDamage for damage the owner deals, OnIncomingDamage for damage the owner takes.
/// </summary>
public abstract class DamageModifierEffect : IEffect
{
    public abstract string Name { get; }
    public int Duration { get; set; }
    public GameObject Owner { get; private set; }

    protected DamageModifierEffect(int duration)
    {
        Duration = duration;
    }

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
    }

    public void OnApply()
    {
        GameEvents.Publish(new CombatEvent.GenericMessage($"{Owner.Name} gains {Name}."));
    }

    public void OnRemove()
    {
        GameEvents.Publish(new CombatEvent.EffectExpired(Owner, Name));
    }

    public void OnTurnStart() { }
    public void OnTurnEnd() { }

    public virtual DamageRecord OnOutgoingDamage(DamageRecord damage) => damage;
    public virtual DamageRecord OnIncomingDamage(DamageRecord damage) => damage;

    /// <summary>
    /// amount scaled by percent, e.g. 50 is +50%, -20 is -20%. The cast drops the fraction, same as RES. Never below 0.
    /// </summary>
    protected static int Scale(int amount, int percent)
    {
        return Math.Max(0, (int)(amount * (1 + percent / 100f)));
    }

    /// <summary>
    /// Changes the damage and says so in the fight feed, if it actually changed.
    /// </summary>
    protected DamageRecord Modify(DamageRecord damage, int newAmount)
    {
        if (newAmount == damage.Amount) return damage;

        int difference = newAmount - damage.Amount;
        string change = difference > 0 ? $"+{difference}" : difference.ToString();
        GameEvents.Publish(new CombatEvent.GenericMessage($"{Owner.Name}'s {Name}: {change} damage."));

        return damage with { Amount = newAmount };
    }
}
