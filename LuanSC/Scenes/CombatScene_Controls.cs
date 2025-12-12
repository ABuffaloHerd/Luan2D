using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes
{
    public partial class CombatScene
    {
        private static int testint = 0;
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

            if (keyboard.IsKeyPressed(Keys.H))
            {
                overlay.IsVisible = !overlay.IsVisible;
                return true;
            }

            // Directional Changes
            if (keyboard.IsKeyPressed(Keys.W))
            {
                testObject.Direction = Data.Direction.UP;
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.S))
            {
                testObject.Direction = Data.Direction.DOWN;
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.A))
            {
                testObject.Direction = Data.Direction.LEFT;
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.D))
            {
                testObject.Direction = Data.Direction.RIGHT;
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.E))
            {
                fightFeed.AddLine($"Testint {testint++}");
            }

            return base.ProcessKeyboard(keyboard);
        }
    }
}
