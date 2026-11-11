using LuanSC.Data.Components;
using LuanSC.Data.Definitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data;

/// <summary>
/// Level factories. Each returns plain settings, CombatScene builds the units from them.
/// Use the [Battle] attribute to make them show up in the battle selection scene.
/// </summary>
public static class LevelRegistry
{
    [Battle("Enigmatic Exercise: Minako", BattleCategory.PARADOX, Order = 4)]
    public static CombatSettings MinakoP() => new()
    {
        Units =
        [
            new UnitDefinition
            {
                Kind = UnitKind.PLAYER,
                Name = "Minako",
                UnitType = UnitType.HUMAN,
                Glyph = 'M',
                Colour = Color.Gray,
                Position = new(1, 1),
                Hp = 150,
                Speed = 90,
                Weapon = "Crossbow",
                Resources =
                [
                    new(ResourceType.MP) { Max = 50 },
                    new(ResourceType.SP) { Max = 75 },
                    new(ResourceType.AMMO) { Max = 1 }, // 1 bolt
                ],
                Spells = ["Reload", "Railgun", "Teleport"],
            },
            new UnitDefinition
            {
                Kind = UnitKind.ENEMY,
                Name = "Target Dummy",
                UnitType = UnitType.MECHANICAL, // a construct, not alive
                Glyph = 'T',
                Colour = Color.White,
                Position = new(5, 5),
                Hp = 100,
                Speed = 50,
            }
        ],

        Width = 45,
        Height = 45,
        Description = "Minako excels at single target damage but struggles when it gets close and personal. Despite being bad at magic, she has tools to get out such situaions. Use this to your advantage."
        // TODO: teleport spell, barrier spell
    };

    [Battle("Spawner Test", BattleCategory.TEST, Order = 3)]
    public static CombatSettings SpawnerTest() => new()
    {
        Units =
        [
            new UnitDefinition
            {
                Kind = UnitKind.PLAYER,
                Name = "Hirina",
                UnitType = UnitType.HUMAN,
                Glyph = 'H',
                Colour = Color.Red,
                Position = new(5, 5),
                Hp = 250,
                Speed = 100,
                Weapon = "Katana",
            },
            new UnitDefinition
            {
                Kind = UnitKind.ENEMY,
                Name = "Spawner",
                UnitType = UnitType.MAGICAL,
                Glyph = (char)0x08,
                Colour = Color.Purple,
                Position = new(15, 15),
                Hp = 100,
                Speed = 50,
                Ai = new("Spawner") { Spawns = ["Skeleton", "Zombie"], Interval = 3 },
            },
        ],

        Width = 45,
        Height = 45,
        Description = "A spawner that summons a skeleton or a zombie every 3 of its turns. Kill it and they go with it.",
    };

    [Battle("Penguin Test", BattleCategory.TEST, Order = 4)]
    public static CombatSettings PenguinTest()
    {
        static UnitDefinition Penguin(int x, int y) => new()
        {
            Kind = UnitKind.ENEMY,
            Name = "Penguin",
            UnitType = UnitType.BEAST,
            Glyph = 'p',
            Colour = Color.LightBlue,
            Position = new(x, y),
            Hp = 10,
            Speed = 60,
            Ai = new("Zombie"),
            Effects = [new("Volatile", Duration: -1, Value: 40)],
        };

        return new()
        {
            Units =
            [
                new UnitDefinition
                {
                    Kind = UnitKind.PLAYER,
                    Name = "Hirina",
                    UnitType = UnitType.HUMAN,
                    Glyph = 'H',
                    Colour = Color.Red,
                    Position = new(5, 5),
                    Hp = 25,
                    Speed = 100,
                    Weapon = "Katana",
                },

                // close enough to chain
                Penguin(10, 10),
                Penguin(12, 10),
                Penguin(14, 10),

                // on its own, blows up with nothing around it
                Penguin(25, 25),
            ],

            Width = 45,
            Height = 45,
            Description = "These seemingly pleasant birds have adapted perfectly to the hostile environment. But explorers beware! Few are prepared for the consequences when penguins TURN!",
        };
    }

