using SadConsole.Input;
using SadConsole.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scene
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

            controls = new(10, 10);
            controls.Position = new(surface.Width / 2, surface.Height / 2);

            surface.Children.Add(controls);
            Children.Add(surface);

            Border.BorderParameters borderParams = Border.BorderParameters.GetDefault();
             
            Border b = new(controls, borderParams);

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
