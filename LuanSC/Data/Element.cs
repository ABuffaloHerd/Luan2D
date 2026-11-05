using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data
{
    /// <summary>
    /// Elements on damage and effects. Flags, so something can be more than one, e.g. Fire | Electric.
    /// Separate from DamageType: a fireball is MAGIC and Fire, a flaming sword could be PHYSICAL and Fire.
    /// </summary>
    [Flags]
    public enum Element
    {
        None = 0,
        Fire = 1 << 0,
        Electric = 1 << 1,
        // add more here, keep them powers of two
    }

    /// <summary>
    /// Shared by HPComponent.ElementModifiers and ElementalModifierEffect so both follow the same rules.
    /// </summary>
    public static class ElementRules
    {
        /// <summary>
        /// Whether a percent modifier for element applies to this hit. The hit needs every element in element.
        /// True damage takes the path of least resistance: weaknesses (positive) apply, resistances (negative) don't.
        /// </summary>
        public static bool Applies(DamageRecord damage, Element element, int percent)
            => element != Element.None
            && damage.Element.HasFlag(element)
            && !(damage.Type == DamageType.TRUE && percent < 0);

        /// <summary>
        /// amount scaled by percent, e.g. 50 is +50%, -20 is -20%. The cast drops the fraction, same as RES. Never below 0.
        /// </summary>
        public static int Scale(int amount, int percent)
            => Math.Max(0, (int)(amount * (1 + percent / 100f)));
    }
}
