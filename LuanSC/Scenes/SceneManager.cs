using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes
{
    public class SceneManager
    {
        private Scene currentScene { get; set; }
        private Func<SceneManager, Scene> pendingScene;

        public SceneManager()
        {
            // runs after input and Update, so no scene code is on the stack when the switch happens
            Game.Instance.FrameUpdate += (sender, host) => ApplyPendingScene();
        }

        /// <summary>
        /// Queues a scene switch for the end of this frame, so it's safe to call from the old scene's own input handlers.
        /// The old scene keeps running until then. If this is called twice in a frame the last one wins.
        /// </summary>
        public void ChangeScene(Func<SceneManager, Scene> factory)
        {
            if (factory == null) return;

            pendingScene = factory;

            // the first scene at startup, there's nothing running to wait for
            if (currentScene is null)
                ApplyPendingScene();
        }

        private void ApplyPendingScene()
        {
            if (pendingScene is null) return;

            // cleared first so a factory that throws doesn't run again every frame
            Func<SceneManager, Scene> factory = pendingScene;
            pendingScene = null;

            Scene previous = currentScene;

            currentScene = factory(this);
            Game.Instance.Screen = currentScene;
            currentScene.IsFocused = true;

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
