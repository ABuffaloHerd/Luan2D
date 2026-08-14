using LuanSC.Data.Components;
using SadConsole.Input;
using Direction = LuanSC.Data.Direction;
using System;
using System.Collections.Generic;
using System.Text;
using LuanSC.Data;
using SadConsole.UI;

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

            // zoom in and out + and -
            if (keyboard.IsKeyPressed(Keys.OemPlus))
            {
                surface.ViewHeight = Math.Max(surface.ViewHeight -= 5, 10);
                surface.ViewWidth = Math.Max(surface.ViewWidth -= 5, 10);

                surface.FontSize = new Point(surface.FontSize.X - 5, surface.FontSize.Y - 5);

                return true;
            }

            if (keyboard.IsKeyPressed(Keys.OemMinus))
            {
                surface.ViewHeight = Math.Min(surface.ViewHeight += 5, 45);
                surface.ViewWidth = Math.Min(surface.ViewWidth += 5, 45);

                surface.FontSize = new Point(surface.FontSize.X + 5, surface.FontSize.Y + 5);
                return true;
            }

            // Open spellbook G
            if (keyboard.IsKeyDown(Keys.G))
            {
                DisplaySpellbook();
                return true;
            }

            // Hit F1 for help
            if (keyboard.IsKeyPressed(Keys.F1))
            {
                DisplayHelp();
                return true;
            }

            return base.ProcessKeyboard(keyboard);
        }
    }
}
