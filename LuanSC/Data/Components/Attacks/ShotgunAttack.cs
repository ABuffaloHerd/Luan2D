using LuanSC.Data.Animations;
using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using LuanSC.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using LuanSC.Data.Combat;

namespace LuanSC.Data.Components.Attacks;

/// <summary>
/// Whacks everything in range that the owner has line of sight to. Walls and furniture stop the pellets,
/// the blocker itself still gets hit.
/// </summary>
public class ShotgunAttack : IAbility
{
    public string Name => "Blast Attack";

    public string Description => "indiscriminately whacks everything it can ";

    public int Damage { get; private set; }
    public DamageType Type => DamageType.PHYSICAL;

    public ShotgunAttack(int damage)
    {
        Damage = damage;
    }

    /// <summary>
    /// Colour of the pellet sparkles.
    /// </summary>
    public Color Colour { get; init; } = Color.Yellow;

    public void Execute(GameObject owner, IReadOnlyList<GameObject> targets, ICombatContext context)
    {
        // worked out before anything is hit, so furniture broken by this shot still shields what's behind it
        HashSet<Point> blockers = LineOfSight.Blockers(context.Objects);

        // pellets fly even when there's nothing to hit, and stop at the same blockers the damage does
        IEnumerable<Point> cells = BlastCells(owner, targets)
            .Where(cell => LineOfSight.HasLineOfSight(owner.Position, owner.Position + cell, blockers));
        context.PlayAnimation(AnimationRegistry.PatternSparkle(owner.Position, cells, Colour));

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
            if (!LineOfSight.HasLineOfSight(owner.Position, target.Position, blockers)) continue; // behind cover

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
            hitAnything = true;

            GameEvents.Publish(new CombatEvent.DamageDealt(owner, target, dealt, Type, pain.Element));
        }

        // only walls in range, or everything was behind cover
        if (!hitAnything)
        {
            GameEvents.Publish(new CombatEvent.GenericMessage($"{owner} tagged the wall for {Damage} damage"));
        }
    }

    /// <summary>
    /// The cells that sparkle, as offsets from the owner: the weapon's range turned the way the owner faces.
    /// Without a weapon it's just the cells the targets stand on.
    /// </summary>
    private static IEnumerable<Point> BlastCells(GameObject owner, IReadOnlyList<GameObject> targets)
    {
        Pattern range = owner.GetComponent<WeaponComponent>()?.Weapon?.Range;
        if (range is not null) return range.GetRotated(owner.Direction);

        return targets.Select(t => t.Position - owner.Position);
    }
}
