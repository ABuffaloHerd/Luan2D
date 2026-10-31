using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Effects;

/// <summary>
/// The owner deals Percent% more damage.
/// </summary>
public class EmpowerEffect(int duration, int percent) : DamageModifierEffect(duration)
{
    public override string Name => "Empower";
    public override EffectTag Tags => EffectTag.Offensive | EffectTag.Buff;
    public int Percent { get; set; } = percent;

    public override DamageRecord OnOutgoingDamage(DamageRecord damage) => Modify(damage, Scale(damage.Amount, Percent));
}
