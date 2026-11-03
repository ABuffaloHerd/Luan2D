using LuanSC.Scenes.Transitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes
{
    public partial class SceneManager
    {
        private Scene currentScene { get; set; }
        private Func<SceneManager, Scene> pendingScene;

        public SceneManager()
        {
            transitionOverlay = CreateTransitionOverlay();

            // runs after input and Update, so no scene code is on the stack when the switch happens
            Game.Instance.FrameUpdate += (sender, host) => UpdateTransition(host.UpdateFrameDelta);
        }

        /// <summary>
        /// Hides the current scene, switches to the new one and reveals it.
        /// Safe to call from the old scene's own input handlers. Requests during a transition are ignored.
        /// </summary>
        /// <param name="sceneTransition">How it looks. Null is the default FadeTransition.</param>
        public void ChangeScene(Func<SceneManager, Scene> factory, ISceneTransition sceneTransition = null)
        {
            if (factory == null) return;
            if (transition != Transition.NONE) return;

            pendingScene = factory;
            activeTransition = sceneTransition ?? new FadeTransition();

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
            activeTransition.Draw(transitionOverlay, 1f, false);
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
    }
}
