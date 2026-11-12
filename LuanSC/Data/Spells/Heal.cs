using LuanSC.Data.Animations;
using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using LuanSC.Data.Combat;

namespace LuanSC.Data.Spells;

/// <summary>
/// Heals whoever is standing on the picked cell. Can't raise the dead or patch up walls.
/// </summary>
public class Heal : CastableSpell
{
    public int Amount { get; }

    public Heal(int amount, int range, params ResourceCost[] costs)
        : base("Heal", $"Heals {amount} HP.", ActionType.SPELL, costs, Targeting.CURSOR, range, Pattern.Circle(0))
    {
        Amount = amount;
    }

    // refused rather than wasted, so a potion isn't used up on nothing
    public override string Refusal(GameObject caster, Point target, ICombatContext context)
    {
        GameObject patient = PatientAt(target, context);
        if (patient is null) return "There's nobody there to heal.";

        HPComponent hp = patient.GetComponent<HPComponent>();
        if (hp.Current >= hp.Max) return $"{patient.Name} is already at full health.";

        return base.Refusal(caster, target, context);
    }

    private static GameObject PatientAt(Point target, ICombatContext context)
        => context.Objects.FirstOrDefault(obj => obj.Position == target && obj.GetComponent<HPComponent>() is { IsAlive: true });

    public override void Resolve(GameObject caster, Point target, ICombatContext context)
    {
        GameObject patient = PatientAt(target, context);

        if (patient is null)
        {
            GameEvents.Publish(new CombatEvent.GenericMessage($"{caster.Name}'s {Name} fizzles on empty ground."));
            return;
        }

        context.PlayAnimation(AnimationRegistry.HealFlash(target));

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
