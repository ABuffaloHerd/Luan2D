using System;

namespace LuanSC.Scenes.Transitions;

/// <summary>
/// How one scene changes into the next. Only decides what the overlay looks like, SceneManager runs the rest.
/// </summary>
/// <remarks>
/// USING ONE
/// Pass it as the second argument to ChangeScene. Leave it out to get the default FadeTransition.
///     manager.ChangeScene(sceneManager => new Menu(sceneManager), new WipeTransition());
///
/// MAKING ONE
/// Add a class in this folder that implements this interface. Nothing needs registering. FadeTransition is the simple
/// example using Tint, WipeTransition draws individual cells. A minimal one that fades through white:
///
///     public sealed class WhiteFlashTransition : ISceneTransition
///     {
///         public TimeSpan Duration => TimeSpan.FromMilliseconds(150);
///
///         public void Draw(ScreenSurface overlay, float amount, bool coveringScreen)
///         {
///             overlay.Tint = new Color(255, 255, 255, (int)(amount * 255));
///         }
///     }
///
/// HOW A TRANSITION RUNS
///  1. ChangeScene puts the overlay on top of the current scene. From here until step 5 it takes all keyboard and mouse input.
///  2. Draw is called every frame with amount going from 0 to 1 and coveringScreen true, over Duration.
///  3. At 1 the old scene is swapped for the new one. Draw is called once more with amount 1 and coveringScreen false.
///  4. Draw is called every frame with amount going from 1 to 0 and coveringScreen false, over Duration again.
///  5. The overlay is taken off the scene and wiped, so the next transition starts clean. No cleanup needed here.
/// The first scene at startup skips the first half, there's nothing to cover.
///
/// RULES FOR DRAW
///  - amount 1 has to hide the whole screen, whatever coveringScreen says. That's when the scenes swap, and any gap shows it.
///  - amount 0 has to show the whole scene.
///  - Set every cell you use on every call. Don't rely on what the last frame left there.
///  - Use solid colours on cells, and Tint for anything partly see through. Partly see through cells can look much dimmer than their alpha,
///    see CombatScene.OverlayColor.
///  - Don't dispose the overlay or change its Parent, IsFocused or IsExclusiveMouse. SceneManager owns those.
///  - Don't call ChangeScene from here. Requests during a transition are ignored.
///
/// PACING
/// amount moves in a straight line. For anything else reshape it at the top of Draw. This one starts and ends slowly:
///     amount = amount * amount * (3f - 2f * amount);
/// 0 stays 0 and 1 stays 1, which any reshaping has to keep or the rules above break.
///
/// SCENES DURING A TRANSITION
/// Scene.IsPaused is true throughout, on both scenes. They keep updating and drawing behind the overlay and input is already blocked,
/// so a scene only has to check IsPaused for things that happen by themselves, like CombatScene does for AI turns.
///
/// STATE
/// Fields are fine, only one transition runs at a time. Make a new instance for each ChangeScene if it needs to start fresh.
/// </remarks>
public interface ISceneTransition
{
    /// <summary>
    /// How long each half takes, so a full scene change is twice this. Zero or less skips straight to the end of each half.
    /// </summary>
    TimeSpan Duration { get; }

    /// <summary>
    /// Draws one frame of the transition. See the rules above.
    /// </summary>
    /// <param name="overlay">Full screen surface over the scene, GAME_WIDTH by GAME_HEIGHT. Starts with clear cells and no tint.</param>
    /// <param name="amount">How much of the screen to hide. 0 shows everything, 1 hides everything.</param>
    /// <param name="coveringScreen">True while hiding the old scene, false while revealing the new one. Lets a wipe carry on in the same direction.</param>
    void Draw(ScreenSurface overlay, float amount, bool coveringScreen);
}
