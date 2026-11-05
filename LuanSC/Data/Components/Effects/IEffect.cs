using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Effects;

public interface IEffect : IComponent
{
    int Duration { get; set; } // duration in turns. -1 = permanant, 0 = expired

    bool IsExpired => Duration == 0;

    // what kind of effect this is, for EffectComponent.HasTag. Untagged effects can still be found by type with Has<T>.
    EffectTag Tags => EffectTag.None;

    // optional, e.g. Fire for a burn. For EffectComponent.HasElement.
    Element Element => Element.None;

    // if implemented, this will be called when the effect is applied to the owner. This allows the effect to hurt the owner, heal the owner, or do anything else it wants when applied.
    void OnApply();

    // if implemented, this will be called when the effect is removed from the owner. This allows the effect to hurt the owner really bad once more or heal the owner really good once more.
    void OnRemove();

    // if implemented, this will be called at the start of the owner's turn. To regenerate, hurt or whatever.
    void OnTurnStart();

    // what if the effect was delayed so you had to think in game? Hopefully this adds """depth""" to the game.
    void OnTurnEnd();

    // if implemented, this will be called when the owner deals damage, before the target's DEF/RES. This allows the effect to boost or weaken it.
    // Runs for every source of damage (attacks, spells) because HPComponent.TakeDamage calls it, so attacks don't need to.
    DamageRecord OnOutgoingDamage(DamageRecord damage) => damage;

    // if implemented, this will be called before damage is applied to the owner, after DEF/RES. This allows the effect to modify or negate the damage.
    // if not implemented, the damage will be applied as normal.
    DamageRecord OnIncomingDamage(DamageRecord damage) => damage;

    // if implemented, this will be called when the owner heals someone (themselves included), before the target's effects.
    // Runs for every source of healing because HPComponent.Heal calls it, so spells don't need to.
    HealingRecord OnOutgoingHealing(HealingRecord healing) => healing;

    // if implemented, this will be called before healing is applied to the owner, after the healer's effects.
    // This allows the effect to boost, reduce or block the healing, e.g. anti-heal.
    HealingRecord OnIncomingHealing(HealingRecord healthy) => healthy;
}
