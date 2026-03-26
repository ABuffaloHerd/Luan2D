using LuanSC.Data;
using LuanSC.Data.Components.Attacks;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Objects.Weapons
{
    public static class WeaponRegistry
    {
        public static Weapon Katana()
        {
            Pattern p = new();
            p.Mark(0, -1).Mark(0, -2);

            Weapon k = new Weapon("Katana", 10, p);

            // Dual strike
            k.Abilities.Add(new BasicAttack(k.Damage));
            k.Abilities.Add(new BasicAttack(k.Damage));

            return k;
        }

        public static Weapon Gun()
        {
            Pattern p = new();
            p.Mark(0, -1).Mark(0, -2).Mark(0, -3);
            Weapon g = new Weapon("Gun", 8, p);
            //g.Components.Add(new RangedAttack());
            //g.Components.Add(new PiercingAttack());
            return g;
        }

        public static Weapon Stick()
        {
            Pattern p = new();
            p.Mark(0, -1);

            Weapon s = new Weapon("Stick", 4);
            s.Abilities.Add(new BasicAttack(s.Damage));
            return s;
        }
    }
}
