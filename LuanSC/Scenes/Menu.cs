using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LuanSC.Scenes
{
    public class Menu : Scene
    {
        private ScreenSurface surface;
        public Menu(SceneManager manager) : base(manager)
        {
            surface = new(40, 30);
            surface.Position = new(GameSettings.GAME_WIDTH / 2 - surface.Width / 2, GameSettings.GAME_HEIGHT / 2 - surface.Height / 2);

            Border.CreateForSurface(surface, "The Void");

            ControlsConsole controls = new(surface.Width - 2, surface.Height - 2)
            {
                Position = new(1, 1)
            };

            Button b = new(20, 1)
            {
                Text = "Start Game",
                Position = new(controls.Width / 2 - 10, 5)
            };

            Button b2 = new(20, 1)
            {
                Text = "Test Scene",
                Position = new(controls.Width / 2 - 10, 7)
            };
            b2.Click += (s, e) =>
            {
                GameRoot.sceneManger.ChangeScene(manager => new TestScene(manager));
            };

            Button b3 = new(20, 1)
            {
                Text = "Visual Novel Scene",
                Position = new(controls.Width / 2 - 10, 9)
            };
            b3.Click += (s, e) =>
            {
                GameRoot.sceneManger.ChangeScene(manager => new VisualNovelScene(manager));
            };

            Button b4 = new(20, 1)
            {
                Text = "Combat Scene",
                Position = new(controls.Width / 2 - 10, 11)
            };
            b4.Click += (s, e) =>
            {
                GameRoot.sceneManger.ChangeScene(manager => new CombatScene(manager, null));
            };

            controls.Controls.Add(b);
            controls.Controls.Add(b2);
            controls.Controls.Add(b3);
            controls.Controls.Add(b4);

            surface.Children.Add(controls);

            Children.Add(surface); 
        }
    }
}
