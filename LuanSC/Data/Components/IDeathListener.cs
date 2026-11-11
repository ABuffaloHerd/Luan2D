using LuanSC.Data.Components.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    /// <summary>
    /// Listens for the owner dying. CombatScene.Cull calls this on every component that implements it, EffectComponent passes it on to effects.
    /// Not called when a form breaks (nobody died) or when a summon vanishes with its owner (it wasn't killed).
    /// </summary>
    public interface IDeathListener
    {
        void OnDeath(ICombatContext context);
    }
}
