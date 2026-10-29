using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    /// <summary>
    /// Listens for the start of the owner's turn. The scene calls this on every component that implements it.
    /// </summary>
    public interface IOnTurnStartListener
    {
        void OnTurnStart();
    }
}
