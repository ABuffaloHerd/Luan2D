using LuanSC.Data;
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
        private ControlsConsole hud;

        private CombatSettings settings;
        public CombatScene(SceneManager manager, CombatSettings? settings) : base(manager)
        {
            // Set up surfaces and borders
            this.settings = settings;

            /// Test text
            surface = new(43, 43);
            surface.Position = new(1, 1);
            //surface.Fill(Color.White, Color.Black, 2);
            surface.Print(0, 0, "Combat Scene");
            surface.Print(0, 1, "Press ESC to return to menu");
            Children.Add(surface);

            /// Overlay surface
            overlay = new(43, 43);
            overlay.Position = new(1, 1);
            //overlay.Fill(Color.Yellow, Color.Transparent, 'X');

            Children.Add(overlay);

            Border.BorderParameters parameters = Border.BorderParameters.GetDefault();
            parameters.AddTitle("Arena");
            new Border(surface, parameters);

            controls = new(21, 20);
            controls.Position = new(45, GameSettings.GAME_HEIGHT / 2);

            parameters = Border.BorderParameters.GetDefault();
            parameters.AddTitle("Controls");
            new Border(controls, parameters);

            Children.Add(controls);

            // make sure that only the scene itself is focused
            foreach (ScreenObject child in Children)
            {
                child.UseKeyboard = false;
                child.IsFocused = false;
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
        }

        private void UpdateOverlay()
        {
            overlay.Clear();

            // This is only used for pattern rendering for now
            // Get current pattern from active controllable object.

            // TODO: Check that it is controllable object's turn

            var pattern = testObject.Weapon.Range;
            foreach (var cell in pattern.GetRotated(testObject.Direction))
            {
                int drawX = testObject.Position.X + cell.X;
                int drawY = testObject.Position.Y + cell.Y;
                if (drawX >= 0 && drawX < overlay.Width && drawY >= 0 && drawY < overlay.Height)
                {
                    overlay.SetCellAppearance(drawX, drawY, new ColoredGlyph(Color.Yellow, Color.Transparent, 'X'));
                }
            }
        }
    }
}
