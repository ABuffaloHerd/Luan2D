using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    /// <summary>
    /// Can the owner take damage?
    /// </summary>
    public class HealthComponent : IComponent
    {
        public string Name => "HealthComponent";

        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int DEF { get; set; } = 0; // Physical Defense
        public int RES { get; set; } = 0; // Magical Resistance
        public GameObject Owner { get; private set; }

        public void SetOwner(GameObject owner)
        {
            this.Owner = owner;
        }

        public HealthComponent(int maxHealth = 100)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
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

            CurrentHealth = Math.Max(0, taken);
            return taken;
        }
    }
}
