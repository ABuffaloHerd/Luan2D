using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Spells;
// TODO: finish.

/// <summary>
/// Minako's overdrive. Shoot a projectile and hit every target in a straight line. Penetrate walls.
/// Also TODO: accompanying animation
/// </summary>
public class RailgunSpell : ISpell
{
    public string Name => throw new NotImplementedException();

    public string Description => throw new NotImplementedException();

    public ActionType Type => throw new NotImplementedException();

    public IReadOnlyList<ResourceCost> Costs => throw new NotImplementedException();

    public Targeting Targeting => throw new NotImplementedException();

    public int Range => throw new NotImplementedException();

    public Pattern Area => throw new NotImplementedException();

    public string CastMessage => throw new NotImplementedException();

    public TurnAction CreateAction(GameObject caster, Point target)
    {
        throw new NotImplementedException();
    }
}
