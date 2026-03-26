using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Attacks;

/// <summary>
/// Whacks everything in range
/// </summary>
public class BlastAttack : IAbility
{
    public string Name => "Blast Attack";

    public string Description => "indiscriminately whacks everything it can ";

    public int Damage { get; private set; }

    public BlastAttack(int damage)
    {
        Damage = damage;
    }

    public void Execute(GameObject owner, IReadOnlyList<GameObject> targets)
    {
        if (targets.Count > 0)
        {
            // laugh
            GameEvents.Publish(new CombatEvent.GenericMessage($"Where is {owner.Name} aiming?"));
        }
    }
}
