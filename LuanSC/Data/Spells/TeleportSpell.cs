using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using LuanSC.Data.Combat;

namespace LuanSC.Data.Spells;

public class TeleportSpell : CastableSpell
{
    public TeleportSpell() : base(
        name: "Teleport",
        description: "Teleport to a nearby location.",
        type: ActionType.SPELL,
        costs: new ResourceCost[] { new(ResourceType.MP, 10) },
        targeting: Targeting.CURSOR,
        range: 10,
        area: new Pattern()
        )
    {
    }
    public override void Resolve(GameObject caster, Point target, ICombatContext context)
    {
        // Check if the target position is valid (e.g., not occupied by a wall or another object)
        if (context.IsWalkable(target))
        {
            caster.Position = target;
            GameEvents.Publish(new CombatEvent.GenericMessage($"{caster.Name} teleports. Nothing personnel, kid."));
        }
        else
        {
            // Optionally, you could provide feedback to the player that the teleport failed
            GameEvents.Publish(new CombatEvent.GenericMessage("Invalid destination.")); // TODO: instagib
        }
    }
}
