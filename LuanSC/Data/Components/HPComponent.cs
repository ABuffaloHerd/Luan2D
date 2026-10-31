using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    /// <summary>
    /// The owner's health. Every hit and heal goes through TakeDamage and Heal, where effects, DEF/RES and elements get their say.
    /// Dead units (0 HP) ignore both.
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
            // corpses stay dead and keep their real killing blow in LastHit
            if (!IsAlive) return 0;

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
                    GameEvents.Publish(new CombatEvent.ElementTrait(Owner, element, percent, taken - before));
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

        /// <summary>
        /// Heal. Every source of healing goes through here, mirroring TakeDamage:
        ///   1. the healer's effects (OnOutgoingHealing), e.g. a healing power buff
        ///   2. the owner's effects (OnIncomingHealing), e.g. anti-heal
        ///   3. capped so it never goes below 0 or past Max
        ///   4. the owner's IHealingListeners and the healer's IHealingGivenListeners are told the final amount
        /// Spells and effects just send the base amount.
        /// </summary>
        /// <param name="healthy">the amount of healing to receive</param>
        /// <returns>the HP actually restored, after effects and the Max cap</returns>
        public int Heal(HealingRecord healthy)
        {
            // no raising the dead
            if (!IsAlive) return 0;

            // the healer's buffs and debuffs. Regen and the like may have no healer.
            var healerEffects = healthy.Healer?.GetComponent<EffectComponent>();
            if (healerEffects is not null)
            {
                healthy = healerEffects.ProcessOutgoingHealing(healthy);
            }

            // Let the owner's effects modify the incoming healing
            var effects = Owner?.GetComponent<EffectComponent>();
            if (effects is not null)
            {
                healthy = effects.ProcessIncomingHealing(healthy);
            }

            // an effect overdoing it mustn't hurt, and you can't heal past full
            int healed = Math.Clamp(healthy.Amount, 0, Max - Current);

            Current += healed;
            HealingRecord final = healthy with { Amount = healed };

            // notify healing listeners
            if (Owner is not null)
            {
                foreach (IHealingListener listener in Owner.Components.OfType<IHealingListener>())
                {
                    listener.OnHealingReceived(final);
                }
            }

            // notify the healer's listeners
            if (healthy.Healer is not null)
            {
                foreach (IHealingGivenListener listener in healthy.Healer.Components.OfType<IHealingGivenListener>())
                {
                    listener.OnHealingGiven(final);
                }
            }

            return healed;
        }
    }
}
