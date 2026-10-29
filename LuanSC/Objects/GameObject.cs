using LuanSC.Data;
using LuanSC.Data.Components;
using SadConsole.Effects;
using SadConsole.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Direction = LuanSC.Data.Direction;

namespace LuanSC.Objects
{
    public abstract class GameObject : Entity
    {
        public Direction Direction = Direction.RIGHT;
        public HashSet<IComponent> Components { get; } = new HashSet<IComponent>();

        /// <summary>
        /// Copy of the glyph this object was created with. Effects like Blink overwrite the live appearance, so read colours from here.
        /// Null for animated objects.
        /// </summary>
        public ColoredGlyphBase BaseAppearance { get; }

        public GameObject(Animated appearance, int zIndex) : base(appearance, zIndex)
        {

        }

        public GameObject(ColoredGlyphBase coloredGlyphBase, int zIndex) : base(coloredGlyphBase, zIndex)
        {
            BaseAppearance = coloredGlyphBase.Clone();
        }

        public void AddComponent<T>(T component) where T : IComponent
        {
            component.SetOwner(this);
            Components.Add(component);
        }

        public T GetComponent<T>() where T : IComponent => Components.OfType<T>().FirstOrDefault();

        public void Blink()
        {
            Blinker b = new()
            {
                BlinkCount = 3,
                BlinkSpeed = TimeSpan.FromMilliseconds(500),
                RestoreCellOnRemoved = true,
                RemoveOnFinished = true
            };

            if (IsSingleCell && BaseAppearance is not null)
                b.BlinkOutBackgroundColor = BaseAppearance.Foreground;

            AppearanceSingle?.Effect = b;
        }
    }
}
