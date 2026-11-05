using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Effects;

/// <summary>
/// The owner takes Percent% more (weakness) or less (resistance, negative Percent) damage from hits with Element.
/// Runs with the other incoming effects, so after DEF/RES and the owner's own HPComponent.ElementModifiers.
/// True damage only takes weaknesses, see ElementRules.Applies. Duration -1 for a permanent trait.
/// </summary>
public class ElementalModifierEffect(int duration, Element element, int percent) : DamageModifierEffect(duration)
{
    public override string Name => Percent >= 0 ? $"{element} Weakness" : $"{element} Resistance";
    public override EffectTag Tags => EffectTag.Defensive | (Percent >= 0 ? EffectTag.Debuff : EffectTag.Buff);
    public Element Element { get; } = element;
    public int Percent { get; set; } = percent;

    public override DamageRecord OnIncomingDamage(DamageRecord damage)
    {
        if (!ElementRules.Applies(damage, Element, Percent)) return damage;
        return Modify(damage, Scale(damage.Amount, Percent));
    }
}
