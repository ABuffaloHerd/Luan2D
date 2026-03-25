using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    /// <summary>
    /// Listens for the start of a turn. Used in effects.
    /// </summary>
    public interface IOnTurnStartListener
    {
        void OnTurnStart();
    }
}
