using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Effects;

/// <summary>
/// An effect that adds to the owner's stats while it's active, e.g. Armour adds DEF.
/// HPComponent sums these into TotalDEF and TotalRES, so the bonus goes through exactly the same maths as the base stat.
/// Only implement the stats you change, the rest default to 0. Negative values work for debuffs.
/// </summary>
public interface IStatModifier
{
    int DEF => 0;
    int RES => 0;
}
