using LuanSC.Data.Animations;
using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Spells;

/// <summary>
/// Heals whoever is standing on the picked cell. Can't raise the dead or patch up walls.
/// </summary>
public class Heal : CastSpell
{
    public int Amount { get; }

    public Heal(int amount, int range, params ResourceCost[] costs)
        : base("Heal", $"Heals {amount} HP.", ActionType.SPELL, costs, Targeting.CURSOR, range, Pattern.Circle(0))
    {
        Amount = amount;
    }

    public override void Resolve(GameObject caster, Point target, ICombatContext context)
    {
        context.PlayAnimation(AnimationRegistry.CircleBlast(target, 0, Color.LimeGreen, glyphs: ['+', '+', '.']));

        GameObject patient = context.Objects.FirstOrDefault(obj =>
            obj.Position == target && obj.GetComponent<HPComponent>() is { IsAlive: true });

        if (patient is null)
        {
            GameEvents.Publish(new CombatEvent.GenericMessage($"{caster.Name}'s {Name} fizzles on empty ground."));
            return;
        }

        HealingRecord healing = new()
        {
            Healer = caster,
            Target = patient,
            Amount = Amount
        };

        int healed = patient.GetComponent<HPComponent>().Heal(healing);

        GameEvents.Publish(new CombatEvent.HealingDealt(caster, patient, healed));
    }
}
