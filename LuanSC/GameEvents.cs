using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC;

public static class GameEvents
{
    public static event Action<string> OnCombatMessage;
    public static void CombatMessage(string message)
    {
        OnCombatMessage?.Invoke(message);
    }
}
