using LuanSC.Data.Components;
using LuanSC.Objects.Weapons;
using LuanSC.Data;

using System;
using System.Collections.Generic;
using System.Text;
using Direction = LuanSC.Data.Direction;
using SadConsole.Effects;

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

        public void Blink()
        {
            Blinker b = new();
            b.BlinkCount = 3;
            b.BlinkSpeed = TimeSpan.FromMilliseconds(500);
            b.RestoreCellOnRemoved = true;
            b.RemoveOnFinished = true;

            if (IsSingleCell)
                b.BlinkOutBackgroundColor = AppearanceSingle.Appearance.Foreground;

            AppearanceSingle?.Effect = b;
        }

        private void InitializePlayer()
        {
            // Initialization logic for player-specific attributes can be added here.
            // Add default components for player objects.
            Components.Add(new HPComponent());
            Components.Add(new MPComponent());
            Components.Add(new SPComponent(50));
            Components.Add(new CollisionComponent());
            Components.Add(new SpeedComponent(150));


            foreach(var component in Components)
            {
                component.SetOwner(this);
            }
        }
    }
}
