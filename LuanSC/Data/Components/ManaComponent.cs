using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    public class ManaComponent : IComponent
    {
        string IComponent.Name => "ManaComponent";
        public int MaxMana { get; set; }
        public int CurrentMana { get; set; }
        public GameObject Owner { get; private set; }
        public void SetOwner(GameObject owner)
        {
            this.Owner = owner;
        }
    }
}
