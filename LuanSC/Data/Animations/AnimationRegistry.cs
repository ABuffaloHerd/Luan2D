using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Animations;

/// <summary>
/// Animation factories. Add the result to the arena surface's Children and it plays out and removes itself.
/// Every call builds a fresh one, so the same animation can play in several places at once.
/// </summary>
public static class AnimationRegistry
{
    /// <summary>
    /// A diamond shaped ring (manhattan distance) that expands from the origin out to radius, leaving a fading trail.
    /// </summary>
    /// <param name="origin">Arena cell the blast starts from.</param>
    /// <param name="radius">How far the ring travels.</param>
    /// <param name="color">Colour of the ring. Defaults to orange.</param>
    /// <param name="frameTime">How long each frame is held. Defaults to 60ms.</param>
    /// <param name="glyphs">Head of the ring first, then the trail. The trail is as long as this minus one.</param>
    public static AnimatedEffect SquareBlast(Point origin, int radius = 3, Color? color = null, TimeSpan? frameTime = null, char[] glyphs = null)
    {
        return RingBlast("Square Blast", origin, radius, color, frameTime, glyphs,
            offset => Math.Abs(offset.X) + Math.Abs(offset.Y));
    }

    /// <summary>
    /// A round ring (euclidean distance) that expands from the origin out to radius, leaving a fading trail.
    /// Looks round because our fonts have square cells.
    /// </summary>
    /// <inheritdoc cref="SquareBlast"/>
    public static AnimatedEffect CircleBlast(Point origin, int radius = 3, Color? color = null, TimeSpan? frameTime = null, char[] glyphs = null)
    {
        // same distance as Pattern.Circle, so a blast lines up with a circle area exactly
        return RingBlast("Circle Blast", origin, radius, color, frameTime, glyphs, Pattern.CircleDistance);
    }

    /// <summary>
    /// Shared by the blasts. A cell is on ring n when distance(offset) == n, so the shape is all in the distance function.
    /// </summary>
    private static AnimatedEffect RingBlast(string name, Point origin, int radius, Color? color, TimeSpan? frameTime, char[] glyphs, Func<Point, int> distance)
    {
        Color c = color ?? Color.Orange;
        TimeSpan duration = frameTime ?? TimeSpan.FromMilliseconds(60);
        glyphs ??= ['*', '+', '.'];

        // one square big enough for the whole blast, with the origin in the middle
        int size = radius * 2 + 1;
        AnimatedEffect blast = new(name, size, size)
        {
            Center = new Point(radius, radius),
            Position = origin,
            Repeat = false
        };

        List<Point>[] rings = Rings(radius, distance);

        // the head moves one ring per frame, keep going until the tail is past the edge too
        int steps = radius + glyphs.Length;
        for (int step = 0; step < steps; step++)
        {
            ICellSurface frame = blast.CreateFrame();
            frame.DefaultBackground = Color.Transparent; // frames are opaque black otherwise
            frame.Clear();

            // draw the tail first so the head wins if they ever overlap
            for (int t = glyphs.Length - 1; t >= 0; t--)
            {
                int ring = step - t;
                if (ring < 0 || ring > radius) continue;

                // fade along the trail: full, then less and less
                byte alpha = (byte)(255 * (glyphs.Length - t) / glyphs.Length);
                ColoredGlyph glyph = new(new Color(c.R, c.G, c.B, alpha), Color.Transparent, glyphs[t]);

                foreach (Point offset in rings[ring])
                    frame.SetCellAppearance(radius + offset.X, radius + offset.Y, glyph);
            }
        }

        blast.AnimationDuration = duration * steps;
        blast.Start();
        return blast;
    }

    /// <summary>
    /// Sorts every offset in the square around the centre into rings by distance. Anything past radius is dropped.
    /// </summary>
    private static List<Point>[] Rings(int radius, Func<Point, int> distance)
    {
        List<Point>[] rings = new List<Point>[radius + 1];
        for (int i = 0; i <= radius; i++)
            rings[i] = new List<Point>();

        for (int dy = -radius; dy <= radius; dy++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                Point offset = new(dx, dy);
                int d = distance(offset);

                if (d <= radius)
                    rings[d].Add(offset);
            }
        }

        return rings;
    }
}
