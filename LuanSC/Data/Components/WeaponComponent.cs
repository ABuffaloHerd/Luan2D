using LuanSC.Objects;
using LuanSC.Objects.Weapons;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    /// <summary>
    /// The fastest way to tell if the gameobject has a weapon or not and provides an interface for it
    /// </summary>
    public class WeaponComponent : IComponent
    {
        public string Name => "Weapon Component";

        public GameObject Owner { get; private set;  }

        public Weapon Weapon { get; set; }
        public void SetOwner(GameObject owner)
        {
            Owner = owner;
        }

        public WeaponComponent(Weapon weapon)
        {
            Weapon = weapon;
        }
    }
}
