using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data;

[Flags]
public enum UnitType
{
    UNKNOWN     = 0, // eldritch horrors beyond human comprehension
    HUMAN       = 1 << 0,
    UNDEAD      = 1 << 1,
    MECHANICAL  = 1 << 2,
    BEAST       = 1 << 3,
    ELF         = 1 << 4,
    GOBLIN      = 1 << 5,
    DWARF       = 1 << 6,
    WALL        = 1 << 7, // not a unit, but useful for filtering
    MAGICAL     = 1 << 8, // magical constructs, golems, elementals
}
