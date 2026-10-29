using SadConsole.Components;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes;

/// <summary>
/// Middle drag pans the view, the scroll wheel zooms. Add it to the arena surface.
/// </summary>
/// <remarks>
/// Modelled on SadConsole.Extended's MouseDragViewPort, which can't be used here as is:
///  - it only drags with the left button, which is the targeting click, and the button is hardcoded.
///  - it measures the drag in WorldCellPosition, which is in unzoomed cells. At 2x zoom the arena
///    would slide twice as fast as the mouse. This measures in pixels and divides by the zoomed font size.
/// </remarks>
public class ArenaCameraMouse : MouseConsoleComponent
{
    /// <summary>
    /// The player moved the view by hand. The scene stops following the current unit.
    /// </summary>
    public event Action? Dragged;

    /// <summary>
    /// Wheel turned over the arena. +1 is towards you (zoom in), -1 is away from you (zoom out).
    /// </summary>
    public event Action<int>? Scrolled;

    private bool isDragging;
    private Point grabPixel;
    private Point grabViewPosition;
    private bool previousExclusiveMouse;

    public override void OnAdded(IScreenObject host)
    {
        if (host is not IScreenSurface)
            throw new Exception($"{nameof(ArenaCameraMouse)} only works on an IScreenSurface.");
    }

    public override void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled)
    {
        IScreenSurface surface = (IScreenSurface)host;
        handled = false;

        if (isDragging)
        {
            if (!state.Mouse.MiddleButtonDown)
            {
                isDragging = false;
                surface.IsExclusiveMouse = previousExclusiveMouse;
                handled = true;
                return;
            }

            // grab and pull: the arena moves with the mouse, so the view moves the other way
            Point pixels = grabPixel - state.Mouse.ScreenPosition;
            Point cells = new(pixels.X / surface.FontSize.X, pixels.Y / surface.FontSize.Y);
            Point viewPosition = ClampView(surface.Surface, grabViewPosition + cells);

            if (surface.Surface.ViewPosition != viewPosition)
            {
                surface.Surface.ViewPosition = viewPosition;
                Dragged?.Invoke();
            }

            handled = true; // no cursor moves or clicks while dragging
            return;
        }

        if (!state.IsOnScreenObject) return;

        int wheel = state.Mouse.ScrollWheelValueChange;
        if (wheel != 0)
        {
            Scrolled?.Invoke(-Math.Sign(wheel)); // MonoGame reports away from you as positive
            handled = true;
            return;
        }

        // nothing to drag when the whole arena fits
        if (!surface.Surface.IsScrollable) return;

        // only on the frame the button goes down, so holding it while moving onto the arena doesn't grab
        if (state.Mouse.MiddleButtonDown && state.Mouse.MiddleButtonDownDuration == TimeSpan.Zero)
        {
            isDragging = true;
            grabPixel = state.Mouse.ScreenPosition;
            grabViewPosition = surface.Surface.ViewPosition;

            // keep getting the mouse if it leaves the arena mid drag
            previousExclusiveMouse = surface.IsExclusiveMouse;
            surface.IsExclusiveMouse = true;

            handled = true;
        }
    }

    /// <summary>
    /// Keeps the view inside the surface.
    /// </summary>
    public static Point ClampView(ICellSurface surface, Point viewPosition)
    {
        return new Point(
            Math.Clamp(viewPosition.X, 0, surface.Width - surface.ViewWidth),
            Math.Clamp(viewPosition.Y, 0, surface.Height - surface.ViewHeight));
    }
}
