using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    /// <summary>
    /// Can the owner use magic?
    /// Just a resource with MP's colours, so it works with ResourceCost and GetResource(ResourceType.MP).
    /// </summary>
    public class MPComponent : ResourceComponent
    {
        public override string Name => "MPComponent";

        // alias for clarity
        public int MaxMP 
        { 
            get => Max;
            set => Max = value;
        }
        public int CurrentMP 
        { 
            get => Current;
            set => Current = value;
        }

        public MPComponent(int maxMana = 100) : base(ResourceType.MP, "MP", "MP", Color.Blue, maxMana)
        {
            LabelColor = Color.LightBlue;
            HudOrder = 10;
        }
    }
}
