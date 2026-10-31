using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Attacks;

/// <summary>
/// Whacks everything in range
/// </summary>
public class BlastAttack : IAbility
{
    public string Name => "Blast Attack";

    public string Description => "indiscriminately whacks everything it can ";

    public int Damage { get; private set; }

    public BlastAttack(int damage)
    {
        Damage = damage;
    }

    public void Execute(GameObject owner, IReadOnlyList<GameObject> targets)
    {
        if (targets.Count == 0)
        {
            // laugh
            GameEvents.Publish(new CombatEvent.GenericMessage($"Where is {owner.Name} aiming?"));
            return;
        }

        bool hitAnything = false;

        // copy first, damage listeners shouldn't be able to trip up the loop
        foreach (GameObject target in targets.ToList())
        {
            HPComponent healthy = target.GetComponent<HPComponent>();
            if (healthy is null || !healthy.IsAlive) continue; // walls and corpses

            // the owner's damage buffs are applied in TakeDamage
            DamageRecord pain = new()
            {
                Attacker = owner,
                Target = target,
                Amount = Damage,
                Type = DamageType.PHYSICAL,
                Source = Name
            };

            int dealt = healthy.TakeDamage(pain);
            hitAnything = true;

            GameEvents.Publish(new CombatEvent.DamageDealt(owner, target, dealt, DamageType.PHYSICAL));
        }

        // only walls in range
        if (!hitAnything)
        {
            GameEvents.Publish(new CombatEvent.GenericMessage($"{owner} tagged the wall for {Damage} damage"));
        }
    }
}
