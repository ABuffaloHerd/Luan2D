using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Effects;

/// <summary>
/// Adds Amount to the owner's DEF while it lasts, so it works exactly like DEF:
/// only physical hits are reduced, and it's taken off before elemental weakness or resistance.
/// </summary>
public class ArmourEffect(int duration, int amount) : DamageModifierEffect(duration), IStatModifier
{
    public override string Name => "Armour";
    public override EffectTag Tags => EffectTag.Defensive | EffectTag.Buff;
    public int Amount { get; set; } = amount;

    int IStatModifier.DEF => Amount;
}
