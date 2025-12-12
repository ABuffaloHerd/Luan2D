using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    /// <summary>
    /// Does this object bump into other objects?
    /// If this component is present, then true.
    /// </summary>
    public class CollisionComponent : IComponent
    {
        private GameObject owner;

        public string Name => "Collision component";

        public GameObject Owner { get; set; }

        public void SetOwner(GameObject owner)
        {
            Owner = owner;
        }
    }
}
