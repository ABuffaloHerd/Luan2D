using LuanSC.Data.Components;
using LuanSC.Data.Specs;
using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data
{
    /// <summary>
    /// Describes a combat scenario. Only data, so the same settings can start any number of fights.
    /// BuildObjects makes the actual units. Levels live in LevelRegistry.
    /// </summary>
    // TODO: Background tiles and audio.
    public sealed record CombatSettings
    {
        public List<UnitSpec> Units { get; init; } = new();

        /// <summary>
        /// Applied to every unit that can have effects, after their own. Each unit gets its own copy, e.g. a weather effect.
        /// </summary>
        public List<EffectSpec> StartEffects { get; init; } = new();

        // default arena size, fills the view exactly. Bigger scrolls, smaller leaves a gap
        public int Width { get; init; } = GameSettings.ARENA_VIEW_SIZE;
        public int Height { get; init; } = GameSettings.ARENA_VIEW_SIZE;

        public float FontScale { get; init; } = 1;

        /// <summary>
        /// New game objects for every unit. Call once per fight, the objects keep their state.
        /// </summary>
        public List<GameObject> BuildObjects()
        {
            List<GameObject> objects = new();

            foreach (UnitSpec unit in Units)
            {
                GameObject obj = BuildUnit(unit);

                EffectComponent effects = obj.GetComponent<EffectComponent>();
                if (effects is not null)
                {
                    foreach (EffectSpec effect in unit.Effects.Concat(StartEffects))
                        effects.Apply(SpecLookup.Effect(effect)); // a new effect every time, they count down their own duration
                }

                objects.Add(obj);
            }

            return objects;
        }

        private static GameObject BuildUnit(UnitSpec unit)
        {
            ColoredGlyph glyph = new(unit.Colour, Color.Transparent, unit.Glyph);

            GameObject obj = unit.Kind switch
            {
                UnitType.PLAYER => new PlayerGameObject(glyph, MagicNumbers.GAMEOBJECT_INDEX),
                UnitType.ENEMY => new EnemyGameObject(glyph, MagicNumbers.GAMEOBJECT_INDEX, SpecLookup.Ai(unit.Ai)),
                UnitType.WALL => Wall.GenerateWall(unit.Colour),
                _ => throw new ArgumentException($"Unknown unit kind {unit.Kind}")
            };

            obj.Position = unit.Position;
            if (unit.Name is not null) obj.Name = unit.Name;

            // start at full health
            if (unit.Hp is int hp && obj.GetComponent<HPComponent>() is HPComponent health)
            {
                health.Max = hp;
                health.Current = hp;
            }

            if (unit.Speed is int speed && obj.GetComponent<SpeedComponent>() is SpeedComponent speedComponent)
                speedComponent.Speed = speed;

            foreach (ResourceSpec spec in unit.Resources)
                ApplyResource(obj, spec);

            ActionComponent actions = obj.GetComponent<ActionComponent>();
            foreach ((ActionType type, int max) in unit.ActionLimits)
                actions?.Configure(type, max);

            if (unit.Weapon is not null && obj.GetComponent<WeaponComponent>() is WeaponComponent weapon)
                weapon.Weapon = SpecLookup.Weapon(unit.Weapon);

            if (unit.Spells.Count > 0)
                obj.AddComponent(new SpellbookComponent(unit.Spells.Select(SpecLookup.Spell).ToArray()));

            return obj;
        }

        private static void ApplyResource(GameObject obj, ResourceSpec spec)
        {
            ResourceComponent resource = obj.GetResource(spec.Type);
            if (resource is null)
            {
                resource = SpecLookup.Resource(spec.Type);
                obj.AddComponent(resource);
            }

            if (spec.DisplayName is not null) resource.DisplayName = spec.DisplayName;
            if (spec.Label is not null) resource.Label = spec.Label;
            if (spec.Colour is Color colour)
            {
                resource.BarColor = colour;
                resource.LabelColor = colour;
            }
            if (spec.TextColour is Color text) resource.TextColor = text;
            if (spec.HudOrder is int order) resource.HudOrder = order;
            if (spec.Regen is int regen) resource.RegenPerTurn = regen;

            if (spec.Max is int max)
            {
                resource.Max = max;
                resource.Current = max; // start full unless told otherwise
            }

            if (spec.Current is int current) resource.Current = current;
        }
    }
}
