using LuanSC.Data;
using LuanSC.Data.Components;
using SadConsole.UI;
using SadConsole.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LuanSC.Scenes;

public partial class CombatScene
{
    private class HUD : ControlsConsole
    {
        private ProgressBar hpBar;
        private ProgressBar mpBar;
        private ProgressBar spBar;
        private ProgressBar odBar;
        private ProgressBar gbBar;

        private Label hplabel;
        private Label mplabel;
        private Label splabel;
        private Label odlabel;
        private Label gblabel;

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

            hplabel = new(this.Width - hpBar.Width);
            hplabel.Position = new(0, hpBar.Position.Y);
            hplabel.DisplayText = "HP :";
            hplabel.TextColor = Color.Red;

            Controls.Add(hpBar);
            Controls.Add(hplabel);

            // MP bar
            mpBar = new(this.Width - 5, 1, HorizontalAlignment.Left);
            mpBar.Position = new(this.Width - hpBar.Width, 2);
            mpBar.Progress = 1.0f;
            mpBar.BarColor = Color.Blue;

            mplabel = new(this.Width - mpBar.Width);
            mplabel.Position = new(0, mpBar.Position.Y);
            mplabel.DisplayText = "MP :";
            mplabel.TextColor = Color.LightBlue;

            Controls.Add(mpBar);
            Controls.Add(mplabel);

            // SP bar
            spBar = new(this.Width - 5, 1, HorizontalAlignment.Left);
            spBar.Position = new(this.Width - spBar.Width, 4);
            spBar.Progress = 1.0f;
            spBar.BarColor = Color.Yellow;
            spBar.DisplayTextColor = Color.Black;

            splabel = new(this.Width - spBar.Width);
            splabel.Position = new(0, spBar.Position.Y);
            splabel.DisplayText = "SP :";
            splabel.TextColor = Color.Yellow;

            Controls.Add(spBar);
            Controls.Add(splabel);

            // OD bar
            odBar = new(this.Width - 5, 1, HorizontalAlignment.Left);
            odBar.Position = new(this.Width - odBar.Width, 6);
            odBar.Progress = 1.0f; // placeholder
            odBar.BarColor = Color.Purple;

            odlabel = new(this.Width - odBar.Width);
            odlabel.Position = new(0, odBar.Position.Y);
            odlabel.DisplayText = $"OD{(char)224}:";
            odlabel.TextColor = Color.Purple;

            Controls.Add(odBar);
            Controls.Add(odlabel);

            // GB bar
            gbBar = new(this.Width - 5, 1, HorizontalAlignment.Left);
            gbBar.Position = new(this.Width - gbBar.Width, 8);
            gbBar.Progress = 1.0f; // placeholder
            gbBar.BarColor = Color.Gold;
            gbBar.DisplayTextColor = Color.DarkGray;

            gblabel = new(this.Width - gbBar.Width);
            gblabel.Position = new(0, gbBar.Position.Y);
            gblabel.DisplayText = $"GB{(char)225}:";
            gblabel.TextColor = Color.DarkGray;

            Controls.Add(gbBar);
            Controls.Add(gblabel);
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

            // put a label
        }

        public void UpdateMP(MPComponent component)
        {
            if (component is null)
            {
                mpBar.IsVisible = false;
                mplabel.IsVisible = false;
                return;
            }
            else
            {
                mpBar.IsVisible = true;
                mplabel.IsVisible = true;
            }

            int max = component.Max;
            int current = component.Current;

            float val = (float)current / (float)max;

            mpBar.Progress = val;
            mpBar.DisplayText = $"{current} / {max}";
        }

        public void UpdateSP(SPComponent component)
        {
            if (component is null)
            {
                spBar.IsVisible = false;
                splabel.IsVisible = false;
                return;
            }
            else
            {
                spBar.IsVisible = true;
                splabel.IsVisible = true;
            }

            int max = component.MaxSP;
            int current = component.CurrentSP;

            float val = (float)current / (float)max;

            spBar.Progress = val;
            spBar.DisplayText = $"{current} / {max}";
        }

        public void UpdateOD(ODComponent component)
        {
            if (component is null)
            {
                odBar.IsVisible = false;
                odlabel.IsVisible = false;

                gbBar.IsVisible = false;
                gblabel.IsVisible = false;
                return;

            }
            else
            {
                odBar.IsVisible = true;
                odlabel.IsVisible = true;

                gbBar.IsVisible = true;
                gblabel.IsVisible = true;
            }

            float val = (float)component.CurrentOD / 100f;

            odBar.Progress = val;
            odBar.DisplayText = $"{component.CurrentOD}%";

            float gbval = component.CurrentGB / 100f;
            gbBar.Progress = gbval;
            gbBar.DisplayText = $"{component.CurrentGB}%";
        }
    }
}
