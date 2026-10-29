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

            // TODO: p is never passed in. It's the same as the default range so nothing breaks, but it will if p changes.
            Weapon s = new Weapon("Stick", 4);
            s.Abilities.Add(new BasicAttack(s.Damage));
            return s;
        }

        // Give em the hands
        public static Weapon Hands()
        {
            Pattern p = new();
            p.Mark(0, -1);
            // TODO: same as Stick, p is never passed in
            Weapon h = new Weapon("The Hands", 1);
            h.Abilities.Add(new BasicAttack(h.Damage));
            return h;
        }
    }
}
