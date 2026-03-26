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
    bool CanUse(GameObject owner) => true; // checks for resources and bonus conditions
    void Execute(GameObject owner, IReadOnlyList<GameObject> targets);
}
