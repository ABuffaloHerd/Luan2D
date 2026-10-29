using LuanSC.Data;
using LuanSC.Data.Components;
using SadConsole.UI;
using SadConsole.UI.Controls;
using System.Diagnostics;


namespace LuanSC.Scenes;

public partial class CombatScene
{
    /// <summary>
    /// A column of generic bar slots. What goes in them is decided by the components, see IHudDisplay.
    /// </summary>
    /// <remarks>
    /// HOW SADCONSOLE REDRAWS CONTROLS (read this before touching Show/Apply)
    ///
    /// Each frame goes:
    ///   1. Update: ControlHost calls UpdateAndRedraw on every control. That repaints the control's own
    ///      surface, but returns immediately unless control.IsDirty is set.
    ///   2. Our CombatScene.Update runs after base.Update, so anything we change lands here.
    ///   3. Render: if the ControlHost is dirty, the render step composites every visible control's surface
    ///      onto the console and then clears control.IsDirty.
    ///
    /// So if we only set properties (which sets IsDirty) in step 2, step 3 composites the control's OLD surface
    /// and wipes the flag. Next frame UpdateAndRedraw sees nothing dirty and never repaints. The new value only
    /// shows up when something else dirties the control during step 1, e.g. the mouse hovering over it.
    ///
    /// It only bit us on player turns because hidden controls never get their IsDirty cleared (the render step
    /// skips them), which keeps the host dirty and step 3 running every frame. The old fixed layout had no hidden
    /// slots for players, so it never happened there.
    ///
    /// The fix, in Apply: after changing a slot we call UpdateAndRedraw ourselves, so the control's surface is
    /// already current, and set Controls.IsDirty so the host composites it this frame.
    ///
    /// Other gotchas found along the way (SadConsole 10.8):
    ///  - Label.TextColor does not set IsDirty.
    ///  - ProgressBar.DisplayText/BarColor/DisplayTextColor set IsDirty even when the value hasn't changed,
    ///    so Show only touches slots whose HudBar actually changed.
    ///  - Surface.Clear() does nothing for controls. They're drawn on a separate render target that is wiped
    ///    on every composite.
    ///
    /// Source: ControlHost.cs and ControlHostRenderStep.cs (SadConsole.Host.MonoGame) on the SadConsole GitHub.
    /// </remarks>
    /// from a real person: tl;dr DON'T TOUCH. as a human myself i'm not fucking reading this wall of text so heed the warning now.
    private class HUD : ControlsConsole
    {
        // HP, MP, SP, OD, GB, MV, AT and three spare for custom resources. As many as fit the panel's width.
        public const int SLOTS = 10;
        private const int LABEL_WIDTH = 5;

        private readonly ProgressBar[] bars = new ProgressBar[SLOTS];
        private readonly Label[] labels = new Label[SLOTS];
        private Color? defaultTextColor;

        // what each slot is currently showing, null means hidden
        private readonly HudBar[] shown = new HudBar[SLOTS];

        private bool warnedOverflow;

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
            Controls.Clear();

            for (int i = 0; i < SLOTS; i++)
            {
                int y = i * 2; // a blank line between bars

                ProgressBar bar = new(this.Width - LABEL_WIDTH, 1, HorizontalAlignment.Left)
                {
                    Position = new(LABEL_WIDTH, y),
                    IsVisible = false
                };

                Label label = new(LABEL_WIDTH)
                {
                    Position = new(0, y),
                    IsVisible = false
                };

                bars[i] = bar;
                labels[i] = label;

                Controls.Add(bar);
                Controls.Add(label);
            }

            defaultTextColor = bars[0].DisplayTextColor;
        }

        /// <summary>
        /// Wipes every slot. Call when the turn changes so nothing from the last unit is left behind.
        /// </summary>
        public void Reset()
        {
            Array.Clear(shown);

            for (int i = 0; i < SLOTS; i++)
                Apply(i, null);
        }

        /// <summary>
        /// Fills slots in order and hides the rest. Anything past SLOTS is dropped.
        /// Only touches slots that changed. SadConsole controls redraw on every property set, even with the same value.
        /// </summary>
        public void Show(IEnumerable<HudBar> hudBars)
        {
            List<HudBar> ordered = hudBars.OrderBy(b => b.Order).ToList();

            // the panel only fits SLOTS bars across its width, the highest Order values lose out
            if (ordered.Count > SLOTS && !warnedOverflow)
            {
                Debug.Print($"HUD has {SLOTS} slots but got {ordered.Count} bars. Dropped: {string.Join(", ", ordered.Skip(SLOTS).Select(b => b.Label))}");
                warnedOverflow = true; // Show runs every frame, once is enough
            }

            HudBar[] next = new HudBar[SLOTS];
            for (int i = 0; i < Math.Min(SLOTS, ordered.Count); i++)
                next[i] = ordered[i];

            for (int i = 0; i < SLOTS; i++)
            {
                if (next[i] == shown[i]) continue; // records compare by value

                Apply(i, next[i]);
            }
        }

        /// <summary>
        /// Puts a bar in a slot, or hides the slot if hudBar is null.
        ///
        /// We run after SadConsole's update, so the controls get redrawn by hand here.
        /// Otherwise the render step composites the old picture and clears IsDirty before
        /// UpdateAndRedraw ever sees it, and the change only shows up when the mouse dirties it again.
        /// </summary>
        private void Apply(int slot, HudBar hudBar)
        {
            ProgressBar bar = bars[slot];
            Label label = labels[slot];

            shown[slot] = hudBar;
            Controls.IsDirty = true; // the host recomposites from the controls' surfaces on its next render

            if (hudBar is null)
            {
                bar.IsVisible = false;
                label.IsVisible = false;
                return;
            }

            bar.Progress = hudBar.Progress;
            bar.DisplayText = hudBar.Text;
            bar.BarColor = hudBar.BarColor;
            bar.DisplayTextColor = hudBar.TextColor ?? defaultTextColor;
            bar.IsVisible = true;

            label.DisplayText = hudBar.Label;
            label.TextColor = hudBar.LabelColor; // doesn't mark the label dirty by itself
            label.IsVisible = true;

            // UpdateAndRedraw bails unless IsDirty is set
            bar.IsDirty = true;
            label.IsDirty = true;
            bar.UpdateAndRedraw(TimeSpan.Zero);
            label.UpdateAndRedraw(TimeSpan.Zero);
        }
    }
}
