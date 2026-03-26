using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data;

/// <summary>
/// A damage record except... the exact opposite
/// </summary>
public record HealingRecord
{
    public required GameObject Healer { get; init; }
    public required GameObject Target { get; init; }
    public required int Amount { get; init; }
}
