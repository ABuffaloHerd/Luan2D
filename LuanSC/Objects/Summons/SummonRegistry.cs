using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Data.Components.Effects;
using LuanSC.Objects.Weapons;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Objects.Summons;

/// <summary>
/// Summon factories. Each takes the owner so it can be passed straight into TurnAction.Summon.
/// </summary>
public static class SummonRegistry
{
    // AI controlled. Walks at the nearest hostile and hits it.
    public static SummonedGameObject Skeleton(GameObject owner)
    {
        SummonedGameObject s = new(new ColoredGlyph(OwnerColor(owner), Color.Transparent, 's'), owner, MagicNumbers.GAMEOBJECT_INDEX);
        s.Name = "Skeleton";
        s.GetComponent<HPComponent>().Max = 30;

        s.AddComponent(new SpeedComponent(80));
        s.AddComponent(new WeaponComponent(WeaponRegistry.Hands()));
        s.AddComponent(ActionComponent.CreateDefault());
        s.AddComponent(new AIComponent(new ZombieBehavior()));
        return s;
    }

    // Player controlled testificate
    public static SummonedGameObject Familiar(GameObject owner)
    {
        SummonedGameObject f = new(new ColoredGlyph(OwnerColor(owner), Color.Transparent, 'f'), owner, MagicNumbers.GAMEOBJECT_INDEX);
        f.Name = "Familiar";
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

    // summons wear their owner's colour so you can tell whose is whose
    // i can tell you this is going to cause visual trouble later
    private static Color OwnerColor(GameObject owner)
        => owner?.BaseAppearance?.Foreground ?? Color.White; // not the live appearance, Blink paints over that
}
