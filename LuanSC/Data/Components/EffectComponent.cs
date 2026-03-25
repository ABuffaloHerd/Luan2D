using LuanSC.Data.Components.Effects;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LuanSC.Data.Components;

public class EffectComponent : IComponent, IDamageDealtListener, IDamageListener
{
    public string Name => "Effect Component";
    public GameObject Owner { get; private set; }

    private List<IEffect> effects = new();

    public void Apply(IEffect effect)
    {
        effects.Add(effect);
        effect.SetOwner(Owner);
        effect.OnApply(Owner);
    }

    public void Remove(IEffect effect)
    {
        if (effects.Remove(effect))
        {
            effect.OnRemove(Owner);
        }
    }

    public void OnDamageDealt(DamageRecord damage) 
    {
        foreach (var effect in effects.OfType<IDamageDealtListener>())
        {
            effect.OnDamageDealt(damage);
        }
    }

    public void OnDamageTaken(DamageRecord damage)
    {
        foreach (var effect in effects.OfType<IDamageListener>())
        {
            effect.OnDamageTaken(damage);
        }
    }

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
    }

    public DamageRecord ProcessIncomingDamage(DamageRecord damage)
    {
        DamageRecord modifiedDamage = damage;
        foreach (var effect in effects)
        {
            modifiedDamage = effect.OnIncomingDamage(Owner, modifiedDamage);
        }
        return modifiedDamage;
    }

    /// <summary>
    /// Runs on turn start, ticks down durations, and removes expired effects. This should be called by the scene or some other external system that manages turns.
    /// </summary>
    public void Tick()
    {
        foreach(var effect in effects.ToList())
        {
#if DEBUG
            Debug.Print("Ticking effect: " + effect.Name + " with duration " + effect.Duration);
#endif
            effect.Duration--;

            if (effect.IsExpired)
            {
                Remove(effect);
            }
            else
            {
                effect.OnTurnStart(Owner);
            }
        }
    }

    public void TickEnd()
    {
        foreach (var effect in effects.ToList())
        {
            effect.OnTurnEnd(Owner);
        }
    }
}
