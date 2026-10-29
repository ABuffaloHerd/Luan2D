using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Effects;

/// <summary>
/// The owner deals Percent% less damage.
/// </summary>
public class WeakenEffect(int duration, int percent) : DamageModifierEffect(duration)
{
    public override string Name => "Weaken";
    public int Percent { get; set; } = percent;

    public override DamageRecord OnOutgoingDamage(DamageRecord damage) => Modify(damage, Scale(damage.Amount, -Percent));
}
