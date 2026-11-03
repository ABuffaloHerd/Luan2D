using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes
{
    public class SceneManager
    {
        private Scene currentScene { get; set; }
        public void ChangeScene(Func<SceneManager, Scene> factory)
        {
            if (factory == null) return;

            // When the current scene is unloaded i am assuming that it goes out of scope enough for the gc to kill it
            // otherwise i will be very, very pissed off.
            // Anything it subscribed to would keep it alive, so let it unsubscribe first.
            (currentScene as IDisposable)?.Dispose();

            currentScene = factory(this);
            Game.Instance.Screen = currentScene;
            currentScene.IsFocused = true;
        }
    }
}
