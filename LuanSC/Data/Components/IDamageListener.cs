using System;
using System.Collections.Generic;
using System.Text;
using LuanSC.Data.Combat;

namespace LuanSC.Data.Components
{
    /// <summary>
    /// Listens for events when damage is taken.
    /// </summary>
    public interface IDamageListener
    {
        void OnDamageTaken(DamageRecord damage);
    }
}
