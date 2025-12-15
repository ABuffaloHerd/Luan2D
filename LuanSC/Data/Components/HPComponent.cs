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
        public override string Name => "HealthComponent";
        public int DEF { get; set; } = 0; // Physical Defense
        public int RES { get; set; } = 0; // Magical Resistance
        public GameObject Owner { get; private set; }

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
                DamageType.PHYSICAL => damage.Amount - DEF, // Flat reduction
                DamageType.MAGIC => (int)(damage.Amount * (1 - RES / 100f)), // Percentage reduction   
                DamageType.TRUE => damage.Amount,     // fuck that's gotta hurt
                _ => damage.Amount,
            };

            Current -= taken;
            return taken;
        }
    }
}
