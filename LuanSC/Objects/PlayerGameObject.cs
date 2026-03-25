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
        public Weapon Weapon 
        { 
            get
            {
                return GetComponent<WeaponComponent>().Weapon;
            }
            set
            {
                GetComponent<WeaponComponent>().Weapon = value;
            }
        }

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
            Components.Add(new HPComponent());
            Components.Add(new MPComponent());
            Components.Add(new SPComponent(50));
            Components.Add(new CollisionComponent());
            Components.Add(new SpeedComponent(150));
            Components.Add(new ControllableComponent());
            Components.Add(new ODComponent());
            Components.Add(new EffectComponent());

            Components.Add(new WeaponComponent(WeaponRegistry.Katana()));

            foreach(var component in Components)
            {
                component.SetOwner(this);
            }
        }
    }
}
