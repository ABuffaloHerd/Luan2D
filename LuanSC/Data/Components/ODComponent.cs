using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    /// <summary>
    /// Handles the overdrive system
    /// </summary>
    public class ODComponent : IComponent, IDamageListener
    {
        private static float OD_MULTIPLIER = 0.1f; // affects how quickly OD fills
        public string Name => "OD Component";

        /// <summary>
        /// Filled when damage is taken, used to trigger overdrive abilities
        /// </summary>
        public int CurrentOD
        {
            get => odCurrent;
            private set
            {
                if (value < 0)
                    odCurrent = 0;
                else if (value > 100)
                {
                    int difference = value - 100;
                    odCurrent = 100; // overflow to GB
                    CurrentGB += difference;
                }
                else
                    odCurrent = value;
            }
        }

        /// <summary>
        /// Filled when OD is at 100, used to trigger even stronger abilities
        /// </summary>
        public int CurrentGB
        {
            get => gbCurrent;
            private set
            {
                if (value < 0)
                    gbCurrent = 0;
                else if (value > 100) // discard overflow
                    gbCurrent = 100;
                else
                    gbCurrent = value;
            }
        }

        // private backing fields
        private int odCurrent;
        private int gbCurrent;

        public GameObject Owner { get; private set; }
        public void SetOwner(GameObject owner)
        {
            Owner = owner;
        }

        public void OnDamageTaken(DamageRecord damage)
        {
            int amount = damage.Amount;
            int odGain = (int)(amount * OD_MULTIPLIER);
            CurrentOD += odGain; // this automatically handles overflow to GB
        }
    }
}
