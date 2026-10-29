using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.UI;

namespace LuanSC.Scenes;

public partial class CombatScene
{
    private int windowInstances = 0;
    private void DisplaySpellbook()
    {
        if (windowInstances > 0) return;

        // might need to make this a class var
        Window spellbook = new Window(30, 30)
        {
            Title = $"Spellbook",
            CanDrag = true,
            UseMouse = true,
            CloseOnEscKey = true,
        };

        windowInstances++;

        spellbook.Closed += (sender, args) =>
        {
            windowInstances--;
            RestoreFocus();
        };
        spellbook.Position = new((GameSettings.GAME_WIDTH / 2) - (spellbook.Width / 2) - 1, (GameSettings.GAME_HEIGHT / 2) - (spellbook.Height / 2) + 1);
        spellbook.Show(true);
    }

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
        help.Position = new((GameSettings.GAME_WIDTH / 2) - (help.Width / 2) - 1, (GameSettings.GAME_HEIGHT / 2) - (help.Height / 2) + 1);
        help.Show(true);
    }

    private void RestoreFocus()
    {
        this.IsFocused = true;
    }
}
