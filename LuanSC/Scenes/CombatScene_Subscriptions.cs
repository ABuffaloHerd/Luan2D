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

            case CombatEvent.EntityDied entityDied:
                fightFeed.AddLine(DeathMessage(entityDied));
                break;

            case CombatEvent.Summoned summoned:
                fightFeed.AddLine($"{summoned.Owner.Name} summoned {summoned.Summon.Name}.");
                break;
        }
    }

    /// <summary>
    /// Picked at random for every death. {target} is who died, {killer} who or what killed them, {source} what they were killed with.
    /// TODO: These will be read from a file later.
    /// </summary>
    private static readonly string[] deathMessages =
    [
        "{target} was killed by {killer}.",
        "{target} was eviscerated by {killer}'s {source}.",
        "{target} was annihilated by {killer}.",
        "{target} was obliterated by {killer}'s {source}.",
        "{target} is no longer among us.",
        "{target} was not the imposter.",
        "The funeral wake for {target} is next week, {killer} is invited.",
        "Call an ambulance! But not for {killer}."
    ];

    private static string DeathMessage(CombatEvent.EntityDied died)
    {
        DamageRecord blow = died.KillingBlow;

        // poison and the like have no attacker, so the effect gets the credit
        string killer = blow?.Attacker?.Name ?? blow?.Source ?? "something";
        string source = blow?.Source ?? "something";

        return deathMessages[Random.Shared.Next(deathMessages.Length)]
            .Replace("{target}", died.Target.Name)
            .Replace("{killer}", killer)
            .Replace("{source}", source);
    }
}
