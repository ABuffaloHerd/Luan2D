using SadConsole.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using LuanSC.Data.Components;

namespace LuanSC
{
    public class GameObject : Entity
    {
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
    }
}
