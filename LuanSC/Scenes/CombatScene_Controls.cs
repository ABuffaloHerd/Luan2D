using LuanSC.Data;
using LuanSC.Data.Components;
using LuanSC.Data.Components.AI;
using LuanSC.Data.Animations;
using LuanSC.Objects.Summons;
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
            // aiming a spell takes over the keyboard, Esc cancels instead of quitting
            if (IsTargeting)
                return ProcessTargetingKeyboard(keyboard);

            if (keyboard.IsKeyPressed(SadConsole.Input.Keys.Escape))
            {
                manager.ChangeScene(sceneManager => new Menu(sceneManager));
                return true;
            }

            // Always allow escape to work
            if (currentControlledGameObject?.GetComponent<AIComponent>() is not null)
                return base.ProcessKeyboard(keyboard);

            // everyone who could take a turn is dead
            if (currentControlledGameObject is null)
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

            // DEBUG: X plays a circle blast, Z a square blast, in the middle of the arena
            if (keyboard.IsKeyPressed(Keys.X))
            {
                surface.Children.Add(AnimationRegistry.CircleBlast(new Point(surface.Width / 2, surface.Height / 2)));
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.Z))
            {
                surface.Children.Add(AnimationRegistry.SquareBlast(new Point(surface.Width / 2, surface.Height / 2)));
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

            // DEBUG SUMMON KEYS. Replaced by the spellbook (G). Uncomment to summon without a spellbook.
            // These skip the spellbook's checks, so anyone can summon anything.
            //
            //// K: familiar in the facing cell
            //if (keyboard.IsKeyPressed(Keys.K))
            //{
            //    Point spot = currentControlledGameObject.Position + currentControlledGameObject.Direction.ToVector();
            //    Execute(currentControlledGameObject, new TurnAction.Summon(SummonRegistry.Familiar, spot, ActionType.SPELL));
            //    return true;
            //}
            //
            //// C: magic circle in the facing cell
            //if (keyboard.IsKeyPressed(Keys.C))
            //{
            //    Point spot = currentControlledGameObject.Position + currentControlledGameObject.Direction.ToVector();
            //    Execute(currentControlledGameObject, new TurnAction.Summon(SummonRegistry.MagicCircle, spot, ActionType.SPELL, [new ResourceCost(ResourceType.MP, 50)]));
            //    return true;
            //}
            //
            //// B: sentry gun in the facing cell
            //if (keyboard.IsKeyPressed(Keys.B))
            //{
            //    Point spot = currentControlledGameObject.Position + currentControlledGameObject.Direction.ToVector();
            //    Execute(currentControlledGameObject, new TurnAction.Summon(owner => SummonRegistry.SentryGun(owner, hitsRequired: 3, builtGlyph: 'Y'), spot, ActionType.SKILL, [new ResourceCost(ResourceType.METAL, 100)]));
            //    return true;
            //}

            // zoom in and out + and -
            // TODO: zoom is half baked and doesn't work very well. Fix it in its own branch.
            // The view always shrinks from the top left, so it can't follow the action. AnimatedEffect assumes
            // the view never scrolls and will need a ViewPosition offset once it does.
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
            if (keyboard.IsKeyPressed(Keys.G))
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
