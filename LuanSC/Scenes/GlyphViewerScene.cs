using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LuanSC.Scenes;

internal class GlyphViewerScene : Scene
{
    private sealed record NamedColour(string Name, Color Colour);

    // every named colour on the Color struct, A to Z. The list box rows are in the same order
    private static readonly List<NamedColour> colours = typeof(Color)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(field => field.FieldType == typeof(Color))
        .Select(field => new NamedColour(field.Name, (Color)field.GetValue(null)!))
        .OrderBy(colour => colour.Name)
        .ToList();

    private char selectedGlyph = (char)0;
    private ScreenSurface glyphDisplay;
    private ScreenSurface glyphHighlight;
    private ScreenSurface info;
    private ControlsConsole controls;
    private RadioButton fgButton;
    private RadioButton bgButton;
    private ListBox colourList;
    private int fgIndex = colours.FindIndex(colour => colour.Name == nameof(Color.White));
    private int bgIndex = colours.FindIndex(colour => colour.Name == nameof(Color.Black));
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

        controls = new(32, GameSettings.GAME_HEIGHT);
        controls.Position = new(GameSettings.GAME_WIDTH - 32, 0);

        // a click would take the keyboard off the scene and Esc would stop working
        controls.UseKeyboard = false;
        controls.FocusOnMouseClick = false;

        // setup controls to change fg and bg. The buttons pick which one the list sets
        fgButton = new(controls.Width, 1) { Position = new(0, 0), GroupName = "target" };
        bgButton = new(controls.Width, 1) { Position = new(0, 1), GroupName = "target" };

        colourList = new(controls.Width, controls.Height - 3)
        {
            Position = new(0, 3),
            DrawBorder = true,
        };

        foreach (NamedColour colour in colours)
        {
            colourList.Items.Add(new ColoredString(new string((char)219, 2), colour.Colour, Color.Transparent)
                + new ColoredString($" {colour.Name}", Color.White, Color.Transparent));
        }

        controls.Controls.Add(fgButton);
        controls.Controls.Add(bgButton);
        controls.Controls.Add(colourList);

        colourList.SelectedItemChanged += (s, e) => ColourPicked();
        fgButton.IsSelectedChanged += (s, e) => ShowTargetColour();
        bgButton.IsSelectedChanged += (s, e) => ShowTargetColour();

        UpdateButtonText();
        fgButton.IsSelected = true;

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
        Children.Add(controls);
    }

    private void ColourPicked()
    {
        if (colourList.SelectedIndex < 0) return;

        if (fgButton.IsSelected)
            fgIndex = colourList.SelectedIndex;
        else
            bgIndex = colourList.SelectedIndex;

        UpdateButtonText();
    }

    // moves the list to the colour already set on whichever of fg and bg was just chosen
    private void ShowTargetColour()
    {
        colourList.SelectedIndex = fgButton.IsSelected ? fgIndex : bgIndex;
        colourList.ScrollToSelectedItem();
    }

    private void UpdateButtonText()
    {
        fgButton.Text = $"FG: {colours[fgIndex].Name}";
        bgButton.Text = $"BG: {colours[bgIndex].Name}";
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
            GameRoot.sceneManger.PopScene();
            return true;
        }
        return base.ProcessKeyboard(keyboard);
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        glyphHighlight.SetGlyph(0, 0, selectedGlyph, colours[fgIndex].Colour, colours[bgIndex].Colour);
        info.Clear();
        info.Print(0, 0, $"Glyph: {selectedGlyph} (0x{(int)selectedGlyph:X2})");
    }
}
