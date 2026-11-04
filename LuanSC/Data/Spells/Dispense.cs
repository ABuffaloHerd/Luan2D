using LuanSC.Data.Animations;
using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Spells;

/// <summary>
/// The Dispenser's turn: HP and metal for every friendly player on the 8 cells around the caster.
/// A player is anything controllable on the caster's team, so sentries and other buildings get nothing.
/// Someone without a metal bar, like a mage, still gets the HP.
/// </summary>
public class Dispense : CastableSpell
{
    public int HpAmount { get; }
    public int MetalAmount { get; }

    public Dispense(int hpAmount, int metalAmount)
        : base("Dispense", $"+{hpAmount} HP and +{metalAmount} metal to adjacent friendly players.", ActionType.SKILL, [], Targeting.FACING, 0, Pattern.Circle(1))
    {
        HpAmount = hpAmount;
        MetalAmount = metalAmount;
    }

    /// <summary>
    /// Adjacent friendly players with room for HP or metal. Empty means casting would do nothing.
    /// </summary>
    public IEnumerable<GameObject> Recipients(GameObject caster, ICombatContext context)
    {
        Team? team = caster.GetComponent<TeamComponent>()?.Team;
        ICollection<Point> area = AreaAt(caster, caster.Position);

        return context.Objects.Where(obj =>
        {
            if (obj == caster || !area.Contains(obj.Position)) return false;
            if (obj.GetComponent<ControllableComponent>() is null) return false;
            if (obj.GetComponent<TeamComponent>()?.Team != team) return false;

            HPComponent hp = obj.GetComponent<HPComponent>();
            if (hp is null || !hp.IsAlive) return false;

            ResourceComponent metal = obj.GetResource(ResourceType.METAL);
            return hp.Current < hp.Max || (metal is not null && metal.Current < metal.Max);
        });
    }

    public override void Resolve(GameObject caster, Point target, ICombatContext context)
    {
        // copy first, healing listeners shouldn't be able to trip up the loop
        foreach (GameObject player in Recipients(caster, context).ToList())
        {
            int healed = player.GetComponent<HPComponent>().Heal(new HealingRecord { Healer = caster, Target = player, Amount = HpAmount });
            if (healed > 0)
            {
                context.PlayAnimation(AnimationRegistry.HealFlash(player.Position));
                GameEvents.Publish(new CombatEvent.HealingDealt(caster, player, healed));
            }

            ResourceComponent metal = player.GetResource(ResourceType.METAL);
            if (metal is null) continue;

            int before = metal.Current;
            metal.Current += MetalAmount; // clamped to max

            if (metal.Current > before)
                GameEvents.Publish(new CombatEvent.GenericMessage($"{caster.Name} gave {player.Name} {metal.Current - before} {metal.DisplayName}."));
        }
    }
}
