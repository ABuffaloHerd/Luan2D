using LuanSC.Data.Components;
using LuanSC.Data.Specs;
using LuanSC.Objects;
using System;
using System.Collections.Generic;

namespace LuanSC.Data;

public enum CombatOutcome
{
    VICTORY,
    DEFEAT
}

/// <param name="Description">Ready to show to the player, e.g. "Round limit: 3 / 10".</param>
/// <param name="Met">True if this condition has been fulfilled.</param>
public sealed record ConditionStatus(CombatOutcome Outcome, string Description, bool Met);

/// <summary>
/// Everything that can end a fight: the level's own conditions and the fallback.
/// Only tracks and answers, CombatScene decides when to ask and what to do about it.
/// </summary>
public sealed class CombatConditions
{
    private sealed record Condition(CombatOutcome Outcome, Func<string> Description, Func<bool> Met);

    private readonly IReadOnlyList<GameObject> objects;
    private readonly List<Condition> conditions = new();

    public int RoundsCompleted { get; set; }

    /// <summary>
    /// The round being played, starting at 1.
    /// </summary>
    public int Round => RoundsCompleted + 1;

    /// <param name="objects">The live list of units in the fight, already filled with the starting units.</param>
    public CombatConditions(IEnumerable<ConditionSpec> specs, IReadOnlyList<GameObject> objects)
    {
        this.objects = objects;

        foreach (ConditionSpec spec in specs)
            conditions.Add(Build(spec));

        // a side that starts empty has nobody to lose, otherwise a level with no enemies wins on the first turn
        AddFallback(Team.ENEMY, CombatOutcome.VICTORY, "Kill all enemies");
        AddFallback(Team.PLAYER, CombatOutcome.DEFEAT, "Keep a friend alive");
    }

    public IReadOnlyList<ConditionStatus> GetConditions()
        => conditions.Select(c => new ConditionStatus(c.Outcome, c.Description(), c.Met())).ToList();

    /// <summary>
    /// The condition that ends the fight, or null if it carries on. Defeat beats victory if both happen at once.
    /// </summary>
    public ConditionStatus Check()
    {
        List<ConditionStatus> met = GetConditions().Where(c => c.Met).ToList();
        return met.FirstOrDefault(c => c.Outcome == CombatOutcome.DEFEAT) ?? met.FirstOrDefault();
    }

    private Condition Build(ConditionSpec spec)
    {
        switch (spec)
        {
            case ConditionSpec.UnitDead dead:
                List<GameObject> units = objects
                    .Where(obj => string.Equals(obj.Name, dead.UnitName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (units.Count == 0)
                    throw new ArgumentException($"Condition names a unit that isn't in the level: {dead.UnitName}");

                return dead.Outcome == CombatOutcome.VICTORY
                    ? new Condition(dead.Outcome, () => $"Kill {dead.UnitName}", () => units.All(IsGone))
                    : new Condition(dead.Outcome, () => $"Keep {dead.UnitName} alive", () => units.Any(IsGone));

            case ConditionSpec.RoundsCompleted rounds:
                string label = rounds.Outcome == CombatOutcome.VICTORY ? "Survive rounds" : "Round limit";
                return new Condition(rounds.Outcome, () => $"{label}: {RoundsCompleted} / {rounds.Rounds}", () => RoundsCompleted >= rounds.Rounds);

            default:
                throw new ArgumentException($"Unknown condition {spec}");
        }
    }

    private void AddFallback(Team team, CombatOutcome outcome, string description)
    {
        if (!HasLivingUnit(team)) return;
        conditions.Add(new Condition(outcome, () => description, () => !HasLivingUnit(team)));
    }

    private bool IsGone(GameObject unit) => !objects.Contains(unit);

    // anything without HP can't die, so it can't hold a side's fallback open. Neutrals and walls never count
    private bool HasLivingUnit(Team team) => objects.Any(obj =>
        obj.GetComponent<TeamComponent>()?.Team == team
        && obj.GetComponent<HPComponent>() is { IsAlive: true });
}
