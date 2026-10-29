using LuanSC.Data;
using SadConsole.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes
{
    public partial class CombatScene
    {
        private class TickerText : ScreenSurface
        {
            public TimeSpan Cooldown = TimeSpan.FromMilliseconds(100);

            private string displayText;
            public string DisplayText
            {
                get => displayText;
                set
                {
                    displayText = value;
                    array = value is null ? null : new LoopingArray<char>(value.ToCharArray());
                }
            }

            private int offset = 0;

            private LoopingArray<char> array;
            private TimeSpan sinceLastScroll = TimeSpan.Zero;

            public TickerText(int width, int height) : base(width, height)
            {
                Surface.DefaultForeground = Color.White;
                Surface.DefaultBackground = Color.Transparent;

                DisplayText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
            }

            public override void Update(TimeSpan delta)
            {
                base.Update(delta);
                if (DisplayText is null) return;

                sinceLastScroll += delta;
                if (sinceLastScroll < Cooldown) return;
                else sinceLastScroll = TimeSpan.Zero;

                Surface.Clear();
                if (DisplayText.Length < Surface.Width)
                    Surface.Print(0, 0, DisplayText);
                else
                {
                    var buffer = new char[Surface.Width];
                    for(int x = 0; x < Surface.Width; x++)
                    {
                        buffer[x] = array[offset + x];
                    }

                    Surface.Print(0, 0, new string(buffer));
                    offset++;
                }
            }
        }

        private class FightFeed : Console
        {
            private LinkedList<string> feedLines = new();

            /// <summary>
            /// Wraps a deque to hide all the complexity of managing the fight feed.
            /// Automatically adds a border with the title "Fight Feed".
            /// </summary>
            /// <param name="width"></param>
            /// <param name="height"></param>
            public FightFeed(int width, int height) : base(width, height)
            {
                Border.BorderParameters parameters = Border.BorderParameters.GetDefault();
                parameters.AddTitle("Fight Feed");
                new Border(this, parameters);

                // if it isn't constructed with 20, it just doesn't work and i don't know why
                //Resize(width * 2, height * 2, true);

                this.UseKeyboard = false;
                this.IsFocused = false;
                this.FocusOnMouseClick = false;

                //this.FontSize = new(10, 10);
            }

            public void AddLine(string line)
            {
                feedLines.AddFirst(line);
            }

            public override void Update(TimeSpan delta)
            {
                base.Update(delta);

                // Clear the console area
                this.Clear();

                // Render the feed lines from the bottom up
                int maxLines = Height;

                for(int i = 0; i < maxLines; i++)
                {
                    if (i >= feedLines.Count)
                        break;
                    this.Print(0, Height - i - 1, feedLines.ElementAt(i));
                }
            }
        }
    }
}
