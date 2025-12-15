using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    public class SPComponent : IComponent
    {
        public string Name => "SP Component";

        public GameObject Owner { get; private set; }

        public int MaxSP { get; set; }
        public int CurrentSP { get; set; }

        public void SetOwner(GameObject owner)
        {
            Owner = owner;
        }

        public SPComponent(int max = 50)
        { 
            MaxSP = max;
            CurrentSP = max;
        }
    }
}
