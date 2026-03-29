using LuanSC.Data.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Objects;

public class Wall : GameObject
{
    public static Wall GenerateWall()
    {
        ColoredGlyph cgb = new ColoredGlyph()
        {
            Glyph = (char)219,
            Foreground = Color.White
        };

        return new Wall(cgb, 0);
    }

    public static Wall GenerateWall(Color c)
    {
        ColoredGlyph cgb = new ColoredGlyph()
        {
            Glyph = (char)219,
            Foreground = c
        };

        return new Wall(cgb, 0);
    }

    private Wall(ColoredGlyphBase coloredGlyphBase, int zIndex) : base(coloredGlyphBase, zIndex)
    {
        Components.Add(new CollisionComponent());
    }
}
