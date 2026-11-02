using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Effects;

/// <summary>
/// An effect that changes how many actions the owner gets each turn, e.g. one more attack.
/// ActionComponent adds these up when it refills at the start of the owner's turn, so a bonus starts on their next turn
/// and ends when the effect does. Bonuses stack, negative values work for debuffs, and a counter never goes below 0.
/// An action type with no limit stays unlimited.
/// </summary>
public interface IActionModifier
{
    int Bonus(ActionType type) => 0;
}
