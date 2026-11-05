using LuanSC.Data.Animations;
using LuanSC.Data.Components.AI;
using LuanSC.Data.Components.Attacks;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Effects;

/// <summary>
/// The owner explodes when it dies, hitting everything in range. Friend or foe, so these can chain.
/// Not tagged Debuff so a cleanse can't defuse it. Tag a variant if a spell ever applies this to enemies.
/// </summary>
public class VolatileEffect : IEffect, IDeathListener
{
    public string Name => "Volatile";
    public GameObject Owner { get; private set; }

    public int Duration { get; set; }
    public int Damage { get; set; }
    public int Radius { get; set; }

    public VolatileEffect(int duration, int damage, int radius = 2)
    {
        Duration = duration;
        Damage = damage;
        Radius = radius;
    }

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
    }

    public void OnApply() { }
    public void OnRemove() { }
    public void OnTurnStart() { }
    public void OnTurnEnd() { }

    public void OnDeath(ICombatContext context)
    {
        GameEvents.Publish(new CombatEvent.GenericMessage($"{Owner.Name} explodes!"));
        context.PlayAnimation(AnimationRegistry.CircleBlast(Owner.Position, Radius, Color.OrangeRed));

        // only the living, so BlastAttack doesn't complain about hitting nothing. Walls and corpses are left out here too.
        List<GameObject> targets = context.Objects
            .Where(obj => obj.GetComponent<HPComponent>() is { IsAlive: true })
            .Where(obj => Pattern.CircleDistance(obj.Position - Owner.Position) <= Radius)
            .ToList();

        if (targets.Count > 0)
            new BlastAttack(Damage).Execute(Owner, targets, context);
    }
}
