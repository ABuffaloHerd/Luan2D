using LuanSC.Data;
using LuanSC.Data.Components;
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

        public static Weapon Throngler()
        {
            Pattern p = new();
            p.Mark(0, -1).Mark(0, -2).Mark(0, -3).Mark(0, -4);
            p.Mark(1, 0).Mark(-1, 0).Mark(1, -1).Mark(-1, -1);
            Weapon t = new Weapon("Throngler", 500, p); // five hundred damage.

            t.Abilities.Add(new BasicAttack(t.Damage));

            return t;
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

        // Hits everything in range
        public static Weapon Shotgun()
        {
            Pattern p = new();
            p.Mark(0, -1);
            p.Mark(1, -1);
            p.Mark(-1, -1);
            p.Mark(0, -2);
            p.Mark(1, -2);
            p.Mark(-1, -2);

            Weapon s = new Weapon("Shotgun", 6, p);
            s.Abilities.Add(new BlastAttack(s.Damage));
            s.Costs.Add(new ResourceCost(ResourceType.AMMO, 1)); // a shell per shot, Reload to refill
            return s;
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

        // Duct tape unrifled musket tied to a trombone stand
        public static Weapon SentryGun()
        {
            Pattern p = new();
            p.Mark(0, -1).Mark(0, -2).Mark(0, -3).Mark(0, -4);

            Weapon g = new Weapon("Sentry Gun", 5, p);
            g.Abilities.Add(new BasicAttack(g.Damage));
            return g;
        }

        // TITAN APLU's arms
        public static Weapon HydraulicFists()
        {
            Pattern p = new();
            p.Mark(0, -1);

            Weapon f = new Weapon("Hydraulic Fists", 25, p);
            f.Abilities.Add(new BasicAttack(f.Damage));
            return f;
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
