using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data
{
    public sealed record DamageRecord
    {
        public required GameObject Attacker { get; init; }
        public required GameObject Target { get; init; }
        public int Amount { get; init; }
        public DamageType Type { get; init; }
    }
}
