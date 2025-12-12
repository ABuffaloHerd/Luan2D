using LuanSC.Objects;
using LuanSC.Objects.Weapons;
using LuanSC.Scenes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Attacks
{
    public class SlashingAttack : IAttackComponent
    {
        public string Name => "Slashing Attack";

        public GameObject Owner { get; set; }

        public CombatRecord Execute(GameObject owner, Weapon weapon, IReadOnlyList<GameObject> targets, CombatScene scene)
        {
            // attack first target in targets
            if (targets.Count == 0)
            {
                return new CombatRecord
                {
                    DamageRecords = new List<DamageRecord>(),
                    Metadata = new Dictionary<string, object>
                    {
                        { "Result", "No targets" }
                    }
                };
            }

            GameObject target = targets[0];
            HealthComponent healthy = target.GetComponent<HealthComponent>();

            if (healthy is null)
                return new CombatRecord
                {
                    DamageRecords = new List<DamageRecord>(),
                    Metadata = new Dictionary<string, object>
                    {
                        {"Result", "No damageable targets." }
                    }
                };

            // TODO: Owner's effects that boost damage go here
            int damage = weapon.Damage;

            // send a damage packet to the targeted health component
            DamageRecord pain = new()
            {
                Attacker = owner,
                Target = target,
                Amount = damage,
                Type = DamageType.PHYSICAL
            };

            List<DamageRecord> records = new();
            records.Add(pain);

            int dealt = healthy.TakeDamage(pain);

            // construct and return a combat record
            CombatRecord result = new CombatRecord
            {
                DamageRecords = records,
            };

            result.Metadata["attacker"] = owner.Name;
            result.Metadata["weapon"] = weapon.Name;
            result.Metadata["target"] = target.Name;

            return result;
        }

        public void SetOwner(GameObject owner)
        {
            Owner = owner;
        }
    }
}
