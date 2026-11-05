using LuanSC.Scenes.Transitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes
{
    public partial class SceneManager
    {
        // what happens to the scene being left once the next one is on screen
        private enum SceneExit
        {
            DISPOSE,      // PopScene: it's finished, the one below takes over
            SUSPEND,      // PushScene: kept on the stack to come back to
            DISPOSE_ALL   // ChangeScene: it and everything on the stack, there's nothing to go back to
        }

        private Scene currentScene { get; set; }
        private Func<SceneManager, Scene> pendingScene;
        private SceneExit pendingExit;

        // scenes to go back to, most recent on top. Not on screen, so they don't update or draw until they're popped
        private readonly Stack<Scene> suspended = new();

        public SceneManager()
        {
            transitionOverlay = CreateTransitionOverlay();

            // runs after input and Update, so no scene code is on the stack when the switch happens
            Game.Instance.FrameUpdate += (sender, host) => UpdateTransition(host.UpdateFrameDelta);
        }

        /// <summary>
        /// Hides the current scene, switches to the new one and reveals it. The old scene and the stack are disposed, so PopScene has nothing to go back to.
        /// Safe to call from the old scene's own input handlers. Requests during a transition are ignored.
        /// </summary>
        /// <param name="sceneTransition">How it looks. Null is the default FadeTransition.</param>
        public void ChangeScene(Func<SceneManager, Scene> factory, ISceneTransition sceneTransition = null)
            => StartSwitch(factory, SceneExit.DISPOSE_ALL, sceneTransition);

        /// <summary>
        /// Like ChangeScene, but keeps the current scene as it is so PopScene can come back to it.
        /// </summary>
        /// <param name="sceneTransition">How it looks. Null is the default FadeTransition.</param>
        public void PushScene(Func<SceneManager, Scene> factory, ISceneTransition sceneTransition = null)
            => StartSwitch(factory, SceneExit.SUSPEND, sceneTransition);

        /// <summary>
        /// Disposes the current scene and goes back to the one it was pushed from, in the state it was left.
        /// Does nothing if there's nothing to go back to.
        /// </summary>
        /// <param name="sceneTransition">How it looks. Null is the default FadeTransition.</param>
        public void PopScene(ISceneTransition sceneTransition = null)
        {
            if (suspended.Count == 0) return;

            StartSwitch(_ => suspended.Pop(), SceneExit.DISPOSE, sceneTransition);
        }

        private void StartSwitch(Func<SceneManager, Scene> factory, SceneExit exit, ISceneTransition sceneTransition)
        {
            if (factory == null) return;
            if (transition != Transition.NONE) return;

            pendingScene = factory;
            pendingExit = exit;
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
            LeaveScene(previous);
        }

        private void LeaveScene(Scene previous)
        {
            if (previous is null) return;

            switch (pendingExit)
            {
                case SceneExit.SUSPEND:
                    // it's off screen now, the mouse shouldn't keep treating it as the last object it was over
                    GameHost.Instance.Mouse.ClearLastMouseScreenObject();
                    suspended.Push(previous);
                    break;

                case SceneExit.DISPOSE:
                    previous.Dispose();
                    break;

                case SceneExit.DISPOSE_ALL:
                    previous.Dispose();
                    while (suspended.Count > 0)
                        suspended.Pop().Dispose();
                    break;
            }
        }
    }
}
