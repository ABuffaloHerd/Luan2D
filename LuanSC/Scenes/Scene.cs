using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using LuanSC.Data;

namespace LuanSC.Scenes
{
    public abstract class Scene : ScreenObject, IDisposable
    {
        private bool disposed;

        protected SceneManager manager;
        protected SoundManager soundManager;
        public Scene(SceneManager manager)
        {
            this.manager = manager;
            soundManager = new SoundManager();
        }

        /// <summary>
        /// Lets go of everything the scene holds. SceneManager calls this once the scene is off screen.
        /// Don't call it from the scene's own input handlers or Update, the surfaces can't be used afterwards.
        /// </summary>
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            OnDispose();
            DisposeTree(this);
        }

        /// <summary>
        /// Scene specific cleanup, like event subscriptions. Runs before the surfaces are disposed.
        /// </summary>
        protected virtual void OnDispose() { }

        /// <summary>
        /// Disposes an object and all its children. Must already be out of the scene, nothing can draw or update it afterwards.
        /// </summary>
        protected static void DisposeTree(IScreenObject obj)
        {
            // the mouse remembers the last object it was over and would read its disposed surface next frame
            GameHost.Instance.Mouse.ClearLastMouseScreenObject();

            DisposeBranch(obj);
        }

        private static void DisposeBranch(IScreenObject obj)
        {
            foreach (IScreenObject child in obj.Children.ToList())
                DisposeBranch(child);

            (obj as IDisposable)?.Dispose();
        }
    }
}
