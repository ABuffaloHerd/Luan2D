using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes
{
    public class SceneManager
    {
        private enum Transition
        {
            NONE,
            FADE_OUT,
            FADE_IN
        }

        /// <summary>
        /// How long each half of a transition takes, so a full scene change is twice this.
        /// </summary>
        private static readonly TimeSpan FadeTime = TimeSpan.FromMilliseconds(250);

        private Scene currentScene { get; set; }
        private Func<SceneManager, Scene> pendingScene;

        private Transition transition = Transition.NONE;
        private TimeSpan transitionTimer;

        // Sits on top of the scene during a transition. Takes the keyboard and mouse too, so nothing reaches the scene underneath.
        // Kept for the whole game and moved from scene to scene, so it must never be left on a scene when that scene is disposed.
        private readonly ScreenSurface fadeOverlay;

        public SceneManager()
        {
            fadeOverlay = new(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT)
            {
                FocusOnMouseClick = false,
            };

            // the tint does the darkening, the cells themselves stay see through
            fadeOverlay.Surface.DefaultBackground = Color.Transparent;
            fadeOverlay.Surface.Clear();

            // runs after input and Update, so no scene code is on the stack when the switch happens
            Game.Instance.FrameUpdate += (sender, host) => UpdateTransition(host.UpdateFrameDelta);
        }

        /// <summary>
        /// Fades the current scene out, switches to the new one at full black and fades it in.
        /// Safe to call from the old scene's own input handlers. Requests during a transition are ignored.
        /// </summary>
        public void ChangeScene(Func<SceneManager, Scene> factory)
        {
            if (factory == null) return;
            if (transition != Transition.NONE) return;

            pendingScene = factory;

            // the first scene at startup, there's nothing on screen to fade out
            if (currentScene is null)
            {
                SwapScene();
                return;
            }

            currentScene.IsPaused = true;
            CoverScene(currentScene);
            StartFade(Transition.FADE_OUT);
        }

        private void UpdateTransition(TimeSpan delta)
        {
            if (transition == Transition.NONE) return;

            transitionTimer += delta;
            float progress = Math.Min((float)(transitionTimer / FadeTime), 1f);

            SetDarkness(transition == Transition.FADE_OUT ? progress : 1f - progress);

            if (progress < 1f) return;

            if (transition == Transition.FADE_OUT)
                SwapScene();
            else
                FinishTransition();
        }

        private void StartFade(Transition fade)
        {
            transition = fade;
            transitionTimer = TimeSpan.Zero;
        }

        /// <param name="darkness">0 shows the scene, 1 is full black.</param>
        private void SetDarkness(float darkness)
        {
            fadeOverlay.Tint = new Color(0, 0, 0, (int)(darkness * 255));
        }

        private void CoverScene(Scene scene)
        {
            fadeOverlay.Parent = scene; // added last, so it draws over everything already there
            fadeOverlay.IsFocused = true;
            fadeOverlay.IsExclusiveMouse = true;
        }

        private void SwapScene()
        {
            // cleared first so a factory that throws doesn't run again
            Func<SceneManager, Scene> factory = pendingScene;
            pendingScene = null;

            Scene previous = currentScene;

            currentScene = factory(this);
            currentScene.IsPaused = true;
            Game.Instance.Screen = currentScene;

            // also takes the overlay off the old scene before it gets disposed
            CoverScene(currentScene);
            SetDarkness(1f);
            StartFade(Transition.FADE_IN);

            // this runs after Screen.Update, so the new scene would be drawn before it ever updated.
            // Controls paint themselves in Update and drawing clears their dirty flag, so they'd stay blank until something dirtied them again.
            currentScene.Update(Game.Instance.UpdateFrameDelta);

            // When the current scene is unloaded i am assuming that it goes out of scope enough for the gc to kill it
            // otherwise i will be very, very pissed off.
            // Anything it subscribed to would keep it alive, so let it clean up. Only after the new scene built successfully,
            // which also means the old one hears any combat events the new one publishes while it builds. Its feed is about to go anyway.
            previous?.Dispose();
        }

        private void FinishTransition()
        {
            fadeOverlay.IsExclusiveMouse = false;
            fadeOverlay.Parent = null;

            currentScene.IsPaused = false;
            currentScene.IsFocused = true;

            transition = Transition.NONE;
        }
    }
}
