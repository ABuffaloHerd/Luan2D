using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Effects;

[Flags]
public enum EffectTag
{
    None = 0,
    Offensive = 1 << 0,
    Defensive = 1 << 1,
    DoT = 1 << 2,   // damage over time
    HoT = 1 << 3,   // heal over time
    Buff = 1 << 4,
    Debuff = 1 << 5,
    Crowd = 1 << 6,   // crowd control, stun, slow etc
}