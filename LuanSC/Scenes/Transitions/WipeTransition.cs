using System;

namespace LuanSC.Scenes.Transitions;

/// <summary>
/// A solid edge sweeps across the screen from left to right, then carries on the same way to reveal the new scene.
/// </summary>
public sealed class WipeTransition : ISceneTransition
{
    private readonly Color colour;

    public TimeSpan Duration { get; }

    /// <param name="duration">Each half. Null is 250ms.</param>
    /// <param name="colour">Null is black.</param>
    public WipeTransition(TimeSpan? duration = null, Color? colour = null)
    {
        Duration = duration ?? TimeSpan.FromMilliseconds(250);
        this.colour = colour ?? Color.Black;
    }

    public void Draw(ScreenSurface overlay, float amount, bool coveringScreen)
    {
        ICellSurface cells = overlay.Surface;
        int covered = (int)MathF.Ceiling(amount * cells.Width);

        for (int x = 0; x < cells.Width; x++)
        {
            bool hidden = coveringScreen ? x < covered : x >= cells.Width - covered;

            for (int y = 0; y < cells.Height; y++)
                cells.SetBackground(x, y, hidden ? colour : Color.Transparent);
        }
    }
}
