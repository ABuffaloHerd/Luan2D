using LuanSC.Data;
using LuanSC.Data.Animations;
using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes;

public partial class CombatScene
{
    public void PlayAnimation(AnimatedEffect animation)
    {
        surface.Children.Add(animation);
    }

    /// <summary>
    /// Runs a TurnAction.Cast. The action counter is checked in Execute, this checks the target and pays the costs.
    /// </summary>
    /// <returns>false if nothing happened, so Execute doesn't spend the action.</returns>
    private bool ResolveCast(GameObject caster, TurnAction.Cast cast)
    {
        Point target = cast.Target;

        // the cursor already stops this for players, but AI and anything else goes through here too
        if (target.X < 0 || target.Y < 0 || target.X >= surface.Width || target.Y >= surface.Height
            || Distance(caster.Position, target) > cast.Spell.Range)
        {
            GameEvents.Publish(new CombatEvent.GenericMessage($"{cast.Spell.Name} can't reach there."));
            return false;
        }

        if (!TryPayCosts(caster, cast.Spell.Costs)) return false;

        // before Resolve, a transform renames the caster.
        // This is only the default, change a single spell's message with CastMessage in SpellRegistry.
        string message = (cast.Spell.CastMessage ?? "{caster} casts {spell}!")
            .Replace("{caster}", caster.Name)
            .Replace("{spell}", cast.Spell.Name);

        GameEvents.Publish(new CombatEvent.GenericMessage(message));
        cast.Spell.Resolve(caster, target, this);

        RefreshTurnOrder(); // transforms rename the caster
        return true;
    }

    /// <summary>
    /// Takes every cost, or none of them if any one can't be paid.
    /// </summary>
    private bool TryPayCosts(GameObject payer, IReadOnlyList<ResourceCost> costs)
    {
        costs ??= [];

        foreach (ResourceCost cost in costs)
        {
            ResourceComponent resource = payer.GetResource(cost.Type);
            if (resource is null || !resource.CanSpend(cost.Amount))
            {
                GameEvents.Publish(new CombatEvent.GenericMessage($"{payer.Name} needs {cost.Amount} {resource?.DisplayName ?? cost.Type.ToString()} for that."));
                return false;
            }
        }

        foreach (ResourceCost cost in costs)
            payer.GetResource(cost.Type).TrySpend(cost.Amount);

        return true;
    }
}
