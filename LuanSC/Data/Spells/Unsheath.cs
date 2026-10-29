using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Spells;

/// <summary>
/// Special move: one hit of flat magic damage to everything in the caster's current weapon range.
/// RES still reduces it. Like a weapon attack it hits anything with HP in range, except the caster.
/// </summary>
/// <remarks>
/// FACING, so it goes off straight away. Area is empty because the cells depend on whatever weapon is equipped
/// at the time; the weapon range overlay (H) already shows them.
/// </remarks>
public class Unsheath : CastSpell
{
    public int Damage { get; }

    public Unsheath(int damage, params ResourceCost[] costs)
        : base("Unsheath", $"{damage} magic dmg, weapon range.", ActionType.SPECIAL, costs, Targeting.FACING, 1, new Pattern())
    {
        Damage = damage;
    }

    public override void Resolve(GameObject caster, Point target, ICombatContext context)
    {
        Pattern range = caster.GetComponent<WeaponComponent>()?.Weapon?.Range;
        if (range is null)
        {
            GameEvents.Publish(new CombatEvent.GenericMessage($"{caster.Name} has nothing to unsheath."));
            return;
        }

        // weapon ranges are relative to the wielder, not the target cell
        HashSet<Point> cells = range.GetRotated(caster.Direction).Select(offset => caster.Position + offset).ToHashSet();

        // copy first, damage listeners shouldn't be able to trip up the loop
        foreach (GameObject obj in context.Objects.ToList())
        {
            if (obj == caster || !cells.Contains(obj.Position)) continue;

            HPComponent hp = obj.GetComponent<HPComponent>();
            if (hp is null || !hp.IsAlive) continue; // walls and corpses

            DamageRecord pain = new()
            {
                Attacker = caster,
                Target = obj,
                Amount = Damage,
                Type = DamageType.MAGIC,
                Source = Name
            };

            int dealt = hp.TakeDamage(pain);

            GameEvents.Publish(new CombatEvent.DamageDealt(caster, obj, dealt, DamageType.MAGIC));
        }
    }
}