    [Battle("Condition Test", BattleCategory.TEST, Order = 1)]
    public static CombatSettings ConditionTest() => new()
    {
        Units =
        [
            new UnitDefinition
            {
                Kind = UnitKind.PLAYER,
                Name = "Hirina",
                UnitType = UnitType.HUMAN,
                Glyph = 'H',
                Colour = Color.Red,
                Position = new(1, 1),
                Hp = 250,
                Speed = 100,
                Resources =
                [
                    new(ResourceType.MP) { Max = 100 },
                    new(ResourceType.OD) { Current = 100 }, // starts full so Unsheath can be used straight away
                    new(ResourceType.GB) { Current = 50 },  // Unsheath takes the 50 GB first, then 50 OD
                ],
                ActionLimits = { [ActionType.SPECIAL] = 1 },
                Weapon = "Throngler",
                Spells = ["Unsheath", "Fireball"],
            },
            new UnitDefinition
            {
                Kind = UnitKind.ENEMY,
                Name = "Target Dummy",
                UnitType = UnitType.MECHANICAL, // a construct, not alive
                Glyph = 'T',
                Colour = Color.White,
                Position = new(5, 5),
                Hp = 100,
                Speed = 50,
            },
            
            new UnitDefinition
            {
                Kind = UnitKind.FURNITURE,
                Name = "Table",
                UnitType = UnitType.FURNITURE,
                Glyph = (char)210,
                Colour = Color.Brown,
                Position = new(3, 3),
                Hp = 10,
            }
        ],
        Width = 45,
        Height = 45,
    };
    [Battle("Test Arena", BattleCategory.TEST, Order = 2)]
    public static CombatSettings Test() => new()
    {
        Units =
        [
            new UnitDefinition
            {
                Kind = UnitKind.PLAYER,
                Name = "Hirina",
                UnitType = UnitType.HUMAN,
                Glyph = 'H',
                Colour = Color.Red,
                Position = new(1, 1),
                Hp = 250,
                Speed = 100,
                Resources =
                [
                    new(ResourceType.MP) { Max = 100 },
                    new(ResourceType.OD) { Current = 100 }, // starts full so Unsheath can be used straight away
                    new(ResourceType.GB) { Current = 50 },  // Unsheath takes the 50 GB first, then 50 OD
                ],
                ActionLimits = { [ActionType.SPECIAL] = 1 },
                Weapon = "Throngler",
                Spells = ["Unsheath", "Fireball"],

                // damage modifier test. Outgoing: +50% then -20%. Incoming, after RES: +25% then -5.
                // Effects run in the order they're applied, so Vulnerable scales before Armour takes its 5 off.
                Effects =
                [
                    new("Poison", Duration: 10, Value: 10),
                    new("Empower", Duration: 5, Value: 50),
                    new("Weaken", Duration: 5, Value: 20),
                    new("Vulnerable", Duration: 5, Value: 25),
                    new("Armour", Duration: 5, Value: 5),
                ],
            },

            new UnitDefinition
            {
                Kind = UnitKind.ENEMY,
                Name = "Target Dummy",
                UnitType = UnitType.MECHANICAL, // a construct, not alive
                Glyph = 'T',
                Colour = Color.White,
                Position = new(5, 5),
                Hp = 100,
                Speed = 50,
            },

            new UnitDefinition
            {
                Kind = UnitKind.PLAYER,
                Name = "Minako",
                UnitType = UnitType.HUMAN,
                Glyph = 'M',
                Colour = Color.Gray,
                Position = new(2, 2),
                Hp = 100,
                Speed = 90,
                Weapon = "Crossbow",
                Resources =
                [
                    new(ResourceType.MP) { Max = 50 },
                    new(ResourceType.SP) { Max = 75 },
                    new(ResourceType.AMMO), // 2 shells
                ],
                Spells = ["Reload"],
                Effects = [new("Barrier", Duration: 1)],
            },

            new UnitDefinition
            {
                Kind = UnitKind.PLAYER,
                Name = "Mariah",
                UnitType = UnitType.HUMAN,
                Glyph = 'M',
                Colour = Color.Blue,
                Position = new(3, 3),
                Hp = 150,
                Speed = 80,
                Resources = [new(ResourceType.MP) { Max = 200, Current = 75 }],
                ActionLimits = { [ActionType.SPELL] = 1 }, // one spell per turn
                Weapon = "Stick",
                Spells = ["SummonFamiliar", "MagicCircle", "Fireball"],
            },

            new UnitDefinition
            {
                Kind = UnitKind.PLAYER,
                Name = "Luna",
                UnitType = UnitType.HUMAN,
                Glyph = 'L',
                Colour = Color.Purple,
                Position = new(5, 4),
                Hp = 150,
                Speed = 75,
                Resources =
                [
                    new(ResourceType.MP) { Max = 50 },
                    new(ResourceType.METAL), // 100 of 200, +25 a turn. sentries cost 100
                    new(ResourceType.OD) { Current = 100 }, // both full so the mech can be tested straight away
                    new(ResourceType.GB) { Current = 100 },
                ],
                ActionLimits = { [ActionType.SKILL] = 1, [ActionType.SPECIAL] = 1 }, // one sentry and one special per turn
                Weapon = "Stick",
                Spells = ["SentryGun", "Dispenser", "TitanAplu"],
            },

            new UnitDefinition
            {
                Kind = UnitKind.PLAYER,
                Name = "Yin Ling",
                UnitType = UnitType.HUMAN,
                Glyph = 'Y',
                Colour = Color.Cyan,
                Position = new(3, 5),
                Hp = 200,
                Speed = 70,
                Resources =
                [
                    new(ResourceType.MP) { Max = 300 },
                    new(ResourceType.SP) { Max = 100 },
                    new(ResourceType.OD), // starts full so Unsheath can be used straight away
                    new(ResourceType.GB),  // Unsheath takes the 50 GB first, then 50 OD
                ],
                ActionLimits = { [ActionType.SPECIAL] = 1 , [ActionType.SPELL] = 2 },
                Weapon = "ManaLens",
                Spells = ["Heal"],
            },

            new UnitDefinition { Kind = UnitKind.WALL, Position = new(4, 4) },

            new UnitDefinition
            {
                Kind = UnitKind.ENEMY,
                Name = "Zombie",
                UnitType = UnitType.UNDEAD,
                Glyph = 'z',
                Colour = Color.Green,
                Position = new(10, 5),
                Hp = 50,
                Speed = 50,
                Ai = new("Zombie"),
                ElementModifiers = { [Element.Fire] = 50 }, // weak to fire
            },

            new UnitDefinition
            {
                Kind = UnitKind.ENEMY,
                Name = "Necromancer",
                UnitType = UnitType.HUMAN,
                Glyph = 'N',
                Colour = Color.Purple,
                Position = new(15, 10),
                Hp = 60,
                Speed = 60,
                ActionLimits = { [ActionType.SPELL] = 1 },
                Ai = new("Summoner", Minion: "Skeleton"),
            },
        ],

        Width = 45,
        Height = 45,
        Description = "The original test arena."
        //FontScale = 1.9f,
    };
}
