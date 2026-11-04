using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes;

internal class GlyphViewerScene : Scene
{
    private char selectedGlyph = (char)0;
    private ScreenSurface glyphDisplay;
    private ScreenSurface glyphHighlight;
    private ScreenSurface info;
    public GlyphViewerScene(SceneManager manager) : base(manager)
    {
        glyphDisplay = new(16, 16);
        glyphHighlight = new(1, 1);
        info = new(32, 32);

        glyphHighlight.FontSize = new(16 * 8, 16 * 8);
        glyphDisplay.FontSize = new(16 * 2, 16 * 2);

        glyphDisplay.Position = new(0, 0);
        glyphHighlight.Position = new(45 / 8, 0);
        info.Position = new(0, 32);

        // display all glyphs in the font
        for (int x = 0; x < glyphDisplay.Width; x++)
        {
            for (int y = 0; y < glyphDisplay.Height; y++)
            {
                int index = y * glyphDisplay.Width + x;
                if (index >= 256) break;
                glyphDisplay.SetGlyph(x, y, (char)index);
            }
        }
        glyphHighlight.SetGlyph(0, 0, (char)0);

        glyphDisplay.MouseButtonClicked += GlyphHighlight_MouseButtonClicked;


        Children.Add(glyphDisplay);
        Children.Add(glyphHighlight);
        Children.Add(info);
    }

    private void GlyphHighlight_MouseButtonClicked(object? sender, MouseScreenObjectState e)
    {
        // get the glyph under the highlight
        Point index = e.SurfaceCellPosition;
        selectedGlyph = (char)(index.Y * glyphDisplay.Width + index.X);
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.IsKeyPressed(Keys.Escape))
        {
            GameRoot.sceneManger.ChangeScene(manager => new Menu(manager));
            return true;
        }
        return base.ProcessKeyboard(keyboard);
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        glyphHighlight.SetGlyph(0, 0, selectedGlyph);
        info.Clear();
        info.Print(0, 0, $"Glyph: {selectedGlyph} (0x{(int)selectedGlyph:X2})");
    }
}
