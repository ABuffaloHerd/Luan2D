using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes
{
    public class TestScene : Scene
    {
        private ScreenSurface surface;
        private ControlsConsole controls;
        private int fc = 0;
        public TestScene(SceneManager manager) : base(manager)
        {

            surface = new(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
            //surface.FillWithRandomGarbage(surface.Font);
            surface.Print(0, 0, "Press ESC to return to menu");

            surface.UseKeyboard = false;
            surface.IsFocused = false;

            controls = new(20, 10);
            controls.Position = new(surface.Width / 2, surface.Height / 2);
            controls.IsFocused = false;
            controls.UseKeyboard = false;
            controls.FocusOnMouseClick = false;


            Border.BorderParameters borderParams = Border.BorderParameters.GetDefault();
            borderParams.AddTitle("Chara");
            borderParams.TitleAlignment = HorizontalAlignment.Center;

            Border b = new(controls, borderParams);

            ProgressBar pb = new(10, 1, HorizontalAlignment.Left);
            pb.Position = new(0, 0);
            pb.BarColor = SadRogue.Primitives.Color.Red;
            pb.Progress = 0.5f;

            controls.Controls.Add(pb);

            surface.Children.Add(controls);
            Children.Add(surface);
        }

        public override bool ProcessKeyboard(Keyboard keyboard)
        {
            if (keyboard.IsKeyPressed(Keys.Escape))
            {
                manager.ChangeScene(sceneManager => new Menu(sceneManager));
                return true;
            }

            if (keyboard.IsKeyPressed(Keys.A))
            {
                surface.Print(0, 5, "You pressed the A key!");
                return true;
            }

            surface.IsDirty = true;
            return base.ProcessKeyboard(keyboard);
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);

            surface.Clear();
            surface.Print(0, 1, $"Time since last frame: {delta.TotalMilliseconds} ms");
            surface.Print(0, 0, "Press ESC to return to menu");
            surface.Print(0, 3, $"Frame count: {fc++}");
        }
    }
}
