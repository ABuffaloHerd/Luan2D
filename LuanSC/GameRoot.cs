using LuanSC.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC
{
    /// <summary>
    /// All the logic here
    /// </summary>
    internal static class GameRoot
    {
        public static SceneManager sceneManger;
        public static void Initialize(object sender, GameHost args)
        {
            // assign scene manager
            sceneManger = new();
            sceneManger.ChangeScene(sceneManger => new Menu(sceneManger));
        }
    }
}
