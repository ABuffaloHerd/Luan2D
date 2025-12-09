using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace LuanSC.Scene
{
    public abstract class Scene : ScreenObject
    {
        protected SceneManager manager;
        public Scene(SceneManager manager)
        {
            this.manager = manager;
        }
    }
}
