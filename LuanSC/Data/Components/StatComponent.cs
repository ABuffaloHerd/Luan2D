using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    public abstract class StatComponent : IComponent
    {
        public abstract string Name { get; }
        public GameObject Owner { get; private set; }

        // private backing fields
        protected int max;
        protected int current;

        public int Current
        {
            get => current;
            set => current = Math.Clamp(value, 0, max);
        }

        public int Max
        {
            get => max;
            set
            {
                max = value;
                current = Math.Min(value, max);
            }
        }

        public virtual void SetOwner(GameObject owner)
        {
            Owner = owner;
        }
    }
}
