using LuanSC.Data.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Objects
{
    /// <summary>
    /// Anything created by another object. Joins the owner's team and disappears when the owner dies.
    /// Control (ControllableComponent or AIComponent), speed and weapons are added by whoever builds it. See SummonRegistry.
    /// </summary>
    public class SummonedGameObject : GameObject
    {
        public GameObject Owner { get; private set; }
        public SummonedGameObject(ColoredGlyphBase coloredGlyphBase, GameObject owner, int zIndex = 0) : base(coloredGlyphBase, zIndex)
        {
            Owner = owner;

            AddComponent(new HPComponent());
            AddComponent(new CollisionComponent());
            AddComponent(new EffectComponent());
            AddComponent(new TeamComponent(owner?.GetComponent<TeamComponent>()?.Team ?? Team.NEUTRAL));
        }
    }
}
