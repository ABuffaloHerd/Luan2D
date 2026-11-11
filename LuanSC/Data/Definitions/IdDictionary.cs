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

namespace LuanSC.Data.Definitions;

/// <summary>
/// Maps the IDs used in specs to the registries. Add a line here when you add something to a registry,
/// or levels won't be able to use it. IDs ignore case.
/// </summary>
public static class IdDictionary
{
    private static readonly Dictionary<string, Func<Weapon>> weapons = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Katana"]          = WeaponRegistry.Katana,
        ["Throngler"]       = WeaponRegistry.Throngler,
        ["Gun"]             = WeaponRegistry.Gun,
        ["Crossbow"]        = WeaponRegistry.Crossbow,
        ["Shotgun"]         = WeaponRegistry.Shotgun,
        ["Stick"]           = WeaponRegistry.Stick,
        ["SentryGun"]       = WeaponRegistry.SentryGun,
        ["Hands"]           = WeaponRegistry.Hands,
        ["HydraulicFists"]  = WeaponRegistry.HydraulicFists,
        ["ManaLens"]        = WeaponRegistry.ManaLens,
        ["RocketLauncher"]  = WeaponRegistry.RocketLauncher,
    };

    private static readonly Dictionary<string, Func<ISpell>> spells = new(StringComparer.OrdinalIgnoreCase)
    {
        ["SummonFamiliar"]  = SpellRegistry.SummonFamiliar,
        ["MagicCircle"]     = SpellRegistry.MagicCircle,
        ["SentryGun"]       = () => SpellRegistry.SentryGun(),
        ["Dispenser"]       = () => SpellRegistry.Dispenser(),
        ["Fireball"]        = () => SpellRegistry.Fireball(),
        ["Unsheath"]        = () => SpellRegistry.Unsheath(),
        ["TitanAplu"]       = () => SpellRegistry.TitanAplu(),
        ["Reload"]          = () => SpellRegistry.Reload(),
        ["Heal"]            = () => SpellRegistry.Heal(),
        ["Railgun"]         = () => SpellRegistry.Railgun(),
        ["Teleport"]        = () => SpellRegistry.Teleport(),
    };

    private static readonly Dictionary<string, Func<GameObject, SummonedGameObject>> summons = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Skeleton"]            = SummonRegistry.Skeleton,
        ["Zombie"]              = SummonRegistry.Zombie,
        ["Familiar"]            = SummonRegistry.Familiar,
        ["MagicCircle"]         = SummonRegistry.MagicCircle,
        ["SentryGun"]           = owner => SummonRegistry.SentryGun(owner),
        ["Dispenser"]           = owner => SummonRegistry.Dispenser(owner),
    };

    private static readonly Dictionary<string, Func<EffectDefinition, IEffect>> effects = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Poison"]      = e => new PoisonEffect(e.Duration, e.Value),
        ["Empower"]     = e => new EmpowerEffect(e.Duration, e.Value),
        ["Weaken"]      = e => new WeakenEffect(e.Duration, e.Value),
        ["Vulnerable"]  = e => new VulnerableEffect(e.Duration, e.Value),
        ["Armour"]      = e => new ArmourEffect(e.Duration, e.Value),
        ["MagicArmour"] = e => new MagicArmourEffect(e.Duration, e.Value),
        ["Barrier"]     = e => new BarrierEffect { Duration = e.Duration },
        // presets, Value is ignored. Each level doubles the damage and radius
        ["Volatile1"]   = e => new VolatileEffect(e.Duration, damage: 40, radius: 2),
        ["Volatile2"]   = e => new VolatileEffect(e.Duration, damage: 80, radius: 4),
        ["Volatile3"]   = e => new VolatileEffect(e.Duration, damage: 160, radius: 8),
        ["Elemental"]   = e => new ElementalModifierEffect(e.Duration, e.Element, e.Value),
    };

    // resources a unit doesn't have by default, e.g. Metal on an engineer
    private static readonly Dictionary<ResourceType, Func<ResourceComponent>> resources = new()
    {
        [ResourceType.MP]       = () => new MPComponent(),
        [ResourceType.SP]       = () => new SPComponent(),
        [ResourceType.OD]       = () => new ODComponent(),
        [ResourceType.GB]       = () => new GBComponent(),
        [ResourceType.METAL]    = () => ResourceRegistry.Metal(),
        [ResourceType.AMMO]     = () => ResourceRegistry.Ammo(),
    };

    public static Weapon Weapon(string id) => Find(weapons, id, "weapon")();
    public static ISpell Spell(string id) => Find(spells, id, "spell")();
    public static Func<GameObject, SummonedGameObject> Summon(string id) => Find(summons, id, "summon");
    public static IEffect Effect(EffectDefinition spec) => Find(effects, spec.Id, "effect")(spec);

    /// <summary>
    /// A new resource of this type. Types without a factory get a plain pool, the spec fills in the rest.
    /// </summary>
    public static ResourceComponent Resource(ResourceType type)
        => resources.TryGetValue(type, out var make) ? make() : new ResourceComponent(type, type.ToString(), type.ToString(), Color.White, 100);

    public static IAIBehavior Ai(AiDefinition spec)
    {
        if (spec is null) return null; // no AI, no turns. Use "Idle" for a unit that takes turns and does nothing

        return spec.Behavior.ToLowerInvariant() switch
        {
            "idle" => new IdleBehavior(),
            "zombie" => new ZombieBehavior(),
            "summoner" => new SummonerBehavior(Summon(spec.Minion ?? throw new ArgumentException("Summoner AI needs a Minion."))),
            "spawner" => new SpawnerBehavior(spec.Spawns.Select(Summon).ToList(), spec.Interval, spec.InitialDelay, spec.Count),
            _ => throw new ArgumentException($"Unknown AI '{spec.Behavior}'. Did you forget to add it to IdDictionary.Ai?")
        };
    }

    private static T Find<T>(Dictionary<string, T> table, string id, string what)
    {
        if (id is not null && table.TryGetValue(id, out T found)) return found;
        throw new ArgumentException($"Unknown {what} '{id}'. Known: {string.Join(", ", table.Keys)}.\nDid you forget to register it in IdDictionary?");
    }
}
