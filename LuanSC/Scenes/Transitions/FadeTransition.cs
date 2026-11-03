using System;

namespace LuanSC.Scenes.Transitions;

/// <summary>
/// Fades to a colour and back. Black unless told otherwise.
/// </summary>
public sealed class FadeTransition : ISceneTransition
{
    private readonly Color colour;

    public TimeSpan Duration { get; }

    /// <param name="duration">Each half. Null is 250ms.</param>
    /// <param name="colour">Null is black.</param>
    public FadeTransition(TimeSpan? duration = null, Color? colour = null)
    {
        Duration = duration ?? TimeSpan.FromMilliseconds(250);
        this.colour = colour ?? Color.Black;
    }

    public void Draw(ScreenSurface overlay, float amount, bool coveringScreen)
    {
        overlay.Tint = new Color(colour.R, colour.G, colour.B, (int)(amount * 255));
    }
}
