using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Effects;

/// <summary>
/// The owner takes Amount less damage per hit. True damage ignores it, like it ignores DEF and barriers.
/// </summary>
public class ArmourEffect(int duration, int amount) : DamageModifierEffect(duration)
{
    public override string Name => "Armour";
    public override EffectTag Tags => EffectTag.Defensive | EffectTag.Buff;
    public int Amount { get; set; } = amount;

    public override DamageRecord OnIncomingDamage(DamageRecord damage)
    {
        if (damage.Type == DamageType.TRUE) return damage;
        return Modify(damage, Math.Max(0, damage.Amount - Amount));
    }
}
