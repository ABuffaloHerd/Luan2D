using LuanSC.Data.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Objects
{
    public class EnemyGameObject : GameObject
    {
        public EnemyGameObject(ColoredGlyphBase coloredGlyphBase, int zIndex) : base(coloredGlyphBase, zIndex)
        {
            // default components: Speed, collision and HP
            Components.Add(new SpeedComponent(100));
            Components.Add(new HPComponent(100));
            Components.Add(new CollisionComponent());
        }
    }
}
