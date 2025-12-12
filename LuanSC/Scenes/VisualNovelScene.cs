using SadConsole.Instructions;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace LuanSC.Scenes
{
    public class VisualNovelScene : Scene
    {
        private ScreenSurface surface;
        private Console textConsole;
        private DrawString instructions;
        public Queue<string> sentences;
        public VisualNovelScene(SceneManager manager) : base(manager)
        {
            this.IsFocused = true;
            sentences = new();

            surface = new(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
            surface.Print(0, 0, "Visual Novel Scene");
            surface.Print(0, 1, "Press ESC to return to menu");
            surface.Print(0, 2, "Press SPACE to advance text.");

            string[] text =
            [
                "Hello, Mariah.",
            ];

            foreach(var line in text)
            {
                sentences.Enqueue(line);
            }

            textConsole = new(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT - 10);
            textConsole.Position = new(0, GameSettings.GAME_HEIGHT - textConsole.Height);
            textConsole.FocusOnMouseClick = false;
            textConsole.UseKeyboard = false;
            textConsole.Cursor.Position = new(0, 0);
            textConsole.Cursor.IsVisible = true;

            Children.Add(surface);
            surface.Children.Add(textConsole);
        }

        public override bool ProcessKeyboard(SadConsole.Input.Keyboard keyboard)
        {
            if (keyboard.IsKeyPressed(SadConsole.Input.Keys.Escape))
            {
                manager.ChangeScene(sceneManager => new Menu(sceneManager));
                return true;
            }

            if (keyboard.IsKeyPressed(SadConsole.Input.Keys.Space))
            {
                StartNextSentence();
                return true;
            }

            return base.ProcessKeyboard(keyboard);
        }

        private void StartNextSentence()
        {
            if (sentences.Count == 0) return;
            if (instructions is not null && !instructions.IsFinished) return;
            
            textConsole.Clear();
            instructions = new(ColoredString.Parser.Parse(sentences.Dequeue()), TimeSpan.FromSeconds(5));
            instructions.RemoveOnFinished = true;

            instructions.Cursor = textConsole.Cursor;

            textConsole.SadComponents.Add(instructions);
        }
    }
}
