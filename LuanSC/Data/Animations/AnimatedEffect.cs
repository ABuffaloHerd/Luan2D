using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Animations;

/// <summary>
/// An AnimatedScreenObject that looks after itself. Add it to the arena surface's Children and it plays out on its own,
/// like a Blinker on a cell. Build these with AnimationRegistry.
///
/// Children render after their parent's renderer, so this draws over every layer AND every entity.
/// Set Cell, not Position. Cell is the arena cell the effect plays on, and Center is the frame cell that lands on it.
/// </summary>
public class AnimatedEffect : AnimatedScreenObject
{
    /// <summary>
    /// The arena cell the effect plays on. Position is worked out from this every update,
    /// because children are placed relative to what's on screen and the arena view scrolls when zoomed in.
    /// </summary>
    public Point Cell { get; set; }

    /// <summary>
    /// Detach from the parent once the last frame has played.
    /// </summary>
    public bool RemoveOnFinished { get; set; } = true;

    public AnimatedEffect(string name, int width, int height) : base(name, width, height)
    {
        AnimationStateChanged += OnAnimationStateChanged;
    }

    public override void Update(TimeSpan delta)
    {
        MatchParentFont();
        FollowParentView();
        base.Update(delta);
    }

    // line up straight away, or the first frame can draw at the wrong spot if the parent already updated this frame
    protected override void OnParentChanged(IScreenObject? oldParent, IScreenObject? newParent)
    {
        base.OnParentChanged(oldParent, newParent);
        MatchParentFont();
        FollowParentView();
    }

    private void FollowParentView()
    {
        Point viewPosition = Parent is IScreenSurface parent ? parent.Surface.ViewPosition : Point.Zero;
        Point position = Cell - viewPosition;

        if (Position != position)
            Position = position;
    }

    /// <summary>
    /// Follow the arena's zoom. The font size is fixed at construction otherwise and the effect drifts off its cell.
    /// </summary>
    private void MatchParentFont()
    {
        if (Parent is not IScreenSurface parent) return;

        Font = parent.Font; // no-op if it's the same font

        if (FontSize != parent.FontSize)
        {
            FontSize = parent.FontSize;
            UpdateAbsolutePosition(); // the FontSize setter doesn't do this itself
        }
    }

    private void OnAnimationStateChanged(object sender, AnimationStateChangedEventArgs e)
    {
        // the parent updates a copy of its children, so leaving mid update is fine
        if (e.NewState == AnimationState.Finished && RemoveOnFinished)
            Parent = null;
    }
}
