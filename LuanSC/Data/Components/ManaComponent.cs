using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    public class ManaComponent : IComponent
    {
        public string Name => "ManaComponent";
        public int MaxMana { get; set; }
        public int CurrentMana { get; set; }
        public GameObject Owner { get; private set; }
        public void SetOwner(GameObject owner)
        {
            this.Owner = owner;
        }

        public ManaComponent(int maxMana = 100)
        {
            MaxMana = maxMana;
            CurrentMana = maxMana;
        }
    }
}
