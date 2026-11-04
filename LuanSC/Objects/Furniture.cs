using LuanSC.Data.Components;

namespace LuanSC.Objects;

/// <summary>
/// A wall you can break. Blocks movement and shots until its HP runs out.
/// No speed so it never takes a turn, no team so it doesn't count towards winning or losing.
/// </summary>
public class Furniture : GameObject
{
    public Furniture(ColoredGlyphBase coloredGlyphBase, int zIndex) : base(coloredGlyphBase, zIndex)
    {
        AddComponent(new HPComponent(50));
        AddComponent(new CollisionComponent());
        UnitType = Data.UnitType.FURNITURE;
    }
}
