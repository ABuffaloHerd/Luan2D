using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    public class SPComponent : StatComponent, IHudDisplay
    {
        public override string Name => "SP Component";

        public IEnumerable<HudBar> GetHudBars()
        {
            yield return new HudBar(20, "SP :", Color.Yellow, Color.Yellow, HudBar.Ratio(Current, Max), $"{Current} / {Max}", Color.Black);
        }

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

        public SPComponent(int max = 50)
        { 
            MaxSP = max;
            CurrentSP = max;
        }
    }
}
