using LuanSC.Data.Components;
using LuanSC.Data.Components.Attacks;
using LuanSC.Data.Components.Effects;
using LuanSC.Data.Spells;

namespace LuanSC.Objects.Items;

/// <summary>
/// Item factories. Add an ID for each in IdDictionary so levels can hand them out or leave them on the floor.
///
/// Use takes any spell. Set its CastMessage, the default reads "{caster} casts {spell}!" with the spell's name, not the item's.
/// </summary>
public static class ItemRegistry
{
    public static Item Potion(int amount = 30)
    {
        return new Item("Potion", $"Heals {amount} HP.", '!')
        {
            Colour = Color.Red,
            Use = new Heal(amount, 0) { Targeting = Targeting.SELF, CastMessage = "{caster} drinks a potion." }
        };
    }

    public static Item Grenade(int damage = 40, int radius = 2, int range = 4)
    {
        return new Item("Grenade", $"{damage} dmg, radius {radius}. Mind your own feet.", (char)7)
        {
            Colour = Color.Green,
            Use = new ExplosionSpell("Grenade", "", ActionType.ITEM, range, new ExplosionAttack(damage, radius))
            {
                CastMessage = "{caster} throws a grenade!"
            }
        };
    }

    public static Item Flare(int range = 3)
    {
        return new Item("Flare", "Lights up and stays where it lands.", 'i')
        {
            Colour = Color.Orange,
            Use = new PlaceItem("Flare", "", LitFlare, range) { CastMessage = "{caster} lights a flare." }
        };
    }

    // what a Flare turns into. Stays on the floor, nobody can pick it up. Doesn't do anything yet
    public static Item LitFlare()
    {
        return new Item("Lit Flare", "Too hot to pick up.", '*')
        {
            Colour = Color.OrangeRed,
            CanPickUp = false
        };
    }

    // passive, each one held adds its own armour
    public static Item ArmourCharm(int amount = 5)
    {
        return new Item("Armour Charm", $"+{amount} DEF while held.", (char)4)
        {
            Colour = Color.Gold,
            Held = () => new ArmourEffect(-1, amount)
        };
    }
}
