using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Data.Components.Attacks;
using LuanSC.Objects;

namespace LuanSC.Data.Spells;

/// <summary>
/// Sets off an ExplosionAttack on the picked cell, with its falloff and blocking. For grenades and the like.
/// The highlight is the blast's full radius, cover isn't shown.
/// </summary>
public class ExplosionSpell : CastableSpell
{
    public ExplosionAttack Blast { get; }

    public ExplosionSpell(string name, string description, ActionType type, int range, ExplosionAttack blast, params ResourceCost[] costs)
        : base(name, description, type, costs, Targeting.CURSOR, range, Pattern.Circle(blast.Radius))
    {
        Blast = blast;
    }

    public override void Resolve(GameObject caster, Point target, ICombatContext context)
        => Blast.Explode(caster, target, context);
}
