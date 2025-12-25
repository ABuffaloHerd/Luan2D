using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Objects
{
    public class SummonedGameObject : GameObject
    {
        public GameObject Owner { get; private set; }
        public SummonedGameObject(ColoredGlyphBase coloredGlyphBase, GameObject owner, int zIndex = 0) : base(coloredGlyphBase, zIndex)
        {
            Owner = owner;
        }
    }
}
