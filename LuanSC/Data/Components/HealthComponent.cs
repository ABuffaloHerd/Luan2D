using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    public class HealthComponent : IComponent
    {
        string IComponent.Name => "HealthComponent";

        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public GameObject Owner { get; private set; }

        public void SetOwner(GameObject owner)
        {
            this.Owner = owner;
        }
    }
}
