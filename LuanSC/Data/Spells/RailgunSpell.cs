using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Spells;
// TODO: finish.

/// <summary>
/// Minako's overdrive. Shoot a projectile and hit every target in a straight line. Penetrate walls.
/// Also TODO: accompanying animation
/// </summary>
public class RailgunSpell : CastableSpell
{
    public int Damage { get; }

    public RailgunSpell(int damage = 100) : base(
        name: "only mu railgun",
        description: "Shoot a projectile that hits every target in a straight line. Penetrates walls.",
        type: ActionType.SPECIAL,
        costs: new ResourceCost[] { new(ResourceType.OD, 100) },
        targeting: Targeting.FACING,
        range: 10,
        area: new Pattern().Line(40, Direction.UP)
        )
    {
        Damage = damage;
    }
    public override void Resolve(GameObject caster, Point target, ICombatContext context)
    {
        // the line is measured from the caster. target is the cell in front, and Pattern.Line leaves out (0, 0), so starting there would miss whoever is adjacent
        ICollection<Point> line = AreaAt(caster, caster.Position);

        // copy first, damage listeners shouldn't be able to trip up the loop
        foreach (GameObject obj in context.Objects.ToList())
        {
            if (!line.Contains(obj.Position)) continue;

            HPComponent hp = obj.GetComponent<HPComponent>();
            if (hp is null || !hp.IsAlive) continue; // walls don't stop the shot, they just don't take damage

            DamageRecord pain = new()
            {
                Attacker = caster,
                Target = obj,
                Amount = Damage,
                Type = DamageType.PHYSICAL,
                Source = Name
            };

            int dealt = hp.TakeDamage(pain);

            GameEvents.Publish(new CombatEvent.DamageDealt(caster, obj, dealt, DamageType.PHYSICAL, pain.Element));
        }
    }
}
