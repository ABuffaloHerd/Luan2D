using LuanSC.Scenes.Transitions;
using System;

namespace LuanSC.Scenes;

/// <summary>
/// Runs the transition between scenes. ChangeScene starts it, SwapScene runs at the midpoint.
/// To make a new transition see ISceneTransition.
/// </summary>
public partial class SceneManager
{
    private enum Transition
    {
        NONE,
        FADE_OUT,
        FADE_IN
    }

    private Transition transition = Transition.NONE;
    private TimeSpan transitionTimer;
    private ISceneTransition activeTransition;

    // Sits on top of the scene during a transition. Takes the keyboard and mouse too, so nothing reaches the scene underneath.
    // Kept for the whole game and moved from scene to scene, so it must never be left on a scene when that scene is disposed.
    private readonly ScreenSurface transitionOverlay;

    private static ScreenSurface CreateTransitionOverlay()
    {
        ScreenSurface overlay = new(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT)
        {
            FocusOnMouseClick = false,
        };

        // transitions draw on this, anything they don't touch stays see through
        overlay.Surface.DefaultBackground = Color.Transparent;
        overlay.Surface.Clear();

        return overlay;
    }

    private void UpdateTransition(TimeSpan delta)
    {
        if (transition == Transition.NONE) return;

        transitionTimer += delta;

        TimeSpan duration = activeTransition.Duration;
        float progress = duration > TimeSpan.Zero
            ? Math.Min((float)(transitionTimer / duration), 1f)
            : 1f;

        bool covering = transition == Transition.FADE_OUT;
        activeTransition.Draw(transitionOverlay, covering ? progress : 1f - progress, covering);

        if (progress < 1f) return;

        if (covering)
            SwapScene();
        else
            FinishTransition();
    }

    private void StartFade(Transition fade)
    {
        transition = fade;
        transitionTimer = TimeSpan.Zero;
    }

    private void CoverScene(Scene scene)
    {
        transitionOverlay.Parent = scene; // added last, so it draws over everything already there
        transitionOverlay.IsFocused = true;
        transitionOverlay.IsExclusiveMouse = true;
    }

    private void FinishTransition()
    {
        transitionOverlay.IsExclusiveMouse = false;
        transitionOverlay.Parent = null;

        // the next transition might be a different one, don't leave this one's drawing for it
        transitionOverlay.Tint = Color.Transparent;
        transitionOverlay.Surface.Clear();

        currentScene.IsPaused = false;
        currentScene.IsFocused = true;

        activeTransition = null;
        transition = Transition.NONE;
    }
}
