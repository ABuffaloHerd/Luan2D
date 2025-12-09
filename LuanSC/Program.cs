using LuanSC;
using LuanSC.Scene;
using SadConsole.Configuration;
using System.Runtime.CompilerServices;

Settings.WindowTitle = "Luan 2D";

// Fix: Use a lambda to match EventHandler<GameHost> signature
Builder startup = new Builder()
    .SetWindowSizeInCells(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT)
    .ConfigureFonts("Resources\\Font\\Bisasam.font")
    .OnStart((sender, args) => GameRoot.Initialize(sender, args))
    .EnableImGuiDebugger(SadConsole.Input.Keys.F12);

Game.Create(startup);
Game.Instance.Run();