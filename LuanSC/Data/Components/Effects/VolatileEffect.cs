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
        new ExplosionAttack(Damage, Radius).Explode(Owner, Owner.Position, context);
    }
}
