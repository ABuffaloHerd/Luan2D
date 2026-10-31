using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes;

/// <summary>
/// Arena zoom and the camera that follows the current unit.
/// </summary>
/// <remarks>
/// Zooming scales the font by a whole number and shrinks the view by the same amount, so the arena keeps
/// roughly the same size on screen and just shows fewer, bigger cells. Whole numbers keep the glyphs crisp.
///
/// The view is never bigger than GameSettings.ARENA_VIEW_SIZE, so big arenas scroll even at zoom 1.
/// Once the view is smaller than the arena it scrolls (ViewPosition), so anything that turns a screen position
/// into an arena cell has to add ViewPosition. Layers and entities already do this themselves, the mouse
/// does too as long as you read state.CellPosition and not state.SurfaceCellPosition, and AnimatedEffect
/// handles it in its own Update.
///
/// Controls: + and - or the scroll wheel zoom, middle drag pans (see ArenaCameraMouse), ~ recentres.
/// Panning by hand stops the camera following until the turn changes, the current unit moves, or ~.
/// </remarks>
public partial class CombatScene
{
    private static readonly int[] ZoomLevels = [1, 2, 3];
    private int zoomIndex = 0;

    // the arena's font size and view size at zoom 1
    private Point baseFontSize;
    private Point baseViewSize;

    private int ZoomFactor => ZoomLevels[zoomIndex];

    // false while the player is looking around by hand
    private bool cameraFollowing = true;

    // where the current unit was last frame, so moving it brings the camera back
    private Point? lastFocus;

    private void HookCamera()
    {
        baseFontSize = surface.FontSize;
        baseViewSize = new(surface.ViewWidth, surface.ViewHeight); // not the arena size, big arenas scroll at zoom 1 too

        ArenaCameraMouse cameraMouse = new();
        cameraMouse.Dragged += () => cameraFollowing = false;
        cameraMouse.Scrolled += Zoom;
        surface.SadComponents.Add(cameraMouse);
    }

    /// <summary>
    /// step +1 zooms in, -1 zooms out. Stops at the ends of ZoomLevels.
    /// Keeps the middle of the view where it was, the camera recentres afterwards if it's following.
    /// </summary>
    private void Zoom(int step)
    {
        int next = Math.Clamp(zoomIndex + step, 0, ZoomLevels.Length - 1);
        if (next == zoomIndex) return;
        zoomIndex = next;

        Point middle = surface.ViewPosition + new Point(surface.ViewWidth / 2, surface.ViewHeight / 2);

        // back to the corner first, a bigger view at the old position could hang off the edge
        surface.ViewPosition = Point.Zero;

        surface.FontSize = baseFontSize * ZoomFactor;
        surface.ViewWidth = baseViewSize.X / ZoomFactor;
        surface.ViewHeight = baseViewSize.Y / ZoomFactor;

        CenterView(middle);
        FollowCamera();
    }

    /// <summary>
    /// Snaps back to the current unit and follows it again. ~ does this.
    /// </summary>
    private void RecenterCamera()
    {
        cameraFollowing = true;
        FollowCamera();
    }

    /// <summary>
    /// Centres the view on the current unit while the camera is following.
    /// Cheap enough to call every frame, it only touches the view when it actually moves.
    /// </summary>
    private void FollowCamera()
    {
        if (currentControlledGameObject is null) return;

        Point focus = currentControlledGameObject.Position;

        // the unit moved, look at it again
        if (lastFocus is not null && lastFocus != focus)
            cameraFollowing = true;
        lastFocus = focus;

        if (cameraFollowing)
            CenterView(focus);
    }

    /// <summary>
    /// Puts cell in the middle of the view, without showing anything past the edge of the arena.
    /// </summary>
    private void CenterView(Point cell)
    {
        Point viewPosition = ArenaCameraMouse.ClampView(surface.Surface,
            new Point(cell.X - surface.ViewWidth / 2, cell.Y - surface.ViewHeight / 2));

        if (surface.ViewPosition != viewPosition)
            surface.ViewPosition = viewPosition;
    }
}
