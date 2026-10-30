using LuanSC.Data.Components;
using LuanSC.Objects.Weapons;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Objects.Mechs;

/// <summary>
/// Kits for TransformSpell. Each is the full set of components the pilot gets while inside, fresh every time.
/// Team and control come from the pilot, don't add them here.
/// </summary>
public static class MechRegistry
{
    // Luna's overdrive. Big, slow, hits hard. No resources or spells yet
    public static IEnumerable<IComponent> TitanAplu()
    {
        return
        [
            new HPComponent(300) { DEF = 5 },
            new CollisionComponent(),
            new SpeedComponent(60),
            new EffectComponent(),
            ActionComponent.CreateDefault(),
            new WeaponComponent(WeaponRegistry.HydraulicFists()),
        ];
    }
}
