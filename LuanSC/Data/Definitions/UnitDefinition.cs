using LuanSC.Data.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Definitions;

public enum UnitKind
{
    PLAYER, // friends
    ENEMY, // kill
    WALL,  // wall man
    FURNITURE // wall man but you can break it
}

/// <summary>
/// Describes one unit. CombatSettings.BuildObjects turns it into a GameObject, a fresh one every fight.
/// Weapons, spells, AI and effects are named by their ID in IdDictionary so a level file can refer to them later.
/// Anything left null keeps the default from PlayerGameObject or EnemyGameObject.
/// </summary>
public sealed record UnitDefinition
{
    public UnitKind Kind { get; init; } = UnitKind.ENEMY;

    /// <summary>
    /// Human, undead and so on. Flags, e.g. UnitType.HUMAN | UnitType.UNDEAD.
    /// </summary>
    public UnitType UnitType { get; init; } = UnitType.UNKNOWN;
    public string Name { get; init; }
    public char Glyph { get; init; } = '?'; // walls ignore this
    public Color Colour { get; init; } = Color.White;
    public Point Position { get; init; }

    /// <summary>
    /// Max HP. Units start at full health.
    /// </summary>
    public int? Hp { get; init; }
    public int? Speed { get; init; }

    /// <summary>
    /// Changes existing resources (MP, SP, OD) or adds missing ones (Metal).
    /// </summary>
    public List<ResourceDefinition> Resources { get; init; } = new();

    /// <summary>
    /// Per-turn limits on top of the defaults, e.g. SPELL = 1.
    /// </summary>
    public Dictionary<ActionType, int> ActionLimits { get; init; } = new();

    /// <summary>
    /// Built-in elemental traits in percent: positive is a weakness, negative a resistance, e.g. [Element.Fire] = -50.
    /// </summary>
    public Dictionary<Element, int> ElementModifiers { get; init; } = new();

    public string Weapon { get; init; }

    /// <summary>
    /// Spell IDs for the spellbook. Empty means no spellbook.
    /// </summary>
    public List<string> Spells { get; init; } = new();

    /// <summary>
    /// Enemies only. Null means idle.
    /// </summary>
    public AiDefinition Ai { get; init; }

    /// <summary>
    /// Applied in order at the start of combat, so Vulnerable before Armour is not the same as Armour before Vulnerable.
    /// </summary>
    public List<EffectDefinition> Effects { get; init; } = new();
}

/// <summary>
/// A resource on one unit. Changes the unit's existing one of that type, or adds it if they don't have it.
/// Everything is optional so a unit can just bump its MP, or carry a fully customised pool.
/// Null keeps the default: the unit's own component, the IdDictionary factory, or failing both a plain white pool of 100.
/// </summary>
public sealed record ResourceDefinition(ResourceType Type)
{
    public int? Max { get; init; }

    /// <summary>
    /// Null starts full if Max was given, otherwise keeps the default (OD starts empty, Metal at 100 of 200).
    /// </summary>
    public int? Current { get; init; }

    /// <summary>
    /// Added every turn. Negative drains.
    /// </summary>
    public int? Regen { get; init; }

    public string DisplayName { get; init; } // fight feed, e.g. "Metal"
    public string Label { get; init; }       // HUD, 3 characters max, e.g. "MT"
    public Color? Colour { get; init; }      // bar and label
    public Color? TextColour { get; init; }  // the numbers drawn over the bar
    public int? HudOrder { get; init; }
}

/// <summary>
/// Value is whatever number the effect takes: damage for Poison, percent for Empower, Weaken and Vulnerable, amount for Armour,
/// percent for Elemental (positive weakness, negative resistance).
/// </summary>
public sealed record EffectDefinition(string Id, int Duration, int Value = 0)
{
    /// <summary>
    /// Only used by Elemental for now, e.g. new("Elemental", -1, 50) { Element = Element.Electric }.
    /// </summary>
    public Element Element { get; init; }
}

/// <summary>
/// Minion is a summon ID, only used by Summoner.
/// </summary>
public sealed record AiDefinition(string Behavior, string Minion = null)
{
    /// <summary>
    /// Spawner only. Summon IDs to pick from at random, list one twice to make it twice as likely.
    /// </summary>
    public List<string> Spawns { get; init; } = new();

    /// <summary>
    /// Spawner only. Its own turns between spawns, 3 spawns on turns 1, 4, 7.
    /// </summary>
    public int Interval { get; init; } = 1;

    /// <summary>
    /// Spawner only. Its own turns to wait before the first spawn, 0 spawns on its first turn.
    /// </summary>
    public int InitialDelay { get; init; }
}
