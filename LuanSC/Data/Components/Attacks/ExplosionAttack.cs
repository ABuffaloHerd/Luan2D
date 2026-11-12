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
/// Fires a shot at the nearest target in range and explodes where it lands. With nothing in range it explodes at the end of the range.
/// Hits everything with HP in the radius, the attacker and allies included. Walls and furniture block the blast for anything behind them.
/// Use Explode directly for explosions that don't come from a weapon, e.g. VolatileEffect.
/// </summary>
public class ExplosionAttack : IAbility
{
    public string Name => "Explosion";
    public string Description => $"Explodes for {Damage} dmg, radius {Radius}, falling off to {EdgePercent}% at the edge.";

    public int Damage { get; private set; }

    /// <summary>
    /// How far the blast reaches, in Pattern.CircleDistance.
    /// </summary>
    public int Radius { get; }

    /// <summary>
    /// Full damage out to here. Past it the damage falls off in a straight line to EdgePercent at Radius.
    /// </summary>
    public int Core { get; }

    /// <summary>
    /// Percent of Damage dealt at the very edge.
    /// </summary>
    public int EdgePercent { get; }

    // CONFIGURABLE: set any of these where the attack is created, e.g.
    //   new ExplosionAttack(damage, radius: 3) { Type = DamageType.MAGIC, Element = Element.Fire, Colour = Color.Cyan }

    public DamageType Type { get; init; } = DamageType.PHYSICAL;
    public Element Element { get; init; } = Element.None;

    /// <summary>
    /// Colour of the blast.
    /// </summary>
    public Color Colour { get; init; } = Color.OrangeRed;

    /// <summary>
    /// Colour of the shot on its way to the target.
    /// </summary>
    public Color ProjectileColor { get; init; } = Color.White;

    /// <param name="damage">Damage at the centre, before DEF and RES.</param>
    /// <param name="radius">How far it reaches.</param>
    /// <param name="core">Full damage out to here, 0 means only the centre cell.</param>
    /// <param name="edgePercent">Percent of the damage dealt at the radius.</param>
    public ExplosionAttack(int damage, int radius = 2, int core = 0, int edgePercent = 25)
    {
        if (radius < 0) throw new ArgumentException($"Explosion radius can't be negative, got {radius}.");
        if (core < 0 || core > radius) throw new ArgumentException($"Explosion core must be between 0 and the radius ({radius}), got {core}.");
        if (edgePercent < 0 || edgePercent > 100) throw new ArgumentException($"Explosion edge percent must be between 0 and 100, got {edgePercent}.");

        Damage = damage;
        Radius = radius;
        Core = core;
        EdgePercent = edgePercent;
    }

    public void Execute(GameObject owner, IReadOnlyList<GameObject> targets, ICombatContext context)
    {
        // the shot stops at whatever is closest, walls included, same as RangedAttack
        Point centre = targets.Count > 0
            ? targets.MinBy(t => Pattern.CircleDistance(t.Position - owner.Position)).Position
            : EndOfRange(owner);

        AnimatedEffect shot = AnimationRegistry.Projectile(owner.Position, centre, ProjectileColor);

        // the blast waits for the shot to land. Damage doesn't, same as RangedAttack
        AnimatedEffect blast = AnimationRegistry.CircleBlast(centre, Radius, Colour);
        shot.AnimationStateChanged += (_, e) =>
        {
            if (e.NewState == AnimatedEffect.AnimationState.Finished) context.PlayAnimation(blast);
        };

        context.PlayAnimation(shot);
        Explode(owner, centre, context, playAnimation: false);
    }

    /// <summary>
    /// Blows up at centre. The owner gets the credit, and their Empower, Weaken and so on change the damage.
    /// </summary>
    /// <param name="playAnimation">False if the caller plays the blast itself, e.g. after a projectile.</param>
    public void Explode(GameObject owner, Point centre, ICombatContext context, bool playAnimation = true)
    {
        if (playAnimation)
            context.PlayAnimation(AnimationRegistry.CircleBlast(centre, Radius, Colour));

        // copy first, damage listeners shouldn't be able to trip up the loop
        List<GameObject> objects = context.Objects.ToList();

        // worked out before anything is hit, so furniture broken by this blast still shields what's behind it
        HashSet<Point> blockers = LineOfSight.Blockers(objects);

        foreach (GameObject obj in objects)
        {
            HPComponent hp = obj.GetComponent<HPComponent>();
            if (hp is null || !hp.IsAlive) continue; // walls and corpses

            int distance = Pattern.CircleDistance(obj.Position - centre);
            if (distance > Radius) continue;
            if (!LineOfSight.HasLineOfSight(centre, obj.Position, blockers)) continue;

            DamageRecord pain = new()
            {
                Attacker = owner,
                Target = obj,
                Amount = DamageAt(distance),
                Type = Type,
                Element = Element,
                Source = Name
            };

            int dealt = hp.TakeDamage(pain);

            GameEvents.Publish(new CombatEvent.DamageDealt(owner, obj, dealt, Type, pain.Element));
        }
    }

    /// <summary>
    /// Damage before DEF and RES at this distance from the centre. Full out to Core, then a straight line down to EdgePercent at Radius.
    /// e.g. damage 40, radius 3, core 1, edge 25 gives 40, 40, 25, 10.
    /// </summary>
    public int DamageAt(int distance)
    {
        if (distance <= Core) return Damage;
        if (distance > Radius) return 0;

        double progress = (double)(distance - Core) / (Radius - Core); // Radius > Core here, or distance <= Core caught it
        double percent = 100 - (100 - EdgePercent) * progress;
        return (int)Math.Round(Damage * percent / 100, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// The cell of the weapon's range furthest from the owner, facing the way they face. The cell in front if they have no weapon.
    /// </summary>
    private static Point EndOfRange(GameObject owner)
    {
        Pattern range = owner.GetComponent<WeaponComponent>()?.Weapon?.Range;
        if (range is null) return owner.Position + owner.Direction.ToVector();

        ICollection<Point> cells = range.GetRotated(owner.Direction);
        if (cells.Count == 0) return owner.Position + owner.Direction.ToVector();

        return owner.Position + cells.MaxBy(Pattern.CircleDistance);
    }
}
