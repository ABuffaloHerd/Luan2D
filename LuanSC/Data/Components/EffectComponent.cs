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

    // QUERIES. Expired effects are skipped, they're only waiting for the next tick to be removed.

    /// <summary>
    /// Every active effect, oldest first.
    /// </summary>
    public IEnumerable<IEffect> Effects => effects.Where(e => !e.IsExpired);

    /// <summary>
    /// The oldest active effect of type T, or null. Works for any effect class, including ones private to a weapon.
    /// </summary>
    public T Get<T>() where T : IEffect => Effects.OfType<T>().FirstOrDefault();

    /// <summary>
    /// Every active effect of type T, for stacks like two Poisons.
    /// </summary>
    public IEnumerable<T> GetAll<T>() where T : IEffect => Effects.OfType<T>();

    public bool Has<T>() where T : IEffect => Effects.OfType<T>().Any();

    /// <summary>
    /// Active effects with all of the given tags, e.g. WithTag(EffectTag.Debuff | EffectTag.DoT).
    /// </summary>
    public IEnumerable<IEffect> WithTag(EffectTag tags) => Effects.Where(e => tags != EffectTag.None && e.Tags.HasFlag(tags));

    public bool HasTag(EffectTag tags) => WithTag(tags).Any();

    /// <summary>
    /// Active effects with all of the given elements, same rules as WithTag.
    /// </summary>
    public IEnumerable<IEffect> WithElement(Element element) => Effects.Where(e => element != Element.None && e.Element.HasFlag(element));

    public bool HasElement(Element element) => WithElement(element).Any();

    public void Remove(IEffect effect)
    {
        if (effects.Remove(effect))
        {
            effect.OnRemove();
        }
    }

    /// <summary>
    /// Removes every effect. Each one still gets its OnRemove.
    /// </summary>
    public void Clear()
    {
        foreach (var effect in effects.ToList())
        {
            Remove(effect);
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

    public DamageRecord ProcessOutgoingDamage(DamageRecord damage)
    {
        DamageRecord modifiedDamage = damage;
        foreach (var effect in effects)
        {
            modifiedDamage = effect.OnOutgoingDamage(modifiedDamage);
        }
        return modifiedDamage;
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

    public HealingRecord ProcessOutgoingHealing(HealingRecord healing)
    {
        HealingRecord modifiedHealing = healing;
        foreach (var effect in effects)
        {
            modifiedHealing = effect.OnOutgoingHealing(modifiedHealing);
        }
        return modifiedHealing;
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

            // only count down live timed effects. One already at 0 (a broken barrier, a finished building) would hit -1,
            // which means permanent. Permanent ones stay negative either way.
            if (effect.Duration > 0)
                effect.Duration--;
        }
    }
}
