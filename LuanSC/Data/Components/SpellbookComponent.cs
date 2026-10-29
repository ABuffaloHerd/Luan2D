using LuanSC.Data.Spells;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components;

/// <summary>
/// The spells and skills the owner knows. No component, no spellbook.
/// </summary>
public class SpellbookComponent : IComponent
{
    public string Name => "Spellbook Component";
    public GameObject Owner { get; private set; }

    public List<ISpell> Spells { get; } = new();

    public SpellbookComponent(params ISpell[] spells)
    {
        Spells.AddRange(spells);
    }

    public IEnumerable<ISpell> OfType(ActionType type) => Spells.Where(s => s.Type == type);

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
    }
}
