using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components;

public enum ActionType
{
    MOVE,
    ATTACK,
    ITEM,
    SKILL,
    SPELL,
    SPECIAL
    // add Item, Skill and so on here
}

/// <summary>
/// Limits how many times the owner can do each type of action in one turn.
/// A type that is not configured has no limit.
/// </summary>
public class ActionComponent : IComponent, IHudDisplay
{
    public const int SPEED_PER_MOVE = 10;

    private class Slot
    {
        public int Max;
        public int Current;
        public int Taken;
        public Func<GameObject, int> Formula; // null means fixed maximum
    }

    private readonly Dictionary<ActionType, Slot> slots = new();

    public string Name => "Action Component";
    public GameObject Owner { get; private set; }

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
    }

    public IEnumerable<ActionType> Types => slots.Keys;

    /// <summary>
    /// Standard setup: speed-based movement, one attack and one skill. Change them per unit with ActionLimits.
    /// </summary>
    public static ActionComponent CreateDefault()
    {
        return new ActionComponent()
            .Configure(ActionType.MOVE, owner =>
            {
                SpeedComponent speed = owner.GetComponent<SpeedComponent>();
                return (speed is not null && speed.CanAct)
                    ? Math.Max(1, speed.Speed / SPEED_PER_MOVE)
                    : 0;
            })
            .Configure(ActionType.ATTACK, 1)
            .Configure(ActionType.SKILL, 1);
    }

    public ActionComponent Configure(ActionType type, int max)
    {
        slots[type] = new Slot { Max = max, Current = max };
        return this;
    }

    public ActionComponent Configure(ActionType type, Func<GameObject, int> formula)
    {
        slots[type] = new Slot { Formula = formula };
        return this;
    }

    public bool IsLimited(ActionType type) => slots.ContainsKey(type);

    // only movement and attacks for now, skill and spell counters would push resources off the HUD
    public IEnumerable<HudBar> GetHudBars()
    {
        if (IsLimited(ActionType.MOVE))
            yield return new HudBar(50, "MV :", Color.Green, Color.Green, HudBar.Ratio(Remaining(ActionType.MOVE), Max(ActionType.MOVE)), $"{Remaining(ActionType.MOVE)} / {Max(ActionType.MOVE)}", Color.Black);

        if (IsLimited(ActionType.ATTACK))
            yield return new HudBar(51, "AT :", Color.Orange, Color.Orange, HudBar.Ratio(Remaining(ActionType.ATTACK), Max(ActionType.ATTACK)), $"{Remaining(ActionType.ATTACK)} / {Max(ActionType.ATTACK)}", Color.Black);
    }

    public int Max(ActionType type) => slots.TryGetValue(type, out Slot slot) ? slot.Max : int.MaxValue;
    public int Remaining(ActionType type) => slots.TryGetValue(type, out Slot slot) ? slot.Current : int.MaxValue;
    public int Taken(ActionType type) => slots.TryGetValue(type, out Slot slot) ? slot.Taken : 0;

    public bool CanSpend(ActionType type, int cost = 1)
        => !slots.TryGetValue(type, out Slot slot) || slot.Current >= cost;

    /// <summary>
    /// Removes points from one counter.
    /// </summary>
    /// <returns>false if there are not enough points. The counter does not change in this case.</returns>
    public bool TrySpend(ActionType type, int cost = 1)
    {
        if (!slots.TryGetValue(type, out Slot slot)) return true; // no limit

        if (slot.Current < cost) return false;

        slot.Current -= cost;
        slot.Taken++;
        return true;
    }

    /// <summary>
    /// Calculates each maximum and refills all counters.
    /// Call this at the start of the owner's turn, after effects tick.
    /// </summary>
    public void Refill()
    {
        foreach (Slot slot in slots.Values)
        {
            if (slot.Formula is not null && Owner is not null)
                slot.Max = slot.Formula(Owner);

            slot.Current = slot.Max;
            slot.Taken = 0;
        }
    }
}
