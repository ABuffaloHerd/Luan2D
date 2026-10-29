using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Instructions;
using SadConsole.UI;
using SadConsole.UI.Controls;
using LuanSC.Data.Components;
using LuanSC.Data.Spells;
using LuanSC.Objects;

namespace LuanSC.Scenes;

public partial class CombatScene
{
    private int windowInstances = 0;
    private void DisplaySpellbook()
    {
        if (windowInstances > 0) return;

        // only your own units, and only on their turn
        GameObject caster = currentControlledGameObject;
        if (caster?.GetComponent<ControllableComponent>() is null) return;

        SpellbookComponent book = caster.GetComponent<SpellbookComponent>();

        // might need to make this a class var
        Window spellbook = new Window(40, 40)
        {
            Title = $"{caster.Name}'s Actions",
            CanDrag = true,
            UseMouse = true,
            CloseOnEscKey = true,
        };

        windowInstances++;

        // Hide may or may not raise Closed, so make sure this only runs once
        bool closed = false;
        void OnClosed()
        {
            if (closed) return;
            closed = true;
            windowInstances--;
            RestoreFocus();
        }

        spellbook.Closed += (sender, args) => OnClosed();

        // content area, the tab control adds its headers and border around this
        int contentWidth = spellbook.Width - 2;
        int contentHeight = spellbook.Height - 2;

        TabControl tabs = new(
        [
            BuildSpellTab("Spells", ActionType.SPELL, caster, book, contentWidth, contentHeight, Cast),
            BuildSpellTab("Skills", ActionType.SKILL, caster, book, contentWidth, contentHeight, Cast),
            BuildSpellTab("Special", ActionType.SPECIAL, caster, book, contentWidth, contentHeight, Cast),
        ], contentWidth, contentHeight)
        {
            Position = new(1, 1)
        };
        spellbook.Controls.Add(tabs);

        spellbook.Position = new((GameSettings.GAME_WIDTH / 2) - (spellbook.Width / 2) - 1, (GameSettings.GAME_HEIGHT / 2) - (spellbook.Height / 2) + 1);
        spellbook.Show(true);

        // closes the book before casting so targeting gets the keyboard back
        void Cast(ISpell spell)
        {
            spellbook.Hide();
            OnClosed();
            this.Cast(caster, spell);
        }
    }

    /// <summary>
    /// One tab of the spellbook. A button per spell with its cost, greyed out if it can't be cast right now.
    /// </summary>
    private static TabItem BuildSpellTab(string header, ActionType type, GameObject caster, SpellbookComponent book, int width, int height, Action<ISpell> cast)
    {
        Panel panel = new(width, height);
        // overwrite the width parameter so that buttons don't overlap with the panel etc.
        width -= 2;

        List<ISpell> spells = book?.OfType(type).ToList() ?? new();
        if (spells.Count == 0)
        {
            panel.Add(new Label("Nothing here.") { Position = new(1, 1) });
            return new TabItem(header, panel);
        }

        int y = 0;
        foreach (ISpell spell in spells)
        {
            Button button = new(width, 1)
            {
                Text = spell.Name,
                Position = new(0, y),
                IsEnabled = spell.CanCast(caster)
            };
            button.Click += (sender, args) => cast(spell);
            panel.Add(button);

            string costs = spell.Costs.Count > 0
                ? string.Join(", ", spell.Costs.Select(c => $"{c.Amount} {c.Type}"))
                : "Free";
            string targeting = spell.Targeting == Targeting.CURSOR ? $"range {spell.Range}" : "in front";

            panel.Add(new Label(width) { DisplayText = $" {costs}, {targeting}", Position = new(0, y + 1), TextColor = Color.Gray });
            panel.Add(new Label(width) { DisplayText = $" {spell.Description}", Position = new(0, y + 2), TextColor = Color.DarkGray });

            y += 4;
        }

        return new TabItem(header, panel);
    }

    // the post build step copies Resources next to the exe
    private static readonly string HelpPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Asset", "help.txt");

    // time per character, so a longer help file takes longer to type out
    private static readonly TimeSpan HelpTypingSpeed = TimeSpan.FromMilliseconds(2);

    private void DisplayHelp()
    {
        if (windowInstances > 0) return;

        Window help = new Window(GameSettings.GAME_WIDTH - 10, GameSettings.GAME_HEIGHT - 10)
        {
            Title = $"Help",
            CanDrag = true,
            UseMouse = true,
            CloseOnEscKey = true,
        };

        windowInstances++;

        help.Closed += (sender, args) =>
        {
            windowInstances--;
            RestoreFocus();
        };

        // read every time it opens so the file can be edited without restarting
        string text = File.Exists(HelpPath)
            ? File.ReadAllText(HelpPath).Replace("\r\n", "\n")
            : "Help file not found.";

        // inside the border, with a cell of padding. Mouse and keyboard stay with the window so dragging and Esc still work
        Console page = new(help.Width - 4, help.Height - 4)
        {
            Position = new(2, 2),
            UseMouse = false,
            UseKeyboard = false,
        };
        page.Cursor.UseLinuxLineEndings = true; // \n starts a new line instead of just moving down
        page.Cursor.IsVisible = true;
        help.Children.Add(page);

        // [c:r f:cyan] and friends colour the text. Time it on the parsed length so the colour codes don't slow it down
        ColoredString coloured = ColoredString.Parser.Parse(text);

        // types the text out like the visual novel scene does
        DrawString typing = new(coloured, HelpTypingSpeed * coloured.Length)
        {
            Cursor = page.Cursor,
            RemoveOnFinished = true,
        };
        page.SadComponents.Add(typing);

        help.Position = new((GameSettings.GAME_WIDTH / 2) - (help.Width / 2) - 1, (GameSettings.GAME_HEIGHT / 2) - (help.Height / 2) + 1);
        help.Show(true);
    }

    private void RestoreFocus()
    {
        this.IsFocused = true;
    }
}
