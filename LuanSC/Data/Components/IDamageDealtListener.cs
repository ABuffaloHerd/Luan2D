using System;
using System.Collections.Generic;
using System.Text;
using LuanSC.Data.Combat;

namespace LuanSC.Data.Components
{
    /// <summary>
    /// Listens for events when damage is dealt.
    /// </summary>
    public interface IDamageDealtListener
    {
        void OnDamageDealt(DamageRecord damage);
    }
}
