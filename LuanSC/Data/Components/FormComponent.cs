using LuanSC.Data.Components.AI;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components;

/// <summary>
/// The owner has turned into something else, e.g. Luna in her mech. Holds everything they were before,
/// and puts it all back when this form runs out of HP (CombatScene.Cull). See TransformSpell.
/// </summary>
/// <remarks>
/// It's the same GameObject the whole time, so the turn order, team and anything pointing at the owner
/// (summons, sentries waiting to be built) carry on as normal.
/// The stashed components don't tick while they're put away, so resources are paused and come back as they were.
/// </remarks>
public class FormComponent : IComponent
{
    public string Name => "Form Component";
    public GameObject Owner { get; private set; }

    // who you are and who's driving, not what body you're in
    private static readonly Type[] kept = [typeof(TeamComponent), typeof(ControllableComponent), typeof(AIComponent)];

    private readonly List<IComponent> stashed = new();
    private string originalName;
    private int originalGlyph;

    /// <summary>
    /// Puts obj into a new form. Effects are cleared first, they don't come along or come back.
    /// Everything except team and control is swapped for the kit.
    /// </summary>
    public static void Enter(GameObject obj, string name, int glyph, IEnumerable<IComponent> kit)
    {
        obj.GetComponent<EffectComponent>()?.Clear();

        FormComponent form = new()
        {
            originalName = obj.Name,
            originalGlyph = obj.BaseAppearance?.Glyph ?? obj.AppearanceSingle.Appearance.Glyph
        };

        form.stashed.AddRange(obj.Components.Where(c => !IsKept(c)));
        obj.Components.RemoveWhere(c => !IsKept(c));

        foreach (IComponent component in kit)
            obj.AddComponent(component);

        obj.AddComponent(form);

        obj.Name = name;
        SetGlyph(obj, glyph);
    }

    /// <summary>
    /// Throws the form away and puts the owner back exactly as they were when they entered it.
    /// </summary>
    public void Revert()
    {
        Owner.Components.RemoveWhere(c => !IsKept(c)); // this included

        foreach (IComponent component in stashed)
            Owner.Components.Add(component); // still owned by them from before

        Owner.Name = originalName;
        SetGlyph(Owner, originalGlyph);
    }

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
    }

    private static bool IsKept(IComponent component) => kept.Any(type => type.IsInstanceOfType(component));

    // same as ConstructionEffect. Base too, Blink and summon colours read from it
    private static void SetGlyph(GameObject obj, int glyph)
    {
        obj.AppearanceSingle.Effect = null; // a running blink would restore the old glyph when it finishes
        obj.AppearanceSingle.Appearance.Glyph = glyph;
        obj.BaseAppearance?.Glyph = glyph;
        obj.IsDirty = true;
    }
}
