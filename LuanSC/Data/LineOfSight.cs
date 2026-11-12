using LuanSC.Data.Components;
using LuanSC.Objects;

namespace LuanSC.Data;

/// <summary>
/// What blocks shots and blasts, and whether one cell can see another. Shared by the attacks so they all agree.
///   HashSet&lt;Point&gt; blockers = LineOfSight.Blockers(context.Objects);
///   if (LineOfSight.HasLineOfSight(owner.Position, target.Position, blockers)) ...
/// </summary>
public static class LineOfSight
{
    /// <summary>
    /// Walls, and furniture that's still standing.
    /// </summary>
    public static bool IsBlocker(GameObject obj)
        => obj is Wall || (obj is Furniture && obj.GetComponent<HPComponent>() is { IsAlive: true });

    /// <summary>
    /// The cells of every blocker. Work this out once before an attack deals damage,
    /// so furniture it breaks still shields what's behind it.
    /// </summary>
    public static HashSet<Point> Blockers(IEnumerable<GameObject> objects)
        => objects.Where(IsBlocker).Select(obj => obj.Position).ToHashSet();

    /// <summary>
    /// False if a blocker sits on the straight line between the two. The ends are left out, so a blocker can still be seen and hit itself.
    /// Same rounded line as AnimationRegistry.Projectile, so what a shot passes through and what blocks agree.
    /// </summary>
    public static bool HasLineOfSight(Point from, Point to, HashSet<Point> blockers)
    {
        int dx = to.X - from.X;
        int dy = to.Y - from.Y;
        int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

        for (int i = 1; i < steps; i++)
        {
            float progress = (float)i / steps;
            Point cell = new(from.X + (int)MathF.Round(dx * progress), from.Y + (int)MathF.Round(dy * progress));
            if (blockers.Contains(cell)) return false;
        }

        return true;
    }
}
