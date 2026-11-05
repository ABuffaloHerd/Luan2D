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
    /// Magic sparkles twinkling at random over a diamond, the same cells SquareBlast covers at this radius.
    /// Each sparkle goes · * ☼ * · and its colour shades from color towards white, whitest at the ☼.
    /// </summary>
    /// <param name="origin">Arena cell at the centre of the diamond.</param>
    /// <param name="radius">Size of the diamond, |x| + |y| &lt;= radius.</param>
    /// <param name="color">The colour a sparkle starts and ends on. Defaults to dodger blue.</param>
    /// <param name="density">Share of cells that sparkle, 0 to 1. Each picked cell twinkles once.</param>
    /// <param name="frames">How long the whole effect runs, in frames. At least as long as one twinkle (5).</param>
    /// <param name="frameTime">How long each frame is held. Defaults to 60ms.</param>
    /// <param name="seed">Fixes which cells sparkle and when. Random every time if left out.</param>
    public static AnimatedEffect Sparkle(Point origin, int radius = 2, Color? color = null, float density = 0.5f, int frames = 12,
        TimeSpan? frameTime = null, int? seed = null)
    {
        Color c = color ?? Color.DodgerBlue;
        TimeSpan duration = frameTime ?? TimeSpan.FromMilliseconds(60);
        Random random = seed is int s ? new Random(s) : Random.Shared;

        // glyph numbers in the font, not characters: 250 is a middle dot, 15 is the little sun in Bisasam. Other fonts may differ
        char[] glyphs = [(char)250, '*', (char)15, '*', (char)250];
        float[] whiteness = [0f, 0.5f, 1f, 0.5f, 0f]; // how far each step shades from c towards white
        frames = Math.Max(frames, glyphs.Length);

        int size = radius * 2 + 1;
        AnimatedEffect sparkle = new("Sparkle", size, size)
        {
            Center = new Point(radius, radius),
            Cell = origin,
            Repeat = false
        };

        // pick which diamond cells sparkle and the frame each one starts on, so the twinkle always finishes in time
        List<(Point Offset, int Start)> sparks = new();
        foreach (List<Point> ring in Rings(radius, offset => Math.Abs(offset.X) + Math.Abs(offset.Y)))
            foreach (Point offset in ring)
                if (random.NextSingle() < density)
                    sparks.Add((offset, random.Next(frames - glyphs.Length + 1)));

        for (int f = 0; f < frames; f++)
        {
            ICellSurface frame = sparkle.CreateFrame();
            frame.DefaultBackground = Color.Transparent; // frames are opaque black otherwise
            frame.Clear();

            foreach ((Point offset, int start) in sparks)
            {
                int step = f - start;
                if (step < 0 || step >= glyphs.Length) continue;

                float w = whiteness[step];
                Color shade = new((int)(c.R + (255 - c.R) * w), (int)(c.G + (255 - c.G) * w), (int)(c.B + (255 - c.B) * w));
                frame.SetCellAppearance(radius + offset.X, radius + offset.Y, new ColoredGlyph(shade, Color.Transparent, glyphs[step]));
            }
        }

        sparkle.AnimationDuration = duration * frames;
        sparkle.Start();
        return sparkle;
    }

    /// <summary>
    /// Green + and hearts flashing over a circle, the same cells Pattern.Circle covers at this radius.
    /// The two glyphs sit in a checkerboard and swap places every flash, with each flash dimmer than the last.
    /// </summary>
    /// <param name="origin">Arena cell at the centre of the circle.</param>
    /// <param name="radius">Size of the circle. 0 is just the origin, 1 includes the diagonals.</param>
    /// <param name="color">Colour of the first flash. Defaults to lime green.</param>
    /// <param name="flashes">How many times it flashes.</param>
    /// <param name="frameTime">How long each flash and each gap is held. Defaults to 80ms.</param>
    public static AnimatedEffect HealFlash(Point origin, int radius = 1, Color? color = null, int flashes = 3, TimeSpan? frameTime = null)
    {
        radius = Math.Max(radius, 0);
        flashes = Math.Max(flashes, 1);
        Color c = color ?? Color.LimeGreen;
        TimeSpan duration = frameTime ?? TimeSpan.FromMilliseconds(80);

        const char Heart = (char)3; // glyph number in the font, not a character
        const float LastBrightness = 0.5f;

        int size = radius * 2 + 1;
        AnimatedEffect flash = new("Heal Flash", size, size)
        {
            Center = new Point(radius, radius),
            Cell = origin,
            Repeat = false
        };

        List<Point>[] rings = Rings(radius, Pattern.CircleDistance);

        for (int f = 0; f < flashes; f++)
        {
            ICellSurface frame = flash.CreateFrame();
            frame.DefaultBackground = Color.Transparent; // frames are opaque black otherwise
            frame.Clear();

            // dims the colour itself rather than its alpha, so the brightness doesn't depend on how alpha blends
            float brightness = flashes == 1 ? 1f : 1f - (1f - LastBrightness) * f / (flashes - 1);
            Color shade = new((int)(c.R * brightness), (int)(c.G * brightness), (int)(c.B * brightness));

            foreach (List<Point> ring in rings)
            {
                foreach (Point offset in ring)
                {
                    int x = radius + offset.X;
                    int y = radius + offset.Y;
                    char glyph = (offset.X + offset.Y + f) % 2 == 0 ? Heart : '+';
                    frame.SetCellAppearance(x, y, new ColoredGlyph(shade, Color.Transparent, glyph));
                }
            }

            // the gap that makes it a flash
            ICellSurface gap = flash.CreateFrame();
            gap.DefaultBackground = Color.Transparent;
            gap.Clear();
        }

        flash.AnimationDuration = duration * flashes * 2;
        flash.Start();
        return flash;
    }

    /// <summary>
    /// One arrow flying a cell per frame from one arena cell to another, pointing the way it's going.
    /// Starts on the first cell after from, so it comes out of the shooter instead of covering them, and finishes on to.
    /// Off-axis shots point along whichever axis they travel furthest, exact diagonals point left or right.
    /// </summary>
    /// <param name="from">Arena cell it's fired from.</param>
    /// <param name="to">Arena cell it lands on. If it's the same as from, it just shows there for a frame.</param>
    /// <param name="color">Colour of the arrow. Defaults to white.</param>
    /// <param name="frameTime">How long it's held on each cell. Defaults to 30ms.</param>
    public static AnimatedEffect Projectile(Point from, Point to, Color? color = null, TimeSpan? frameTime = null)
    {
        Color c = color ?? Color.White;
        TimeSpan duration = frameTime ?? TimeSpan.FromMilliseconds(30);

        int dx = to.X - from.X;
        int dy = to.Y - from.Y;
        int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

        // glyph numbers in the font, not characters. Up is -y in the arena
        char glyph = Math.Abs(dx) >= Math.Abs(dy)
            ? (dx >= 0 ? (char)26 : (char)27)
            : (dy < 0 ? (char)24 : (char)25);

        // one box around the whole flight, with from at its Center so the effect lines up with the arena
        Point corner = new(Math.Min(from.X, to.X), Math.Min(from.Y, to.Y));
        AnimatedEffect projectile = new("Projectile", Math.Abs(dx) + 1, Math.Abs(dy) + 1)
        {
            Center = from - corner,
            Cell = from,
            Repeat = false
        };

        int frames = Math.Max(steps, 1);
        for (int i = 1; i <= frames; i++)
        {
            ICellSurface frame = projectile.CreateFrame();
            frame.DefaultBackground = Color.Transparent; // frames are opaque black otherwise
            frame.Clear();

            // straight line, rounded to the nearest cell
            float progress = steps == 0 ? 0f : (float)i / steps;
            Point cell = new(from.X + (int)MathF.Round(dx * progress), from.Y + (int)MathF.Round(dy * progress));

            frame.SetCellAppearance(cell.X - corner.X, cell.Y - corner.Y, new ColoredGlyph(c, Color.Transparent, glyph));
        }

        projectile.AnimationDuration = duration * frames;
        projectile.Start();
        return projectile;
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
