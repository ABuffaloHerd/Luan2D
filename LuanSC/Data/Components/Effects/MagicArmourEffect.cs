using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Effects;

/// <summary>
/// Adds Amount to the owner's RES while it lasts, so it works exactly like RES:
/// only magic hits are reduced, by Amount percent, before elemental weakness or resistance.
/// </summary>
public class MagicArmourEffect(int duration, int amount) : DamageModifierEffect(duration), IStatModifier
{
    public override string Name => "Magic Armour";
    public override EffectTag Tags => EffectTag.Defensive | EffectTag.Buff;
    public int Amount { get; set; } = amount;

    int IStatModifier.RES => Amount;
}
