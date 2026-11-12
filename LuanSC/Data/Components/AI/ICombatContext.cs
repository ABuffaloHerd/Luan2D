using System;
using System.Collections.Generic;
using System.Text;
using LuanSC.Data.Combat;

namespace LuanSC.Data.Components.AI;

using LuanSC.Data.Animations;
using LuanSC.Objects;

public interface ICombatContext
{
    IReadOnlyList<GameObject> Objects { get; }
    bool IsWalkable(Point position);

    /// <summary>
    /// Plays an animation on the arena. Fire and forget, it removes itself when done.
    /// </summary>
    void PlayAnimation(AnimatedEffect animation);

    /// <summary>
    /// Adds an object to the arena mid fight, e.g. an item dropped on the floor. Set its Position first.
    /// </summary>
    void Spawn(GameObject obj);

    /// <summary>
    /// The round being played, starting at 1.
    /// </summary>
    int Round { get; }

    /// <summary>
    /// Every condition that can end this fight, the fallback included, with its current progress.
    /// </summary>
    IReadOnlyList<ConditionStatus> GetConditions();
}
