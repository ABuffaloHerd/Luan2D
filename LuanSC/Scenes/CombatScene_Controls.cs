using LuanSC.Data.Components;
using SadConsole.Input;
using Direction = LuanSC.Data.Direction;
using System;
using System.Collections.Generic;
using System.Text;
using LuanSC.Data;

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
                Point newPos = Direction.UP.ToVector();
                if (BoundsCheck(currentControlledGameObject.Position + newPos))
                    currentControlledGameObject.Position += newPos;
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.Down))
            {
                Point newPos = Direction.DOWN.ToVector();
                if (BoundsCheck(currentControlledGameObject.Position + newPos))
                    currentControlledGameObject.Position += newPos;
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.Left))
            {
                Point newPos = Direction.LEFT.ToVector();
                if (BoundsCheck(currentControlledGameObject.Position + newPos))
                    currentControlledGameObject.Position += newPos;
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.Right))
            {
                Point newPos = Direction.RIGHT.ToVector();
                if (BoundsCheck(currentControlledGameObject.Position + newPos))
                    currentControlledGameObject.Position += newPos;
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.H))
            {
                // toggle overlay
                overlayVisible = !overlayVisible;

                if(!overlayVisible)
                {
                    overlay.Clear();
                }
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
                StartNextTurn();
                return true;
            }

            // Attack key L
            if (keyboard.IsKeyPressed(Keys.L))
            {
                // Run attack function
                Attack(currentControlledGameObject);
            }

            return base.ProcessKeyboard(keyboard);
        }
    }
}
