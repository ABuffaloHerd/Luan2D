using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components;

public enum Team
{
    PLAYER,
    ENEMY,
    NEUTRAL // non aggressive, but fights back if you piss it off.
}

/// <summary>
/// Which side is the owner on?
/// Objects without this component are not hostile to anyone.
/// </summary>
public class TeamComponent : IComponent
{
    public string Name => "Team Component";
    public GameObject Owner { get; private set; }

    public Team Team { get; set; }

    public TeamComponent(Team team)
    {
        Team = team;
    }

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
    }

    public static bool AreHostile(GameObject a, GameObject b)
    {
        TeamComponent teamA = a?.GetComponent<TeamComponent>();
        TeamComponent teamB = b?.GetComponent<TeamComponent>();

        if (teamA is null || teamB is null) return false;
        if (teamA.Team == Team.NEUTRAL || teamB.Team == Team.NEUTRAL) return false;

        return teamA.Team != teamB.Team;
    }
}
