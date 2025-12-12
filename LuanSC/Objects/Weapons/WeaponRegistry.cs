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
            k.Components.Add(new SlashingAttack());

            return k;
        }
    }
}
