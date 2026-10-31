using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Effects;

/// <summary>
/// What kind of effect something is, so weapons and spells can look for whole groups (e.g. cleanse every Debuff).
/// Combine them: Weaken is Offensive | Debuff. Query with EffectComponent.HasTag / WithTag.
/// </summary>
[Flags]
public enum EffectTag
{
    None = 0,
    Offensive = 1 << 0, // changes damage the owner deals
    Defensive = 1 << 1, // changes damage the owner takes
    DoT = 1 << 2,   // damage over time
    HoT = 1 << 3,   // heal over time
    Buff = 1 << 4,
    Debuff = 1 << 5,
    Crowd = 1 << 6,   // crowd control, stun, slow etc
}