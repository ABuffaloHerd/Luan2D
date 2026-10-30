using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    /// <summary>
    /// Overdrive. Charges from damage taken, spent on special moves. Starts empty.
    /// Charge past full spills into the owner's GBComponent, if they have one.
    /// </summary>
    public class ODComponent : ResourceComponent, IDamageListener
    {
        public override string Name => "OD Component";

        /// <summary>
        /// OD gained per point of damage taken, after DEF, RES and effects. The fraction is dropped,
        /// so at 0.1 a hit under 10 gives nothing, and neither does a blocked one. That's intended.
        /// </summary>
        public float ChargeRate { get; set; }

        public ODComponent(int max = 100, float chargeRate = 0.1f) : base(ResourceType.OD, "Overdrive", $"OD{(char)224}", Color.Purple, max)
        {
            ChargeRate = chargeRate;
            HudOrder = 30;
            Current = 0;
        }

        protected override string HudText => $"{(int)(Current * 100f / Max)}%";

        // GB is spent first and OD covers the rest, so an OD cost can be paid out of both bars
        public override bool CanSpend(int amount)
            => Current + (Owner?.GetResource(ResourceType.GB)?.Current ?? 0) >= amount;

        public override bool TrySpend(int amount)
        {
            if (!CanSpend(amount)) return false;

            if (Owner?.GetResource(ResourceType.GB) is ResourceComponent gb)
            {
                int fromGb = Math.Min(gb.Current, amount);
                gb.Current -= fromGb;
                amount -= fromGb;
            }

            Current -= amount;
            return true;
        }

        public void OnDamageTaken(DamageRecord damage)
        {
            int gain = (int)(damage.Amount * ChargeRate);
            if (gain <= 0) return;

            int overflow = Math.Max(0, Current + gain - Max);
            Current += gain; // clamped to Max

            if (overflow > 0 && Owner?.GetResource(ResourceType.GB) is ResourceComponent gb)
                gb.Current += overflow; // GB throws away its own overflow
        }
    }
}
