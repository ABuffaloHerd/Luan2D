namespace LuanSC.Data.Definitions;

/// <summary>
/// An extra way for a level to end, on top of the fallback (all friends dead or all enemies dead).
/// The outcome says which way it ends: UnitDead with VICTORY is a kill objective, with DEFEAT a protect objective.
/// RoundsCompleted with VICTORY is survive N rounds, with DEFEAT a time limit.
/// </summary>
public abstract record ConditionDefinition(CombatOutcome Outcome)
{
    /// <summary>
    /// Units are found by name once at the start of the fight, so a mech form still counts as its pilot and later summons don't count.
    /// If several share the name, VICTORY needs all of them dead and DEFEAT only needs one.
    /// </summary>
    public sealed record UnitDead(CombatOutcome Outcome, string UnitName) : ConditionDefinition(Outcome);

    /// <summary>
    /// Met when that many rounds have fully finished. A kill on the last turn of the last round still counts first.
    /// </summary>
    public sealed record RoundsCompleted(CombatOutcome Outcome, int Rounds) : ConditionDefinition(Outcome);
}
