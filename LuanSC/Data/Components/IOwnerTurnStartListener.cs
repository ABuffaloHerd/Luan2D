using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components;

/// <summary>
/// Put this on a summon's component to do something at the start of its owner's turn.
/// Works for summons that never get a turn of their own, like magic circles.
/// </summary>
public interface IOwnerTurnStartListener
{
    void OnOwnerTurnStart(GameObject owner);
}
