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
        public GameObject(Animated appearance, int zIndex) : base(appearance, zIndex)
        {

        }

        public GameObject(ColoredGlyphBase coloredGlyphBase, int zIndex) : base(coloredGlyphBase, zIndex)
        {

        }

        public void AddComponent<T>(T component) where T : IComponent
        {
            component.SetOwner(this);
            Components.Add(component);
        }

        public T GetComponent<T>() where T : IComponent => Components.OfType<T>().FirstOrDefault();

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
    }
}
