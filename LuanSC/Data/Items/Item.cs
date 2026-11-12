
using LuanSC.Data.Components.Effects;
using LuanSC.Data.Spells;

namespace LuanSC.Data.Items;

/// <summary>
/// One kind of item. There's only this class, what an item does depends on which of the optional parts are set:
///   Use   - shows in the Items tab and can be used, e.g. a potion. Consumable decides if a use takes one away.
///   Held  - an effect on whoever carries it, e.g. a charm. One effect per item, so they stack.
///   neither - just sits there, e.g. a lit flare with CanPickUp off.
/// Build them in ItemRegistry. Units carry them in an InventoryComponent, the arena holds them as ItemGameObjects.
/// Items stack by Name.
/// </summary>
public class Item
{
    public string Name { get; init; }
    public string Description { get; init; }
    public char Glyph { get; init; }
    public Color Colour { get; init; } = Color.White;

    /// <summary>
    /// False for things that stay where they are in the arena, e.g. a lit flare.
    /// </summary>
    public bool CanPickUp { get; init; } = true;

    /// <summary>
    /// What using it does. Any spell works, the item supplies the name and it always costs an ITEM action.
    /// Null means it can't be used from the Items tab.
    /// </summary>
    public ISpell Use { get; init; }

    /// <summary>
    /// True takes one away per use. False for tools that never run out.
    /// </summary>
    public bool Consumable { get; init; } = true;

    /// <summary>
    /// Makes the effect the carrier gets while they hold this. Called once per item held. Use a duration of -1.
    /// </summary>
    public Func<IEffect> Held { get; init; }

    public Item(string name, string description, char glyph)
    {
        Name = name;
        Description = description;
        Glyph = glyph;
    }
}
