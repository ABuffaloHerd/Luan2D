using LuanSC.Data.Components;
using LuanSC.Data.Components.Effects;
using LuanSC.Objects;
using LuanSC.Objects.Weapons;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data
{
    /// <summary>
    /// Describes a combat scenario.
    /// </summary>
    public struct CombatSettings
    {
        public List<GameObject> GameObjects = new();

        // default arena size
        public int Width = 43;
        public int Height = 43;

        public float FontScale = 1;

        public CombatSettings() { }
        public void AddGameObject(GameObject thing)
        {
            GameObjects.Add(thing); 
        }

        public static CombatSettings GetTestSettings()
        {
            CombatSettings settings = new CombatSettings();

            var hirina = new PlayerGameObject(new ColoredGlyph(Color.Red, Color.Transparent, 'H'), 0);
            hirina.Name = "Hirina";
            hirina.GetComponent<HPComponent>().Max = 250;
            hirina.GetComponent<MPComponent>().Max = 100;
            hirina.GetComponent<SpeedComponent>().Speed = 100;
            hirina.GetComponent<WeaponComponent>().Weapon = WeaponRegistry.Katana();
            hirina.Position = new(1, 1);
            settings.AddGameObject(hirina);

            var targetdummy = new EnemyGameObject(new ColoredGlyph(Color.White, Color.Transparent, 'T'), 0);
            targetdummy.Name = "Target Dummy";
            targetdummy.Position = new(5, 5);
            targetdummy.GetComponent<HPComponent>().Max = 100;
            targetdummy.GetComponent<SpeedComponent>().Speed = 50;
            settings.AddGameObject(targetdummy);

            var minako = new PlayerGameObject(new ColoredGlyph(Color.Gray, Color.Transparent, 'M'), 0);
            minako.Name = "Minako";
            minako.GetComponent<HPComponent>().Max = 100;
            minako.GetComponent<MPComponent>().Max = 50;
            minako.GetComponent<SpeedComponent>().Speed = 90;
            minako.GetComponent<SPComponent>().Max = 75;
            minako.Position = new Point(2, 2);
            settings.AddGameObject(minako);

            var mariah = new PlayerGameObject(new ColoredGlyph(Color.Blue, Color.Transparent, 'M'), 0);
            mariah.Name = "Mariah";
            mariah.GetComponent<HPComponent>().Max = 150;
            mariah.GetComponent<MPComponent>().Max = 200;
            mariah.GetComponent<MPComponent>().Current = 75;
            mariah.GetComponent<SpeedComponent>().Speed = 80;
            mariah.GetComponent<WeaponComponent>().Weapon = WeaponRegistry.Stick();
            mariah.Position = new Point(3, 3);
            settings.AddGameObject(mariah);

            //settings.Width = 21;
            //settings.Height = 21;
            //settings.FontScale = 1.9f;

            // Poison hirina
            PoisonEffect poison = new(10, 10);
            hirina.GetComponent<EffectComponent>().Apply(poison);

            // give minako a barrier
            BarrierEffect barrier = new()
            {
                Duration = 1
            };
            minako.GetComponent<EffectComponent>().Apply(barrier);

            return settings;
        }
    }
}
