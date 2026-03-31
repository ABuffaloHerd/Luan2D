using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.UI;

namespace LuanSC.Scenes;

public partial class CombatScene
{
    private int spellbookInstances = 0;
    private void DisplaySpellbook()
    {
        if (spellbookInstances > 0) return;

        // might need to make this a class var
        Window spellbook = new Window(30, 30)
        {
            Title = $"Spellbook",
            CanDrag = true,
            UseMouse = true,
            CloseOnEscKey = true,
        };

        spellbookInstances++;

        spellbook.Closed += (sender, args) =>
        {
            spellbookInstances--;
            RestoreFocus();
        };
        spellbook.Position = new((GameSettings.GAME_WIDTH / 2) - (spellbook.Width / 2) - 1, (GameSettings.GAME_HEIGHT / 2) - (spellbook.Height / 2) + 1);
        spellbook.Show(true);
    }

    private void RestoreFocus()
    {
        this.IsFocused = true;
    }
}
