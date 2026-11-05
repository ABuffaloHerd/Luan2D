using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Attacks;

public interface IAbility
{
    string Name { get; }
    string Description { get; }
    int Damage { get; } 
    DamageType Type { get; }
    bool CanUse(GameObject owner) => true; // checks for resources and bonus conditions

    /// <summary>
    /// Hits the targets. Damage, costs and the like are up to the ability.
    /// </summary>
    /// <param name="targets">Everything in the weapon's range, attacker excluded. Can include walls and corpses.</param>
    /// <param name="context">The fight, for animations (context.PlayAnimation) and looking at other objects. Same as spells get.</param>
    void Execute(GameObject owner, IReadOnlyList<GameObject> targets, ICombatContext context);
}
