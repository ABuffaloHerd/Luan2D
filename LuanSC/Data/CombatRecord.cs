using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data
{
    public record CombatRecord
    {
        public List<DamageRecord> DamageRecords { get; init; } = new();
        public Dictionary<string, object> Metadata { get; init; } = new();

        public int TotalDamage()
        {
            int total = 0;
            foreach (var record in DamageRecords)
            {
                total += record.Amount;
            }
            return total;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine("Combat Record:");
            foreach (var damage in DamageRecords)
            {
                sb.AppendLine($"- {damage}");
            }

            foreach (var kvp in Metadata)
            {
                sb.AppendLine($"Metadata - {kvp.Key}: {kvp.Value}");
            }

            return sb.ToString();
        }

        // default constructor for easy initialization
        public CombatRecord(string attacker)
        {
            Metadata["attacker"] = attacker;
            Metadata["result"] = false;
        }

        public CombatRecord() { }
    }
}
