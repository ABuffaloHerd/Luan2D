using LuanSC.Data.Components;
using SadConsole.UI;
using SadConsole.UI.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes
{
    public partial class CombatScene
    {
        private class HUD : ControlsConsole
        {
            private ProgressBar hpBar;
            private ProgressBar mpBar;
            private ProgressBar spBar;
            private ProgressBar odBar;
            private ProgressBar gbBar;

            public HUD(int width, int height) : base(width, height)
            {
                Rebuild();
            }

            public HUD(int width, int height, ColoredGlyphBase[] initialCells) : base(width, height, initialCells)
            {
                Rebuild();
            }

            public HUD(ICellSurface surface, IFont? font = null, Point? fontSize = null) : base(surface, font, fontSize)
            {
                Rebuild();
            }

            public HUD(int width, int height, int bufferWidth, int bufferHeight) : base(width, height, bufferWidth, bufferHeight)
            {
                Rebuild();
            }

            public HUD(int width, int height, int bufferWidth, int bufferHeight, ColoredGlyphBase[]? initialCells) : base(width, height, bufferWidth, bufferHeight, initialCells)
            {
                Rebuild();
            }

            public void Clear()
            {
                base.Surface.Clear();
                base.Controls.Clear();
            }

            public void Rebuild()
            {
                // HP bar
                hpBar = new(this.Width - 5, 1, HorizontalAlignment.Left);
                hpBar.Position = new(this.Width - hpBar.Width, 0);
                hpBar.Progress = 1.0f;
                hpBar.BarColor = Color.Red;

                Controls.Add(hpBar);

                // MP bar
                mpBar = new(this.Width - 5, 1, HorizontalAlignment.Left);
                mpBar.Position = new(this.Width - hpBar.Width, 2);
                mpBar.Progress = 1.0f;
                mpBar.BarColor = Color.Blue;

                Controls.Add(mpBar);

                // SP bar
                spBar = new(this.Width - 5, 1, HorizontalAlignment.Left);
                spBar.Position = new(this.Width - spBar.Width, 4);
                spBar.Progress = 1.0f;
                spBar.BarColor = Color.Yellow;
            }

            public void UpdateHP(HPComponent component)
            {
                if (component is null) return;

                // get the max
                int max = component.Max;
                int current = component.Current;

                float val = (float)current / (float)max;

                hpBar.Progress = val;
                hpBar.DisplayText = $"{current} / {max}";
            }

            public void UpdateMP(MPComponent component)
            {
                if (component is null) return;
                int max = component.Max;
                int current = component.Current;

                float val = current / max;

                mpBar.Progress = val;
                mpBar.DisplayText = $"{current} m / {max}";
            }

            public void UpdateSP(SPComponent component)
            {
                if (component is null) return;
                int max = component.MaxSP;
                int current = component.CurrentSP;
                float val = current / max;
                spBar.Progress = val;
                spBar.DisplayText = $"{current} / {max}";
            }
        }
    }
}
