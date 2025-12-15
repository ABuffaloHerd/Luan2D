using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    /// <summary>
    /// Can the owner use magic?
    /// </summary>
    public class MPComponent : StatComponent
    {
        public override string Name => "MPComponent";

        public GameObject Owner { get; private set; }

        public MPComponent(int maxMana = 100)
        {
            Max = maxMana;
            Current = maxMana;
        }
    }
}
