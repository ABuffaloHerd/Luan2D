using LuanSC.Data.Components;
using LuanSC.Objects.Weapons;
using LuanSC.Data;

using System;
using System.Collections.Generic;
using System.Text;
using Direction = LuanSC.Data.Direction;

namespace LuanSC.Objects
{
    /// <summary>
    /// Friendly controllable player objects.
    /// </summary>
    public class PlayerGameObject : GameObject
    {
        public Direction Direction = Direction.RIGHT;
        public Weapon Weapon { get; set; }
        public PlayerGameObject(Animated appearance, int zIndex) : base(appearance, zIndex)
        {
            InitializePlayer();
        }
        public PlayerGameObject(ColoredGlyphBase coloredGlyphBase, int zIndex) : base(coloredGlyphBase, zIndex)
        {
            InitializePlayer();
        }

        private void InitializePlayer()
        {
            // Initialization logic for player-specific attributes can be added here.
            // Add default components for player objects.
            Components.Add(new HealthComponent());
            Components.Add(new ManaComponent());
            Components.Add(new CollisionComponent());

            foreach(var component in Components)
            {
                component.SetOwner(this);
            }
        }
    }
}
