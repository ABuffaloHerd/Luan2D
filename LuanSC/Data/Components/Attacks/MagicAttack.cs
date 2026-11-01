using LuanSC.Data.Animations;
using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Attacks;

/// <summary>
/// Magic bolt at the first target in range, like BasicAttack but always magic damage and with sparkles instead of a slash.
/// For staves, lenses and the like. A sword that does magic damage should use BasicAttack with Type = MAGIC instead.
/// </summary>
public class MagicAttack : IAbility
{
    public string Name => "Magic Attack";
    public string Description => "A bolt of magic that deals magic damage to a single target.";
    public int Damage { get; private set; }
    public DamageType Type => DamageType.MAGIC;

    /// <summary>
    /// Radius of the sparkle diamond. Only the target is hit, this is just for show.
    /// </summary>
    public int EffectRadius { get; init; } = 1;

    public MagicAttack(int damage)
    {
        Damage = damage;
    }

    public void Execute(GameObject owner, IReadOnlyList<GameObject> targets, ICombatContext context)
    {
        // attack first target in targets
        if (targets.Count == 0)
        {
            GameEvents.Publish(new CombatEvent.GenericMessage($"{owner.Name} fizzled magic at the air for {Damage} damage"));
            return;
        }

        GameObject target = targets[0];
        HPComponent healthy = target.GetComponent<HPComponent>();

        if (healthy is null)
        {
            GameEvents.Publish(new CombatEvent.GenericMessage($"{owner.Name} fizzled magic at the wall for {Damage} damage"));
            return;
        }

        // the owner's damage buffs are applied in TakeDamage
        DamageRecord pain = new()
        {
            Attacker = owner,
            Target = target,
            Amount = Damage,
            Type = Type,
            Source = Name
        };

        int dealt = healthy.TakeDamage(pain);

        // origin is the target's arena cell, so the sparkles are centred on whoever got hit
        context.PlayAnimation(AnimationRegistry.Sparkle(target.Position, EffectRadius));

        GameEvents.Publish(new CombatEvent.DamageDealt(owner, target, dealt, Type, pain.Element));
    }
}
