using LuanSC.Data.Animations;
using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Spells;

/// <summary>
/// Flat magic damage to everything with HP inside a circle around the target. RES still reduces it.
/// </summary>
public class Fireball : CastSpell
{
    public int Damage { get; }
    public int Radius { get; }

    /// <summary>
    /// When true it hits everyone in the blast, caster and allies included. When false only hostiles.
    /// </summary>
    public bool FriendlyFire { get; init; } = true;

    public Fireball(int damage, int radius, int range, params ResourceCost[] costs)
        : base("Fireball", $"{damage} magic dmg, radius {radius}.", ActionType.SPELL, costs, Targeting.CURSOR, range, Pattern.Circle(radius))
    {
        Damage = damage;
        Radius = radius;
    }

    public override void Resolve(GameObject caster, Point target, ICombatContext context)
    {
        context.PlayAnimation(AnimationRegistry.CircleBlast(target, Radius, Color.OrangeRed));

        ICollection<Point> blast = AreaAt(caster, target);

        // copy first, damage listeners shouldn't be able to trip up the loop
        foreach (GameObject obj in context.Objects.ToList())
        {
            if (!blast.Contains(obj.Position)) continue;

            HPComponent hp = obj.GetComponent<HPComponent>();
            if (hp is null || !hp.IsAlive) continue; // walls and corpses

            if (!FriendlyFire && !TeamComponent.AreHostile(caster, obj)) continue;

            DamageRecord pain = new()
            {
                Attacker = caster,
                Target = obj,
                Amount = Damage,
                Type = DamageType.MAGIC
            };

            int dealt = hp.TakeDamage(pain);

            GameEvents.Publish(new CombatEvent.DamageDealt(caster, obj, dealt, DamageType.MAGIC));
        }
    }
}
