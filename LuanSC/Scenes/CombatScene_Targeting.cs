using LuanSC.Data;
using LuanSC.Data.Components;
using LuanSC.Data.Spells;
using LuanSC.Objects;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Text;
using Direction = LuanSC.Data.Direction;

namespace LuanSC.Scenes;

/// <summary>
/// The arena cursor and spell targeting.
/// The cursor follows the mouse all the time. Subscribe to CursorMoved / CursorClicked for things like inspecting objects.
/// </summary>
public partial class CombatScene
{
    // overlay colours, translucent so the objects underneath stay readable
    private static readonly Color RANGE_COLOR = new(0, 128, 255, 50);
    private static readonly Color AREA_COLOR = new(0, 255, 0, 128);         // in range, 0.5 alpha
    private static readonly Color AREA_INVALID_COLOR = new(255, 0, 0, 204); // out of range, 0.8 alpha

    /// <summary>
    /// Arena cell under the cursor.
    /// </summary>
    public Point CursorPosition { get; private set; }

    public event Action<Point> CursorMoved;
    public event Action<Point> CursorClicked;

    public bool IsTargeting => targetingSpell is not null;

    private ISpell targetingSpell; // non-null while picking a target
    private GameObject targetingCaster;

    public IEnumerable<GameObject> ObjectsAt(Point position) => gameObjects.Where(obj => obj.Position == position);

    private void HookCursor()
    {
        surface.MouseMove += (sender, state) => MoveCursor(state.CellPosition);

        surface.MouseButtonClicked += (sender, state) =>
        {
            if (!state.Mouse.LeftClicked) return;

            MoveCursor(state.CellPosition);
            CursorClicked?.Invoke(CursorPosition);
        };

        CursorClicked += _ =>
        {
            if (IsTargeting) ConfirmTarget();
        };
    }

    private void MoveCursor(Point position)
    {
        position = new(Math.Clamp(position.X, 0, surface.Width - 1), Math.Clamp(position.Y, 0, surface.Height - 1));
        if (position == CursorPosition) return;

        CursorPosition = position;
        CursorMoved?.Invoke(position);
    }

    /// <summary>
    /// Casts straight away for FACING spells, starts targeting for CURSOR spells.
    /// </summary>
    public void Cast(GameObject caster, ISpell spell)
    {
        if (!spell.CanCast(caster))
        {
            GameEvents.Publish(new CombatEvent.GenericMessage($"{caster.Name} can't cast {spell.Name} right now."));
            return;
        }

        switch (spell.Targeting)
        {
            case Targeting.FACING:
                Execute(caster, spell.CreateAction(caster, caster.Position + caster.Direction.ToVector()));
                break;

            case Targeting.CURSOR:
                targetingSpell = spell;
                targetingCaster = caster;
                MoveCursor(caster.Position);
                fightFeed.AddLine($"Pick a target for {spell.Name}. Click or Enter to cast, Esc to cancel.");
                break;
        }
    }

    private bool InTargetRange(Point target)
    {
        if (!IsTargeting) return false;
        if (target.X < 0 || target.Y < 0 || target.X >= surface.Width || target.Y >= surface.Height) return false;

        return Distance(targetingCaster.Position, target) <= targetingSpell.Range;
    }

    private void ConfirmTarget()
    {
        if (!InTargetRange(CursorPosition))
        {
            fightFeed.AddLine($"Out of range. {targetingSpell.Name} reaches {targetingSpell.Range} cells.", Color.Red);
            return;
        }

        ISpell spell = targetingSpell;
        GameObject caster = targetingCaster;
        CancelTargeting();

        Execute(caster, spell.CreateAction(caster, CursorPosition));
    }

    private void CancelTargeting()
    {
        targetingSpell = null;
        targetingCaster = null;
    }

    /// <summary>
    /// Arrow keys move the cursor, Enter casts, Esc cancels. Swallows everything else while aiming.
    /// </summary>
    private bool ProcessTargetingKeyboard(Keyboard keyboard)
    {
        if (keyboard.IsKeyPressed(Keys.Escape))
        {
            fightFeed.AddLine($"{targetingSpell.Name} cancelled.");
            CancelTargeting();
            return true;
        }

        if (keyboard.IsKeyPressed(Keys.Enter))
        {
            ConfirmTarget();
            return true;
        }

        if (keyboard.IsKeyPressed(Keys.Up)) MoveCursor(CursorPosition + Direction.UP.ToVector());
        if (keyboard.IsKeyPressed(Keys.Down)) MoveCursor(CursorPosition + Direction.DOWN.ToVector());
        if (keyboard.IsKeyPressed(Keys.Left)) MoveCursor(CursorPosition + Direction.LEFT.ToVector());
        if (keyboard.IsKeyPressed(Keys.Right)) MoveCursor(CursorPosition + Direction.RIGHT.ToVector());

        return true;
    }

    /// <summary>
    /// Range in blue, the spell's area at the cursor in green, or red if the cursor is out of range.
    /// </summary>
    private void DrawTargeting()
    {
        int range = targetingSpell.Range;
        Point origin = targetingCaster.Position;

        for (int dy = -range; dy <= range; dy++)
        {
            for (int dx = -range; dx <= range; dx++)
            {
                Point cell = origin + new Point(dx, dy);
                if (InTargetRange(cell))
                    overlay.SetBackground(cell.X, cell.Y, RANGE_COLOR);
            }
        }

        // the cursor cell always gets painted too, in case an area doesn't include (0, 0)
        Color areaColor = InTargetRange(CursorPosition) ? AREA_COLOR : AREA_INVALID_COLOR;
        foreach (Point offset in targetingSpell.Area.GetRotated(targetingCaster.Direction).Append(Point.Zero))
        {
            Point cell = CursorPosition + offset;
            if (cell.X >= 0 && cell.Y >= 0 && cell.X < overlay.Width && cell.Y < overlay.Height)
                overlay.SetBackground(cell.X, cell.Y, areaColor);
        }
    }

    private static int Distance(Point a, Point b)
        => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
}
