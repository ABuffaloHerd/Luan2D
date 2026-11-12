using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Data.Spells;
using LuanSC.Objects;

namespace LuanSC.Data.Items;

/// <summary>
/// Makes an item's Use spell look like an ITEM action, so the spellbook window, targeting and CanCast work on it as they are.
/// The action it creates wraps the spell's own, and the scene takes the item away when that succeeds.
/// </summary>
public sealed class ItemUse(Item item) : ISpell
{
    public Item Item { get; } = item;

    public string Name => Item.Name;
    public string Description => Item.Description;
    public ActionType Type => ActionType.ITEM;
    public IReadOnlyList<ResourceCost> Costs => Item.Use.Costs;
    public Targeting Targeting => Item.Use.Targeting;
    public int Range => Item.Use.Range;
    public Pattern Area => Item.Use.Area;
    public string CastMessage => Item.Use.CastMessage;

    public TurnAction CreateAction(GameObject caster, Point target)
        => new TurnAction.UseItem(Item, Item.Use.CreateAction(caster, target));
}
