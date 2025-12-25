using SadConsole.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes
{
    public partial class CombatScene
    {
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
                Resize(width * 2, height * 2, true);

                this.UseKeyboard = false;
                this.IsFocused = false;
                this.FocusOnMouseClick = false;

                this.FontSize = new(10, 10);
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
