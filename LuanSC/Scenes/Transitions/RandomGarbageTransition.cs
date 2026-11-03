using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes.Transitions;

/// <summary>
/// Random flickering characters appear all over the screen until they cover it, then disappear in the reverse order.
/// The pattern is kept for the life of the instance, so pass a new one to each ChangeScene to get a different one.
/// </summary>
public class RandomGarbageTransition : ISceneTransition
{
    // the amount each cell gets covered at. All under 1 and none under 0, so amount 1 covers everything and amount 0 covers nothing
    private float[,] thresholds;

    public TimeSpan Duration { get; }

    public RandomGarbageTransition(TimeSpan? duration = null)
    {
        Duration = duration ?? TimeSpan.FromMilliseconds(250);
    }

    public void Draw(ScreenSurface overlay, float amount, bool coveringScreen)
    {
        ICellSurface cells = overlay.Surface;
        int glyphs = overlay.Font.TotalGlyphs;

        if (thresholds is null)
        {
            thresholds = new float[cells.Width, cells.Height];

            for (int x = 0; x < cells.Width; x++)
                for (int y = 0; y < cells.Height; y++)
                    thresholds[x, y] = Random.Shared.NextSingle();
        }

        for (int x = 0; x < cells.Width; x++)
        {
            for (int y = 0; y < cells.Height; y++)
            {
                if (thresholds[x, y] < amount)
                {
                    // solid background, the scene can't show through the gaps in the glyphs
                    Color colour = new(Random.Shared.Next(256), Random.Shared.Next(256), Random.Shared.Next(256));
                    Color bgcolour = new(Random.Shared.Next(256), Random.Shared.Next(256), Random.Shared.Next(256));
                    cells.SetGlyph(x, y, Random.Shared.Next(glyphs), colour, bgcolour);
                }
                else
                {
                    cells.SetGlyph(x, y, 0, Color.Transparent, Color.Transparent);
                }
            }
        }
    }
}
