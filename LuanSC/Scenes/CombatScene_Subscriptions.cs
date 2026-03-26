using LuanSC.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace LuanSC.Scenes;

public partial class CombatScene : IDisposable
{
    private bool disposed;
    private readonly Action<CombatEvent> combatEventHandler;
    private void Subscribe()
    {
        GameEvents.OnCombatEvent += combatEventHandler;
    }
    public void Dispose()
    {
        if (disposed) return;
        GameEvents.OnCombatEvent -= combatEventHandler;
        disposed = true;
    }

    private void HandleCombatEvent(CombatEvent e)
    {
        switch (e)
        {
            case CombatEvent.DamageDealt damageDealt:
                if (damageDealt.Attacker is null) // it's probably from an effect but c# doesn't have unions. the one time those are actually useful
                {
                    fightFeed.AddLine($"{damageDealt.Target.Name} took {damageDealt.Amount} {damageDealt.Type} damage.");
                    break;
                }
                fightFeed.AddLine($"{damageDealt.Attacker.Name} dealt {damageDealt.Amount} {damageDealt.Type} damage to {damageDealt.Target.Name}.");
                break;

            case CombatEvent.DamageBlocked damageBlocked:
                fightFeed.AddLine($"{damageBlocked.Owner?.Name ?? "An unknown source"}'s {damageBlocked.EffectName} blocked {damageBlocked.Blocked} damage.");
                break;
            case CombatEvent.HealingDealt healingRecord:
                fightFeed.AddLine($"{healingRecord.Healer.Name} healed {healingRecord.Target.Name} for {healingRecord.Amount} HP.");
                break;
            case CombatEvent.GenericMessage genericMessage:
                fightFeed.AddLine(genericMessage.Message);
                break;

            case CombatEvent.EffectExpired effectExpired:
                fightFeed.AddLine($"{effectExpired.Target.Name}'s {effectExpired.EffectName} expired.");
                break;
        }
    }
}
