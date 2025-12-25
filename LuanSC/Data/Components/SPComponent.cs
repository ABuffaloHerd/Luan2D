using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    public class SPComponent : StatComponent
    {
        public override string Name => "SP Component";

        public GameObject Owner { get; private set; }

        // alias for clarity
        public int MaxSP 
        { 
            get => Max;
            set => Max = value;
        }
        public int CurrentSP 
        { 
            get => Current;
            set => Current = value;
        }

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
