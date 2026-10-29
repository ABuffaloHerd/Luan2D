using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    /// <summary>
    /// The second overdrive bar. Starts empty and only fills from OD charge that spills over, see ODComponent.
    /// Anything past full is lost.
    /// </summary>
    public class GBComponent : ResourceComponent
    {
        public override string Name => "GB Component";

        public GBComponent(int max = 100) : base(ResourceType.GB, "GB", $"GB{(char)225}", Color.Gold, max)
        {
            LabelColor = Color.DarkGray;
            TextColor = Color.DarkGray;
            HudOrder = 31;
            Current = 0;
        }

        protected override string HudText => $"{(int)(Current * 100f / Max)}%";
    }
}
