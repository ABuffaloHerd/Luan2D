using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    /// <summary>
    /// Is the owner capable of having a turn?
    /// </summary>
    public class ControllableComponent : IComponent
    {
        public string Name => "Controllable Component";

        public GameObject Owner { get; private set; }

        public void SetOwner(GameObject owner)
        {
            Owner = owner;
        }
    }
}
