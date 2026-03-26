using LuanSC.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC;

public static class GameEvents
{
    public static event Action<CombatEvent> OnCombatEvent;
    public static void Publish(CombatEvent e)
    {
        OnCombatEvent?.Invoke(e);
    }
}
