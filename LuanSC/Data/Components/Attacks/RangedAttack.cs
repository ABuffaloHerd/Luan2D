using LuanSC.Data.Animations;
using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Attacks;

/// <summary>
/// Fires a projectile at the nearest target in range. Single target like BasicAttack, but anything closer blocks the shot,
/// walls included. The hit lands straight away, the projectile is just for show.
/// Preconfigured to use the projectile animation
/// </summary>
public class RangedAttack : IAbility
{
    public string Name => "Ranged Attack";
    public string Description => "Fires a projectile that hits the nearest target in range.";
    public int Damage { get; private set; }

    // CONFIGURABLE: set any of these where the attack is created, e.g.
    //   new RangedAttack(damage) { Type = DamageType.MAGIC, ProjectileColor = Color.Cyan, ProjectileFrameTime = TimeSpan.FromMilliseconds(15) }

    /// <summary>
    /// Damage type of the hit.
    /// </summary>
    public DamageType Type { get; init; } = DamageType.PHYSICAL;

    /// <summary>
    /// Colour of the projectile.
    /// </summary>
    public Color ProjectileColor { get; init; } = Color.White;

    /// <summary>
    /// How long the projectile is held on each cell. Lower is faster.
    /// </summary>
    public TimeSpan ProjectileFrameTime { get; init; } = TimeSpan.FromMilliseconds(30);

    private Random rand;

    public RangedAttack(int damage)
    {
        rand = new Random();
        Damage = damage;
    }

    public void Execute(GameObject owner, IReadOnlyList<GameObject> targets, ICombatContext context)
    {
        if (targets.Count == 0)
        {
            List<CombatEvent.GenericMessage> messages = new()
            {
                new CombatEvent.GenericMessage($"{owner.Name} tagged the air for {Damage} damage."),
                new CombatEvent.GenericMessage($"{owner.Name} missed."),
                new CombatEvent.GenericMessage($"Bro where you aiming at??"),
                new CombatEvent.GenericMessage($"{owner.Name} wasted precious ammunition."),
                new CombatEvent.GenericMessage($"Holding your weapon sideways will not help you."),
            };
            GameEvents.Publish(messages[rand.Next(messages.Count)]);
            return;
        }

        // targets come in no particular order, and a shot shouldn't pass through what's in front
        GameObject target = targets.MinBy(t => DistanceSquared(owner.Position, t.Position));

        // starts at the shooter's arena cell and lands on the target's
        context.PlayAnimation(AnimationRegistry.Projectile(owner.Position, target.Position, ProjectileColor, ProjectileFrameTime));

        HPComponent healthy = target.GetComponent<HPComponent>();
        if (healthy is null)
        {
            GameEvents.Publish(new CombatEvent.GenericMessage($"{owner.Name} shot the wall for {Damage} damage"));
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

        GameEvents.Publish(new CombatEvent.DamageDealt(owner, target, dealt, Type, pain.Element));
    }

    private static int DistanceSquared(Point a, Point b)
    {
        int dx = a.X - b.X;
        int dy = a.Y - b.Y;
        return dx * dx + dy * dy;
    }
}
