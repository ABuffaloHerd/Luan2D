using LuanSC.Data;
using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Data.Components.Effects;
using LuanSC.Data.Spells;
using LuanSC.Objects.Weapons;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Objects.Summons;

/// <summary>
/// Summon factories. Each takes the owner so it can be passed straight into TurnAction.Summon.
/// The "X summoned Y" message isn't set here. Players' summons set it with CastMessage on the spell in SpellRegistry,
/// AI summons with SummonerBehavior.Message.
/// </summary>
public static class SummonRegistry
{
    // AI controlled. Walks at the nearest hostile and hits it.
    public static SummonedGameObject Skeleton(GameObject owner)
    {
        SummonedGameObject s = new(new ColoredGlyph(OwnerColor(owner), Color.Transparent, 's'), owner, MagicNumbers.GAMEOBJECT_INDEX);
        s.Name = "Skeleton";
        s.UnitType = UnitType.UNDEAD;
        s.GetComponent<HPComponent>().Max = 30;

        s.AddComponent(new SpeedComponent(80));
        s.AddComponent(new WeaponComponent(WeaponRegistry.Hands()));
        s.AddComponent(ActionComponent.CreateDefault());
        s.AddComponent(new AIComponent(new ZombieBehavior()));
        return s;
    }

    // AI controlled. Slower and tougher than a skeleton, same idea.
    public static SummonedGameObject Zombie(GameObject owner)
    {
        SummonedGameObject z = new(new ColoredGlyph(OwnerColor(owner), Color.Transparent, 'z'), owner, MagicNumbers.GAMEOBJECT_INDEX);
        z.Name = "Zombie";
        z.UnitType = UnitType.UNDEAD;
        z.GetComponent<HPComponent>().Max = 50;

        z.AddComponent(new SpeedComponent(50));
        z.AddComponent(new WeaponComponent(WeaponRegistry.Hands()));
        z.AddComponent(ActionComponent.CreateDefault());
        z.AddComponent(new AIComponent(new ZombieBehavior()));
        return z;
    }

    // Player controlled testificate
    public static SummonedGameObject Familiar(GameObject owner)
    {
        SummonedGameObject f = new(new ColoredGlyph(OwnerColor(owner), Color.Transparent, 'f'), owner, MagicNumbers.GAMEOBJECT_INDEX);
        f.Name = "Familiar";
        f.UnitType = UnitType.BEAST;
        f.GetComponent<HPComponent>().Max = 40;

        f.AddComponent(new SpeedComponent(120));
        f.AddComponent(new WeaponComponent(WeaponRegistry.Stick()));
        f.AddComponent(ActionComponent.CreateDefault());
        f.AddComponent(new ControllableComponent());
        return f;
    }

    // Never takes a turn. Can be walked over. Gives the owner MP every turn until destroyed.
    public static SummonedGameObject MagicCircle(GameObject owner)
    {
        SummonedGameObject c = new(new ColoredGlyph(Color.MediumPurple, Color.Transparent, (char)15), owner, MagicNumbers.FLOOR_INDEX);
        c.Name = "Magic Circle";
        c.UnitType = UnitType.MAGICAL;
        c.GetComponent<HPComponent>().Max = 20;

        c.RemoveComponent<CollisionComponent>();
        c.AddComponent(new ManaWellComponent(10));
        return c;
    }

    // Building. Shows a half block until the owner hits it hitsRequired times, then becomes builtGlyph and starts shooting.
    // Doesn't take turns until built, and joins the turn order at the start of the next round.
    public static SummonedGameObject SentryGun(GameObject owner, int hitsRequired = 3, char builtGlyph = 'Y')
    {
        SummonedGameObject g = new(new ColoredGlyph(OwnerColor(owner), Color.Transparent, ConstructionEffect.UNBUILT_GLYPH), owner, MagicNumbers.GAMEOBJECT_INDEX);
        g.Name = "Sentry Gun";
        g.UnitType = UnitType.MECHANICAL;
        g.GetComponent<HPComponent>().Max = 50;

        g.GetComponent<EffectComponent>().Apply(new ConstructionEffect(hitsRequired, builtGlyph, built =>
        {
            built.AddComponent(new SpeedComponent(70));
            built.AddComponent(new WeaponComponent(WeaponRegistry.SentryGun()));
            built.AddComponent(new ActionComponent().Configure(ActionType.ATTACK, 1));
            built.AddComponent(new AIComponent(new SentryBehavior()));
        }));

        return g;
    }

    // Building. Shows a half block until the owner hits it hitsRequired times, then becomes a theta (glyph 233).
    // On its turn it gives every friendly player on the 8 cells around it HP and metal. Joins the turn order at the start of the next round.
    public static SummonedGameObject Dispenser(GameObject owner, int hitsRequired = 4, int hpAmount = 10, int metalAmount = 25)
    {
        SummonedGameObject d = new(new ColoredGlyph(OwnerColor(owner), Color.Transparent, ConstructionEffect.UNBUILT_GLYPH), owner, MagicNumbers.GAMEOBJECT_INDEX);
        d.Name = "Dispenser";
        d.UnitType = UnitType.MECHANICAL;
        d.GetComponent<HPComponent>().Max = 50;

        d.GetComponent<EffectComponent>().Apply(new ConstructionEffect(hitsRequired, (char)233, built =>
        {
            Dispense dispense = new(hpAmount, metalAmount) { CastMessage = "The dispenser works." };

            built.AddComponent(new SpeedComponent(1)); // act last in the round so players can move onto it before it dispenses
            built.AddComponent(new ActionComponent().Configure(ActionType.SKILL, 1));
            built.AddComponent(new AIComponent(new DispenserBehavior(dispense)));
        }));

        return d;
    }

    // summons wear their owner's colour so you can tell whose is whose
    // i can tell you this is going to cause visual trouble later
    private static Color OwnerColor(GameObject owner)
        => owner?.BaseAppearance?.Foreground ?? Color.White; // not the live appearance, Blink paints over that
}
