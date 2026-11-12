using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Data.Items;
using LuanSC.Objects;

namespace LuanSC.Data.Spells;

/// <summary>
/// Puts an item on the picked cell of the arena, e.g. a lit flare. Units can stand on the cell, walls, furniture and other items can't share it.
/// </summary>
public class PlaceItem : CastableSpell
{
    public Func<Item> Create { get; }

    public PlaceItem(string name, string description, Func<Item> create, int range, params ResourceCost[] costs)
        : base(name, description, ActionType.ITEM, costs, Targeting.CURSOR, range, new Pattern().Mark(0, 0))
    {
        Create = create;
        UnitType = UnitType.ITEM;
    }

    public override string Refusal(GameObject caster, Point target, ICombatContext context)
    {
        bool taken = context.Objects.Any(obj => obj.Position == target && obj is ItemGameObject or Wall or Furniture);
        return taken ? "There's no room for that there." : base.Refusal(caster, target, context);
    }

    public override void Resolve(GameObject caster, Point target, ICombatContext context)
    {
        context.Spawn(new ItemGameObject(Create()) { Position = target });
    }
}
