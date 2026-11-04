using LuanSC.Data;
using LuanSC.Data.Definitions;
using LuanSC.Scenes.Transitions;
using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes;

public class BattleSelectionScene : Scene
{
    ControlsConsole controls;
    ScreenSurface info;
    Button launch;
    CombatSettings? selected;

    public BattleSelectionScene(SceneManager manager) : base(manager)
    {
        int half = GameSettings.GAME_WIDTH / 2;

        controls = new ControlsConsole(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);

        // a click would take the keyboard off the scene and Esc would stop working
        controls.UseKeyboard = false;
        controls.FocusOnMouseClick = false;

        // left half, the list of battles
        ListBox battles = new(half - 2, GameSettings.GAME_HEIGHT - 2)
        {
            Position = new(1, 1),
            DrawBorder = true,
        };

        // sorted by category, so a category's battles stay together
        foreach (BattleEntry battle in BattleCatalog.All)
            battles.Items.Add(battle);

        battles.SelectedItemChanged += (s, e) => Select(e.Item as BattleEntry);

        // right half, the launch button under the details of the selected battle
        launch = new(GameSettings.GAME_WIDTH - half - 3, 3)
        {
            Text = "SEND IT",
            Position = new(half + 1, GameSettings.GAME_HEIGHT - 5),
            IsEnabled = false,
        };
        launch.Click += (s, e) =>
        {
            if (selected is not CombatSettings settings) return;

            GameRoot.sceneManger.ChangeScene(manager => new CombatScene(manager, settings), new RandomGarbageTransition(TimeSpan.FromMilliseconds(500)));
        };

        controls.Controls.Add(battles);
        controls.Controls.Add(launch);

        info = new(GameSettings.GAME_WIDTH - half - 3, GameSettings.GAME_HEIGHT - 8)
        {
            Position = new(half + 1, 2),
            UseMouse = false,
            UseKeyboard = false,
            FocusOnMouseClick = false,
        };
        Border.BorderParameters bp = Border.BorderParameters.GetDefault();
        bp.DrawBorder = true;

        Border b = new(info, bp);

        Children.Add(controls);
        Children.Add(info);

        Select(null);
        IsFocused = true;
    }

    /// <summary>
    /// Shows a battle's details and arms the launch button. Null clears the panel and disarms it.
    /// </summary>
    private void Select(BattleEntry? battle)
    {
        selected = battle?.Create();
        launch.IsEnabled = selected is not null;

        info.Surface.Clear();

        if (battle is null || selected is null)
        {
            PrintWrapped("Select a battle.", Color.Gray, 1);
            return;
        }

        int y = PrintWrapped(battle.Title, Color.Yellow, 1) + 1;

        string description = string.IsNullOrWhiteSpace(selected.Description) ? "No description." : selected.Description;
        y = PrintWrapped(description, Color.White, y) + 1;

        y = PrintWrapped($"Arena: {selected.Width} x {selected.Height}", Color.Gray, y) + 1;

        y = PrintUnits("Party", UnitKind.PLAYER, selected, y);
        PrintUnits("Enemies", UnitKind.ENEMY, selected, y);
    }

    /// <summary>
    /// Prints a heading and the names of every unit of that kind. Repeats are counted, e.g. "Zombie x3". Prints nothing if there are none.
    /// </summary>
    private int PrintUnits(string heading, UnitKind kind, CombatSettings settings, int y)
    {
        List<IGrouping<string, UnitDefinition>> units = settings.Units
            .Where(unit => unit.Kind == kind)
            .GroupBy(unit => unit.Name ?? "Unnamed")
            .ToList();

        if (units.Count == 0) return y;

        y = PrintWrapped(heading, Color.Gray, y);

        foreach (IGrouping<string, UnitDefinition> unit in units)
        {
            string name = unit.Count() > 1 ? $"{unit.Key} x{unit.Count()}" : unit.Key;
            y = PrintWrapped($"  {name}", Color.White, y);
        }

        return y + 1;
    }

    /// <summary>
    /// Prints word wrapped text to the info panel and returns the next free row.
    /// </summary>
    private int PrintWrapped(string text, Color colour, int y)
    {
        foreach (string line in text.WordWrap(info.Width - 2))
            info.Surface.Print(1, y++, line, colour);

        return y;
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.IsKeyPressed(SadConsole.Input.Keys.Escape))
        {
            GameRoot.sceneManger.ChangeScene(manager => new Menu(manager));
            return true;
        }

        return base.ProcessKeyboard(keyboard);
    }
}
