using LuanSC.Data;
using LuanSC.Data.Components;
using SadConsole.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes
{
    public partial class CombatScene : Scene
    {
        // Surface is where the gameobjects are drawn
        private ScreenSurface surface;

        // overlay is where effects and the cursor is drawn
        private ScreenSurface overlay;

        private ControlsConsole controls;
        private HUD hud;

        private CombatSettings settings;
        private FightFeed fightFeed;
        public CombatScene(SceneManager manager, CombatSettings? settings) : base(manager)
        {
            // Set up surfaces and borders
            this.settings = settings;


            /// Main surface for game objects. Has the entity manager attached to it.
            surface = new(43, 43);
            surface.Position = new(1, 1);

            /// Test text
            surface.Print(0, 0, "Combat Scene");
            surface.Print(0, 1, "Press ESC to return to menu");

            Border.BorderParameters parameters = Border.BorderParameters.GetDefault();
            ShapeParameters shapeParams = ShapeParameters.CreateStyledBoxThick(Color.White);
            parameters.ChangeBorderStyle(shapeParams);
            parameters.AddTitle("Arena");
            parameters.TitleBackground = Color.White;
            parameters.TitleForeground = Color.DarkRed;
            parameters.ChangeBorderForegroundColor(Color.Gray);
            new Border(surface, parameters);

            Children.Add(surface);
            /// Overlay surface for effects and cursor and range patterns
            overlay = new(43, 43);
            overlay.Position = new(1, 1);
            //overlay.Fill(Color.Yellow, Color.Transparent, 'X');

            Children.Add(overlay);


            // CONTROLS
            controls = new(21, 20);
            controls.Position = new(46, GameSettings.GAME_HEIGHT / 2 + 2);

            parameters = Border.BorderParameters.GetDefault();
            parameters.AddTitle("Controls");
            new Border(controls, parameters);

            Children.Add(controls);

            // HUD
            hud = new(21, 20);
            hud.Position = new(46, 1);
            parameters = Border.BorderParameters.GetDefault();
            parameters.AddTitle("HUD");
            new Border(hud, parameters);

            Children.Add(hud);

            // FIGHT FEED
            fightFeed = new FightFeed(20, 20);
            fightFeed.Position = new(controls.Position.X + controls.Width + 2, 1); // y is 1 to accomodate border

            Children.Add(fightFeed);

            // make sure that only the scene itself is focused
            foreach (ScreenSurface child in Children)
            {
                child.UseKeyboard = false;
                child.IsFocused = false;
                child.FocusOnMouseClick = false;
            }

            // REALLY make sure this scene is focused
            this.IsFocused = true;

            // Set up game objects and entities
            Init();

        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);

            // Refresh the overlay if it's visible and is dirty
            if (overlay.IsVisible)
            {
                UpdateOverlay();
            }


            hud.UpdateHP(currentControlledGameObject.GetComponent<HPComponent>());
        }

        private void UpdateOverlay()
        {
            overlay.Clear();

            // This is only used for pattern rendering for now
            // Get current pattern from active controllable object.

            // TODO: Check that it is controllable object's turn

            var pattern = currentControlledGameObject.Weapon.Range;
            foreach (var cell in pattern.GetRotated(currentControlledGameObject.Direction))
            {
                int drawX = currentControlledGameObject.Position.X + cell.X;
                int drawY = currentControlledGameObject.Position.Y + cell.Y;
                if (drawX >= 0 && drawX < overlay.Width && drawY >= 0 && drawY < overlay.Height)
                {
                    overlay.SetCellAppearance(drawX, drawY, new ColoredGlyph(Color.Yellow, Color.Transparent, 'X'));
                }
            }
        }
    }
}
