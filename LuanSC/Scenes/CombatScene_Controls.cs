using LuanSC.Data.Components;
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
                currentControlledGameObject.Position += new Point(0, -1);
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.Down))
            {
                currentControlledGameObject.Position += new Point(0, 1);
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.Left))
            {
                currentControlledGameObject.Position += new Point(-1, 0);
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.Right))
            {
                currentControlledGameObject.Position += new Point(1, 0);
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
                currentControlledGameObject.Direction = Data.Direction.UP;
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.S))
            {
                currentControlledGameObject.Direction = Data.Direction.DOWN;
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.A))
            {
                currentControlledGameObject.Direction = Data.Direction.LEFT;
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.D))
            {
                currentControlledGameObject.Direction = Data.Direction.RIGHT;
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.E))
            {
                fightFeed.AddLine($"Testint {testint++}");
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.Space))
            {
                currentControlledGameObject.Blink();
                currentControlledGameObject.GetComponent<HPComponent>().Current += 10;

                return true;
            }

            return base.ProcessKeyboard(keyboard);
        }
    }
}
