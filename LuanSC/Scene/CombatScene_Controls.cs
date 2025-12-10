using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scene
{
    public partial class CombatScene
    {
        public override bool ProcessKeyboard(Keyboard keyboard)
        {
            if (keyboard.IsKeyPressed(SadConsole.Input.Keys.Escape))
            {
                manager.ChangeScene(sceneManager => new Menu(sceneManager));
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.Up))
            {
                testObject.Position += new Point(0, -1);
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.Down))
            {
                testObject.Position += new Point(0, 1);
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.Left))
            {
                testObject.Position += new Point(-1, 0);
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.Right))
            {
                testObject.Position += new Point(1, 0);
                return true;
            }

            return base.ProcessKeyboard(keyboard);
        }
    }
}
