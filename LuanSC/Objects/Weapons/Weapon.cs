using LuanSC.Data;
using LuanSC.Data.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Objects.Weapons
{
    public class Weapon
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Damage { get; set; }

        public Pattern Range { get; private set; } = new Pattern(); // Default: single target

        public HashSet<IAttackComponent> Components{ get; } = new();
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

        public T GetComponent<T>() where T : IAttackComponent => Components.OfType<T>().FirstOrDefault();
    }
}
