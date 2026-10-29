using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Data.Components.Effects;
using LuanSC.Data.Spells;
using LuanSC.Objects;
using LuanSC.Objects.Summons;
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

            var hirina = new PlayerGameObject(new ColoredGlyph(Color.Red, Color.Transparent, 'H'), MagicNumbers.GAMEOBJECT_INDEX);
            hirina.Name = "Hirina";
            hirina.GetComponent<HPComponent>().Max = 250;
            hirina.GetComponent<MPComponent>().Max = 100;
            hirina.GetComponent<SpeedComponent>().Speed = 100;
            hirina.GetComponent<WeaponComponent>().Weapon = WeaponRegistry.Katana();
            hirina.Position = new(1, 1);
            settings.AddGameObject(hirina);

            var targetdummy = new EnemyGameObject(new ColoredGlyph(Color.White, Color.Transparent, 'T'), MagicNumbers.GAMEOBJECT_INDEX);
            targetdummy.Name = "Target Dummy";
            targetdummy.Position = new(5, 5);
            targetdummy.GetComponent<HPComponent>().Max = 100;
            targetdummy.GetComponent<SpeedComponent>().Speed = 50;
            settings.AddGameObject(targetdummy);

            var minako = new PlayerGameObject(new ColoredGlyph(Color.Gray, Color.Transparent, 'M'), MagicNumbers.GAMEOBJECT_INDEX);
            minako.Name = "Minako";
            minako.GetComponent<HPComponent>().Max = 100;
            minako.GetComponent<MPComponent>().Max = 50;
            minako.GetComponent<SpeedComponent>().Speed = 90;
            minako.GetComponent<SPComponent>().Max = 75;
            minako.Position = new Point(2, 2);
            settings.AddGameObject(minako);

            var mariah = new PlayerGameObject(new ColoredGlyph(Color.Blue, Color.Transparent, 'M'), MagicNumbers.GAMEOBJECT_INDEX);
            mariah.Name = "Mariah";
            mariah.GetComponent<HPComponent>().Max = 150;
            mariah.GetComponent<MPComponent>().Max = 200;
            mariah.GetComponent<MPComponent>().Current = 75;
            mariah.GetComponent<SpeedComponent>().Speed = 80;
            mariah.GetComponent<WeaponComponent>().Weapon = WeaponRegistry.Stick();
            mariah.Position = new Point(3, 3);
            settings.AddGameObject(mariah);

            var luna = new PlayerGameObject(new ColoredGlyph(Color.Purple, Color.Transparent, 'L'), MagicNumbers.GAMEOBJECT_INDEX);
            luna.Name = "Luna";
            luna.GetComponent<HPComponent>().Max = 150;
            luna.GetComponent<MPComponent>().Max = 50;
            luna.GetComponent<SpeedComponent>().Speed = 75;
            luna.GetComponent<WeaponComponent>().Weapon = WeaponRegistry.Stick();
            luna.GetComponent<ActionComponent>().Configure(ActionType.SKILL, 1); // one sentry per turn
            luna.AddComponent(ResourceRegistry.Metal()); // 100 of 200, +25 a turn. sentries cost 100
            luna.Position = new Point(5, 4);
            settings.AddGameObject(luna);

            var wall = Wall.GenerateWall();
            wall.Position = new Point(4, 4);
            settings.AddGameObject(wall);

            var zombie = new EnemyGameObject(new ColoredGlyph(Color.Green, Color.Transparent, 'z'), MagicNumbers.GAMEOBJECT_INDEX, new ZombieBehavior());
            zombie.Name = "Zombie";
            zombie.GetComponent<HPComponent>().Max = 50;
            zombie.GetComponent<SpeedComponent>().Speed = 50;
            zombie.Position = (10, 5);

            settings.AddGameObject(zombie);

            var necromancer = new EnemyGameObject(new ColoredGlyph(Color.Purple, Color.Transparent, 'N'), MagicNumbers.GAMEOBJECT_INDEX, new SummonerBehavior(SummonRegistry.Skeleton));
            necromancer.Name = "Necromancer";
            necromancer.GetComponent<HPComponent>().Max = 60;
            necromancer.GetComponent<SpeedComponent>().Speed = 60;
            necromancer.GetComponent<ActionComponent>().Configure(ActionType.SPELL, 1);
            necromancer.Position = (15, 10);
            settings.AddGameObject(necromancer);

            // mariah casts one spell per turn
            mariah.GetComponent<ActionComponent>().Configure(ActionType.SPELL, 1);
            mariah.AddComponent(new SpellbookComponent(SpellRegistry.SummonFamiliar(), SpellRegistry.MagicCircle(), SpellRegistry.Fireball()));

            luna.AddComponent(new SpellbookComponent(SpellRegistry.SentryGun(hitsRequired: 3, builtGlyph: 'Y')));

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
