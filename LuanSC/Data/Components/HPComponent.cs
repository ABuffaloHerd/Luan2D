using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    /// <summary>
    /// Can the owner take damage?
    /// </summary>
    public class HPComponent : StatComponent
    {
        public override string Name => "HP Component";
        public int DEF { get; set; } = 0; // Physical Defense
        public int RES { get; set; } = 0; // Magical Resistance

        public bool IsAlive => Current > 0;

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
        /// Take damage
        /// </summary>
        /// <param name="damage">the amount of hurt to receive</param>
        /// <returns>the amount of hurt taken by this component, after effect processing</returns>
        public int TakeDamage(DamageRecord damage)
        {
            // TODO: Process effects that reduce damage here
            // Take effects from Owner.GetComponents<EffectComponent>()

            int taken = damage.Type switch
            {
                DamageType.PHYSICAL => damage.Amount - DEF,                     // Flat reduction
                DamageType.MAGIC    => (int)(damage.Amount * (1 - RES / 100f)), // Percentage reduction   
                DamageType.TRUE     => damage.Amount,                           // fuck that's gotta hurt
                _                   => damage.Amount,                           // how did we get here
            };


            // Let effects modify the incoming damage
            var effects = Owner?.GetComponent<EffectComponent>();
            if (effects is not null)
            {
                taken = effects.ProcessIncomingDamage(damage with { Amount = taken }).Amount;
            }

            Current -= taken;

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
    }
}
