using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    /// <summary>
    /// Can the owner take damage?
    /// </summary>
    public class HPComponent : StatComponent, IHudDisplay
    {
        public override string Name => "HP Component";

        public IEnumerable<HudBar> GetHudBars()
        {
            yield return new HudBar(0, "HP :", Color.Red, Color.Red, HudBar.Ratio(Current, Max), $"{Current} / {Max}");
        }
        public int DEF { get; set; } = 0; // Physical Defense
        public int RES { get; set; } = 0; // Magical Resistance

        /// <summary>
        /// The owner's own elemental traits, in percent: positive is a weakness, negative a resistance.
        /// e.g. { [Element.Electric] = 50, [Element.Fire] = -25 }. Temporary ones go in ElementalModifierEffect instead.
        /// </summary>
        public Dictionary<Element, int> ElementModifiers { get; } = new();

        public bool IsAlive => Current > 0;

        /// <summary>
        /// The most recent hit, after effects and DEF/RES. On a corpse this is the killing blow.
        /// </summary>
        public DamageRecord LastHit { get; private set; }

        // Alias for clarity
        public int MaxHP
        {
            get => Max;
            set => Max = value;
        }
        public int CurrentHP
        {
            get => Current;
            set => Current = value;
        }

        public HPComponent(int maxHealth = 100)
        {
            Max = maxHealth;
            Current = maxHealth;
        }

        /// <summary>
        /// Take damage. Every source of damage goes through here, so this is where all the effects get their say:
        ///   1. the attacker's effects (OnOutgoingDamage), e.g. a damage buff
        ///   2. DEF or RES
        ///   3. the owner's ElementModifiers, multiplied together for hits with several elements
        ///   4. the owner's effects (OnIncomingDamage), e.g. barrier or an ElementalModifierEffect
        /// Attacks and spells just send the base damage.
        /// </summary>
        /// <param name="damage">the amount of hurt to receive</param>
        /// <returns>the amount of hurt taken by this component, after effect processing</returns>
        public int TakeDamage(DamageRecord damage)
        {
            // the attacker's buffs and debuffs. Poison and the like have no attacker.
            var attackerEffects = damage.Attacker?.GetComponent<EffectComponent>();
            if (attackerEffects is not null)
            {
                damage = attackerEffects.ProcessOutgoingDamage(damage);
            }

            int taken = damage.Type switch
            {
                DamageType.PHYSICAL => damage.Amount - DEF,                     // Flat reduction
                DamageType.MAGIC => (int)(damage.Amount * (1 - RES / 100f)), // Percentage reduction   
                DamageType.TRUE => damage.Amount,                           // fuck that's gotta hurt
                _ => damage.Amount,                           // how did we get here
            };

            // elemental traits, after DEF/RES. Each one that applies multiplies
            foreach ((Element element, int percent) in ElementModifiers)
            {
                if (!ElementRules.Applies(damage, element, percent)) continue;

                int before = taken;
                taken = ElementRules.Scale(taken, percent);

                if (taken != before)
                {
                    string change = taken > before ? $"+{taken - before}" : (taken - before).ToString();
                    string trait = percent >= 0 ? "weakness" : "resistance";
                    GameEvents.Publish(new CombatEvent.GenericMessage($"{Owner?.Name}'s {element} {trait}: {change} damage."));
                }
            }


            // Let effects modify the incoming damage
            var effects = Owner?.GetComponent<EffectComponent>();
            if (effects is not null)
            {
                taken = effects.ProcessIncomingDamage(damage with { Amount = taken }).Amount;
            }

            // DEF higher than the hit, or an effect overdoing it, mustn't heal
            taken = Math.Max(taken, 0);

            Current -= taken;
            LastHit = damage with { Amount = taken };

            // notify damage listeners
            if (Owner is not null)
            {
                foreach (IDamageListener listener in Owner.Components.OfType<IDamageListener>())
                {
                    listener.OnDamageTaken(damage with { Amount = taken });
                }
            }

            // notify listeners if damage is dealt
            if (damage.Attacker is not null)
            {
                foreach (IDamageDealtListener listener in damage.Attacker.Components.OfType<IDamageDealtListener>())
                {
                    listener.OnDamageDealt(damage with { Amount = taken });
                }
            }

            return taken;
        }

        public int Heal(HealingRecord healthy)
        {
            int amount = healthy.Amount;
            int healed = Math.Min(amount, Max - Current);

            // Opportunity for effects to modify healing
            foreach(IHealingListener listener in Owner.Components.OfType<IHealingListener>())
            {
                listener.OnHealingReceived(healthy with { Amount = healed });
            }

            Current += healed;
            return healed;
        }
    }
}
