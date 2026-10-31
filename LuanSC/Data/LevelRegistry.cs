using LuanSC.Data.Components;
using LuanSC.Data.Specs;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data;

/// <summary>
/// Level factories. Each returns plain settings, CombatScene builds the units from them.
/// </summary>
public static class LevelRegistry
{
    public static CombatSettings Test() => new()
    {
        Units =
        [
            new UnitSpec
            {
                Kind = UnitType.PLAYER,
                Name = "Hirina",
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

            new UnitSpec
            {
                Kind = UnitType.ENEMY,
                Name = "Target Dummy",
                Glyph = 'T',
                Colour = Color.White,
                Position = new(5, 5),
                Hp = 100,
                Speed = 50,
            },

            new UnitSpec
            {
                Kind = UnitType.PLAYER,
                Name = "Minako",
                Glyph = 'M',
                Colour = Color.Gray,
                Position = new(2, 2),
                Hp = 100,
                Speed = 90,
                Weapon = "Shotgun",
                Resources =
                [
                    new(ResourceType.MP) { Max = 50 },
                    new(ResourceType.SP) { Max = 75 },
                    new(ResourceType.AMMO), // 2 shells
                ],
                Spells = ["Reload"],
                Effects = [new("Barrier", Duration: 1)],
            },

            new UnitSpec
            {
                Kind = UnitType.PLAYER,
                Name = "Mariah",
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

            new UnitSpec
            {
                Kind = UnitType.PLAYER,
                Name = "Luna",
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
                Spells = ["SentryGun", "TitanAplu"],
            },

            new UnitSpec { Kind = UnitType.WALL, Position = new(4, 4) },

            new UnitSpec
            {
                Kind = UnitType.ENEMY,
                Name = "Zombie",
                Glyph = 'z',
                Colour = Color.Green,
                Position = new(10, 5),
                Hp = 50,
                Speed = 50,
                Ai = new("Zombie"),
                ElementModifiers = { [Element.Fire] = 50 }, // weak to fire
            },

            new UnitSpec
            {
                Kind = UnitType.ENEMY,
                Name = "Necromancer",
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
        //FontScale = 1.9f,
    };
}
