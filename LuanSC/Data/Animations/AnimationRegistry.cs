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
    /// Diagonal slashes through the origin, one after another. Strokes alternate '\' then '/', so 2 makes an X and
    /// a third re-cuts the first. Cells where both diagonals are showing draw as 'X'.
    /// Finished strokes stay dimly on screen until the last one is done, then everything fades out.
    /// </summary>
    /// <param name="origin">Arena cell the slashes cross at.</param>
    /// <param name="length">Cells per stroke. Odd lengths centre exactly, even ones sit half a cell down and right.</param>
    /// <param name="color">Colour of the strokes. Defaults to white.</param>
    /// <param name="count">How many strokes.</param>
    /// <param name="sweep">False flashes each stroke in whole. True draws it a cell per frame with a fading trail.</param>
    /// <param name="frameTime">How long each frame is held. Defaults to 60ms.</param>
    public static AnimatedEffect Slash(Point origin, int length = 3, Color? color = null, int count = 1, bool sweep = false, TimeSpan? frameTime = null)
    {
        length = Math.Max(length, 1);
        count = Math.Max(count, 1);
        Color c = color ?? Color.White;
        TimeSpan duration = frameTime ?? TimeSpan.FromMilliseconds(60);

        const byte Full = 255, Trail = 170, Settled = 90; // brightness of the head, just behind it, and done strokes
        const int Back = 1, Forward = 2;                  // '\' and '/' as bits, both set is 'X'

        // one square big enough for a whole stroke, with the origin in the middle
        int half = length / 2;
        int size = half * 2 + 1;
        AnimatedEffect slash = new("Slash", size, size)
        {
            Center = new Point(half, half),
            Cell = origin,
            Repeat = false
        };

        // every frame is a map of offset -> which diagonals are on it and how bright
        List<Dictionary<Point, (int Mask, byte Alpha)>> frames = new();
        Dictionary<Point, int> settled = new();

        void Mark(Dictionary<Point, (int Mask, byte Alpha)> frame, Point offset, int mask, byte alpha)
        {
            frame[offset] = frame.TryGetValue(offset, out var cell)
                ? (cell.Mask | mask, Math.Max(cell.Alpha, alpha))
                : (mask, alpha);
        }

        Dictionary<Point, (int Mask, byte Alpha)> SettledFrame(byte alpha)
        {
            Dictionary<Point, (int Mask, byte Alpha)> frame = new();
            foreach ((Point offset, int mask) in settled)
                Mark(frame, offset, mask, alpha);
            return frame;
        }

        for (int s = 0; s < count; s++)
        {
            int mask = s % 2 == 0 ? Back : Forward;

            // top-left to bottom-right for '\', top-right to bottom-left for '/'
            List<Point> cells = new();
            for (int i = 0; i < length; i++)
            {
                int k = i - (length - 1) / 2;
                cells.Add(mask == Back ? new Point(k, k) : new Point(-k, k));
            }

            // sweep: head moves one cell per frame. Flash: whole stroke bright, then a beat dimmer
            int steps = sweep ? length : 2;
            for (int step = 0; step < steps; step++)
            {
                Dictionary<Point, (int Mask, byte Alpha)> frame = SettledFrame(Settled);

                if (sweep)
                {
                    for (int i = 0; i <= step; i++)
                        Mark(frame, cells[i], mask, i == step ? Full : i == step - 1 ? Trail : Settled);
                }
                else
                {
                    foreach (Point offset in cells)
                        Mark(frame, offset, mask, step == 0 ? Full : Trail);
                }

                frames.Add(frame);
            }

            foreach (Point offset in cells)
                settled[offset] = settled.TryGetValue(offset, out int m) ? m | mask : mask;
        }

        // fade everything out together
        frames.Add(SettledFrame(Settled));
        frames.Add(SettledFrame(Settled / 3));

        foreach (Dictionary<Point, (int Mask, byte Alpha)> cells in frames)
        {
            ICellSurface frame = slash.CreateFrame();
            frame.DefaultBackground = Color.Transparent; // frames are opaque black otherwise
            frame.Clear();

            foreach ((Point offset, (int mask, byte alpha)) in cells)
            {
                char glyph = mask == (Back | Forward) ? 'X' : mask == Back ? '\\' : '/';
                frame.SetCellAppearance(half + offset.X, half + offset.Y, new ColoredGlyph(new Color(c.R, c.G, c.B, alpha), Color.Transparent, glyph));
            }
        }

        slash.AnimationDuration = duration * frames.Count;
        slash.Start();
        return slash;
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
            Cell = origin,
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
