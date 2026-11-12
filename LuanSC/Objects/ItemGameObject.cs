using LuanSC.Data.Items;

namespace LuanSC.Objects;

/// <summary>
/// An item lying in the arena. Drawn on the floor layer under units, no collision and no HP,
/// so it doesn't block movement, attacks, blasts or line of sight.
/// A unit with an InventoryComponent picks it up by ending a move on it, unless Item.CanPickUp is off.
/// </summary>
public class ItemGameObject : GameObject
{
    public Item Item { get; }
    public int Count { get; }

    public ItemGameObject(Item item, int count = 1)
        : base(new ColoredGlyph(item.Colour, Color.Transparent, item.Glyph), MagicNumbers.FLOOR_INDEX)
    {
        Item = item;
        Count = Math.Max(count, 1);
        Name = Count > 1 ? $"{item.Name} x{Count}" : item.Name;
    }
}
