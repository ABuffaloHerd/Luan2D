using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Effects;

/// <summary>
/// The owner takes Percent% more damage, true damage included.
/// </summary>
public class VulnerableEffect(int duration, int percent) : DamageModifierEffect(duration)
{
    public override string Name => "Vulnerable";
    public int Percent { get; set; } = percent;

    public override DamageRecord OnIncomingDamage(DamageRecord damage) => Modify(damage, Scale(damage.Amount, Percent));
}
