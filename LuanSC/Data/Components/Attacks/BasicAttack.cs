using LuanSC.Objects;
using LuanSC.Objects.Weapons;
using LuanSC.Scenes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Attacks
{
    public class BasicAttack : IAbility
    {
        public string Name => "Slashing Attack";
        public string Description => "A basic slashing attack that deals physical damage to a single target.";
        public int Damage { get; private set; }

        public BasicAttack(int damage)
        {
            Damage = damage;
        }

        public void Execute(GameObject owner, IReadOnlyList<GameObject> targets)
        {
            // attack first target in targets
            if (targets.Count == 0)
            {
                // if there are no targets, just publish a message (mocking the player) and return
                GameEvents.Publish(new CombatEvent.GenericMessage($"{owner} tagged the air for {Damage} damage"));
                return;
            }

            GameObject target = targets[0];
            HPComponent healthy = target.GetComponent<HPComponent>();

            // if the target doesn't have a health component, just publish a message (mocking the player) and return
            if (healthy is null)
            {
                GameEvents.Publish(new CombatEvent.GenericMessage($"{owner} tagged the wall for {Damage} damage"));
                return;
            }

            // TODO: Owner's effects that boost damage go here
            int damage = Damage;

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
            GameEvents.Publish(damageDealtEvent);
        }
    }
}
