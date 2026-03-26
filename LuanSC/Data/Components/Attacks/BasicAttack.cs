using LuanSC.Objects;
using LuanSC.Objects.Weapons;
using LuanSC.Scenes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Attacks
{
    public class BasicAttack : IAttackComponent
    {
        public string Name => "Slashing Attack";

        public GameObject Owner { get; set; }

        public void Execute(GameObject owner, Weapon weapon, IReadOnlyList<GameObject> targets, CombatScene scene)
        {
            // attack first target in targets
            if (targets.Count == 0)
            {
                // if there are no targets, just publish a message (mocking the player) and return
                GameEvents.Publish(new CombatEvent.GenericMessage($"{Owner} tagged the air for {weapon.Damage} damage"));
                return;
            }

            GameObject target = targets[0];
            HPComponent healthy = target.GetComponent<HPComponent>();

            // if the target doesn't have a health component, just publish a message (mocking the player) and return
            if (healthy is null)
            {
                GameEvents.Publish(new CombatEvent.GenericMessage($"{Owner} tagged the wall for {weapon.Damage} damage"));
                return;
            }

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

            int dealt = healthy.TakeDamage(pain);

            // pUBLISH A DAMAGE DEALT EVENT
            var damageDealtEvent = new CombatEvent.DamageDealt
            (
                owner,
                target,
                dealt,
                DamageType.PHYSICAL
            );
        }

        public void SetOwner(GameObject owner)
        {
            Owner = owner;
        }
    }
}
