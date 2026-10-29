using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
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

    // Player controlled.
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

    // summons wear their owner's colour so you can tell whose is whose
    // i can tell you this is going to cause trouble later
    private static Color OwnerColor(GameObject owner)
        => owner?.BaseAppearance?.Foreground ?? Color.White; // not the live appearance, Blink paints over that
}
