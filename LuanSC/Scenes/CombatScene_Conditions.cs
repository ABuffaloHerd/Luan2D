using LuanSC.Data;
using SadConsole.UI;
using SadConsole.UI.Controls;
using System;
using System.Collections.Generic;
using LuanSC.Data.Combat;

namespace LuanSC.Scenes;

/// <summary>
/// Win and lose conditions, and what happens when one is met.
/// </summary>
public partial class CombatScene
{
    // long enough for the killing blow and its death message to land
    private static readonly TimeSpan ResultDelay = TimeSpan.FromSeconds(1);

    private CombatConditions conditions;
    private ConditionStatus fightResult; // non-null once the fight is over
    private TimeSpan resultTimer = TimeSpan.Zero;
    private bool resultShown;

    public int Round => conditions.Round;

    public IReadOnlyList<ConditionStatus> GetConditions() => conditions.GetConditions();

    private void AnnounceRound()
    {
        fightFeed.AddLine(" ");
        fightFeed.AddLine($"--- Round {Round} ---", Color.PeachPuff);
        fightFeed.AddLine(" ");
    }

    private void CheckConditions()
    {
        if (fightResult is not null) return;

        ConditionStatus result = conditions.Check();
        if (result is not null)
            EndFight(result);
    }

    private void EndFight(ConditionStatus result)
    {
        fightResult = result;

        // no current unit stops the AI and player input
        currentControlledGameObject = null;
        aiPlan = null;
        queue.Clear();
        CancelTargeting();
        overlay.Clear();
        hud.Reset();
        RefreshTurnOrder();

        GameEvents.Publish(new CombatEvent.FightEnded(result.Outcome, result.Description));
    }

    /// <summary>
    /// Opens the result window from Update. The fight can end while the scene is still being built, and an Inspect window could be open.
    /// </summary>
    private void UpdateFightEnd(TimeSpan delta)
    {
        if (fightResult is null || resultShown) return;

        resultTimer += delta;
        if (resultTimer < ResultDelay || windowInstances > 0) return;

        resultShown = true;
        ShowResultWindow();
    }

    private void ShowResultWindow()
    {
        bool victory = fightResult.Outcome == CombatOutcome.VICTORY;

        Window window = new(40, 9)
        {
            Title = victory ? "Victory" : "Defeat",
            CanDrag = false,
            UseMouse = true,
            CloseOnEscKey = false,
        };

        windowInstances++;
        window.Closed += (sender, args) => closedWindows.Add(window);

        int width = window.Width - 4;
        window.Controls.Add(new Label(width) { DisplayText = fightResult.Description, Position = new(2, 2), TextColor = victory ? Color.Green : Color.Red });
        window.Controls.Add(new Label(width) { DisplayText = $"Ended on round {Round}", Position = new(2, 3), TextColor = Color.Gray });

        Button menu = new(width, 1)
        {
            Text = "Back",
            Position = new(2, 5),
        };
        menu.Click += (sender, args) =>
        {
            window.Hide(); // otherwise the modal window outlives the scene
            manager.PopScene();
        };
        window.Controls.Add(menu);

        window.Position = new((GameSettings.GAME_WIDTH / 2) - (window.Width / 2) - 1, (GameSettings.GAME_HEIGHT / 2) - (window.Height / 2) + 1);
        window.Show(true);
    }
}
