using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    /// <summary>
    /// Skill points. Just a resource with SP's colours, so it works with ResourceCost and GetResource(ResourceType.SP).
    /// </summary>
    public class SPComponent : ResourceComponent
    {
        public override string Name => "SP Component";

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

        public SPComponent(int max = 50) : base(ResourceType.SP, "SP", "SP", Color.Yellow, max)
        {
            TextColor = Color.Black;
            HudOrder = 20;
        }
    }
}
