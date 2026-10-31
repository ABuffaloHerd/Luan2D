using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Spells;

/// <summary>
/// Turns the caster into another form until that form's HP runs out, then they're put back as they were.
/// The kit is every component the form has: HP, speed, weapon, spellbook and so on. Team and control stay. See FormComponent.
/// </summary>
/// <remarks>
/// FACING with an empty Area, it's cast on yourself so there's nothing to aim.
/// </remarks>
public class TransformSpell : CastSpell
{
    public string FormName { get; }
    public int Glyph { get; }
    public Func<IEnumerable<IComponent>> Kit { get; }

    /// <summary>
    /// What the caster counts as while transformed, e.g. MECHANICAL. Null keeps their own type.
    /// </summary>
    public UnitType? FormType { get; init; }

    public TransformSpell(string name, string description, ActionType type, string formName, int glyph, Func<IEnumerable<IComponent>> kit, params ResourceCost[] costs)
        : base(name, description, type, costs, Targeting.FACING, 1, new Pattern())
    {
        FormName = formName;
        Glyph = glyph;
        Kit = kit;
    }

    public override void Resolve(GameObject caster, Point target, ICombatContext context)
        => FormComponent.Enter(caster, FormName, Glyph, Kit(), FormType);
}
