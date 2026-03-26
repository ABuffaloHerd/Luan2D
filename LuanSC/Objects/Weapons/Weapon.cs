using LuanSC.Data;
using LuanSC.Data.Components;
using LuanSC.Data.Components.Attacks;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Objects.Weapons
{
    /// <summary>
    /// Glorified IAbility container
    /// Damage attribute is used at the weapon factory and is passed into attacks so the attacks decide how to use that number.
    /// </summary>
    public class Weapon
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Damage { get; set; }

        public Pattern Range { get; private set; } = new Pattern(); // Default: single target

        public List<IAbility> Abilities{ get; } = new();
        public Weapon(string name, int damage)
        {
            Name = name;
            Damage = damage;

            // default 1 range pattern
            Range.Mark(0, -1);
        }

        public Weapon (string name, int damage, Pattern range) : this(name, damage)
        {
            Range = range;
        }

        public T GetAbility<T>() where T : IAbility => Abilities.OfType<T>().FirstOrDefault();
    }
}
