using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data
{
    public abstract record CombatEvent
    {
        public sealed record DamageDealt(GameObject Attacker, GameObject Target, int Amount, DamageType Type) : CombatEvent;

        /// <summary>
        /// When damage is blocked
        /// </summary>
        /// <param name="Owner">Who was saved</param>
        /// <param name="EffectName">What saved them</param>
        /// <param name="Blocked">How much was blocked</param>
        public sealed record DamageBlocked(GameObject Owner, string EffectName, int Blocked) : CombatEvent;
        public sealed record HealingDealt(GameObject Healer, GameObject Target, int Amount) : CombatEvent;
        public sealed record EffectApplied(GameObject Target, string EffectName) : CombatEvent;
        public sealed record EffectExpired(GameObject Target, string EffectName) : CombatEvent;
        public sealed record GenericMessage(string Message) : CombatEvent;
        public sealed record EntityDied(GameObject Target) : CombatEvent;
    }
}
