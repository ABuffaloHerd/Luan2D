using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Objects.Weapons;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Objects
{
    public class EnemyGameObject : GameObject
    {
        public EnemyGameObject(ColoredGlyphBase coloredGlyphBase, int zIndex, IAIBehavior behavior = null) : base(coloredGlyphBase, zIndex)
        {
            AddComponent(new SpeedComponent(100));
            AddComponent(new HPComponent(100));
            AddComponent(new CollisionComponent());
            AddComponent(new EffectComponent());
            AddComponent(new WeaponComponent(WeaponRegistry.Stick()));
            AddComponent(new AIComponent(behavior ?? new IdleBehavior()));
        }
    }
}
