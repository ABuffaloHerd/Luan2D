using LuanSC.Data.Components.Effects;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LuanSC.Data.Components;

public class EffectComponent : IComponent, IDamageDealtListener, IDamageListener, IHealingListener, IHealingGivenListener
{
    public string Name => "Effect Component";
    public GameObject Owner { get; private set; }

    private List<IEffect> effects = new();

    public void Apply(IEffect effect)
    {
        effects.Add(effect);
        effect.SetOwner(Owner);
        effect.OnApply();
    }

    public void Remove(IEffect effect)
    {
        if (effects.Remove(effect))
        {
            effect.OnRemove();
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

    public void OnHealingReceived(HealingRecord healing)
    {
        foreach (var effect in effects.OfType<IHealingListener>())
        {
            effect.OnHealingReceived(healing);
        }
    }

    public void OnHealingGiven(HealingRecord healing)
    {
        foreach (var effect in effects.OfType<IHealingGivenListener>())
        {
            effect.OnHealingGiven(healing);
        }
    }

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
    }

    public DamageRecord ProcessIncomingDamage(DamageRecord damage, Action<string> report = null)
    {
        DamageRecord modifiedDamage = damage;
        foreach (var effect in effects)
        {
            modifiedDamage = effect.OnIncomingDamage(modifiedDamage);
        }
        return modifiedDamage;
    }

    public HealingRecord ProcessIncomingHealing(HealingRecord healthy)
    {
        HealingRecord modifiedHealing = healthy;
        foreach (var effect in effects)
        {
            modifiedHealing = effect.OnIncomingHealing(modifiedHealing);
        }
        return modifiedHealing;
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

            if (effect.IsExpired)
            {
                GameEvents.CombatMessage($"{effect.Name} has expired.");
                Remove(effect);
            }
            else
            {
                effect.OnTurnStart();
            }
        }
    }

    public void TickEnd(Action<string> report = null)
    {
        foreach (var effect in effects.ToList())
        {
            effect.OnTurnEnd();
            effect.Duration--;
        }
    }
}
