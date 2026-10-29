using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.Effects;

/// <summary>
/// An unbuilt building. Hits from the summoner build it up instead of hurting it. Anyone else hurts it normally.
/// Once HitsRequired is reached the glyph changes to BuiltGlyph and OnBuilt runs.
/// </summary>
public class ConstructionEffect : IEffect
{
    public const char UNBUILT_GLYPH = (char)220; // lower half block

    public string Name => "Construction";
    public GameObject Owner { get; private set; }

    public int Duration { get; set; } = -1; // until built

    public int HitsRequired { get; set; }
    public int Hits { get; private set; }
    public bool IsBuilt => Hits >= HitsRequired;

    public char BuiltGlyph { get; set; }

    /// <summary>
    /// Runs once when the building is finished. Add speed, weapons and AI here.
    /// </summary>
    public Action<GameObject> OnBuilt { get; set; }

    public ConstructionEffect(int hitsRequired, char builtGlyph, Action<GameObject> onBuilt = null)
    {
        HitsRequired = hitsRequired;
        BuiltGlyph = builtGlyph;
        OnBuilt = onBuilt;
    }

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
    }

    public void OnApply() { }
    public void OnRemove() { }
    public void OnTurnStart() { }
    public void OnTurnEnd() { }

    public DamageRecord OnIncomingDamage(DamageRecord damage)
    {
        if (IsBuilt) return damage;

        // only the one who summoned it can build it
        GameObject builder = (Owner as SummonedGameObject)?.Owner;
        if (builder is null || damage.Attacker != builder) return damage;

        // Counts every hit, not every attack. Multi-hit weapons like the katana build twice as fast.
        // This is a feature. Do not fix.
        Hits++;

        if (IsBuilt)
            Finish();
        else
            GameEvents.Publish(new CombatEvent.GenericMessage($"{builder.Name} works on the {Owner.Name} ({Hits}/{HitsRequired})."));

        return damage with { Amount = 0 };
    }

    private void Finish()
    {
        Owner.AppearanceSingle.Appearance.Glyph = BuiltGlyph;
        Owner.BaseAppearance?.Glyph = BuiltGlyph;
        Owner.IsDirty = true;

        Duration = 0; // the effect component clears it on the building's next tick

        OnBuilt?.Invoke(Owner);
        GameEvents.Publish(new CombatEvent.GenericMessage($"The {Owner.Name} is built."));
    }
}
