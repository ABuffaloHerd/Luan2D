using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using LuanSC.Data;

namespace LuanSC.Scenes
{
    public abstract class Scene : ScreenObject
    {
        protected SceneManager manager;
        protected SoundManager soundManager;
        public Scene(SceneManager manager)
        {
            this.manager = manager;
            soundManager = new SoundManager();
        }
    }
}
