using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data
{
    public record CombatRecord
    {
        public List<DamageRecord> DamageRecords { get; init; } = new();
        public Dictionary<string, object> Metadata { get; init; } = new();
    }
}
