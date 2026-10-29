using System;
using System.Collections.Generic;
using System.Text;

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
}
