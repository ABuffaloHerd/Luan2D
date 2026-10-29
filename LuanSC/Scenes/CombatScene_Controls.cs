using LuanSC.Data;
using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using SadConsole.Input;
using SadConsole.UI;
using System;
using System.Collections.Generic;
using System.Text;
using Direction = LuanSC.Data.Direction;

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

            // Always allow escape to work
            if (currentControlledGameObject?.GetComponent<AIComponent>() is not null)
                return base.ProcessKeyboard(keyboard);

            if (keyboard.IsKeyPressed(Keys.Up))
            {
                //Point newPos = Direction.UP.ToVector();
                //if (IsWalkable(currentControlledGameObject.Position + newPos))
                //    currentControlledGameObject.Position += newPos;
                //return true;
                Execute(currentControlledGameObject, new TurnAction.Move(Direction.UP));
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.Down))
            {
                Execute(currentControlledGameObject, new TurnAction.Move(Direction.DOWN));
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.Left))
            {
                Execute(currentControlledGameObject, new TurnAction.Move(Direction.LEFT));
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.Right))
            {
                Execute(currentControlledGameObject, new TurnAction.Move(Direction.RIGHT));
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
                Execute(currentControlledGameObject, new TurnAction.Face(Direction.UP));
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.S))
            {
                Execute(currentControlledGameObject, new TurnAction.Face(Direction.DOWN));
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.A))
            {
                Execute(currentControlledGameObject, new TurnAction.Face(Direction.LEFT));
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.D))
            {
                Execute(currentControlledGameObject, new TurnAction.Face(Direction.RIGHT));
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.E))
            {
                fightFeed.AddLine($"Testint {testint++}");
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.Space))
            {
                Execute(currentControlledGameObject, new TurnAction.EndTurn());
                return true;
            }

            // Attack key L
            if (keyboard.IsKeyPressed(Keys.L))
            {
                Execute(currentControlledGameObject, new TurnAction.Attack());
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
