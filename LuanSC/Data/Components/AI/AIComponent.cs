using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components.AI;

public interface IAIBehavior
{
    /// <summary>
    /// Plans the actions for one turn. The scene adds EndTurn at the end.
    /// </summary>
    IReadOnlyList<TurnAction> PlanTurn(GameObject self, ICombatContext context);
}

public class AIComponent : IComponent
{
    public string Name => "AI Component";
    public GameObject Owner { get; private set; }

    public IAIBehavior Behavior { get; set; }

    /// <summary>
    /// Time between two actions. This lets the player see each action.
    /// </summary>
    public TimeSpan ActionDelay { get; set; } = TimeSpan.FromMilliseconds(400);

    public AIComponent(IAIBehavior behavior)
    {
        Behavior = behavior;
    }

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
    }
}
