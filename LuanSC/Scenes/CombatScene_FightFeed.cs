using LuanSC.Data;
using SadConsole.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Scenes
{
    public partial class CombatScene
    {
        /// <summary>
        /// Scrolls random tips right to left. Each tip scrolls all the way off before the next one starts.
        /// </summary>
        private class TickerText : ScreenSurface
        {
            public TimeSpan Cooldown = TimeSpan.FromMilliseconds(100);

            // every tip from every file, one per line
            private readonly List<string> tips = new();

            // the tip on screen, with a screen's worth of spaces in front so it scrolls in from the right edge
            private string current;
            private int offset = 0;
            private TimeSpan sinceLastScroll = TimeSpan.Zero;

            /// <param name="files">File names under Resources/Asset. Missing ones are skipped.</param>
            public TickerText(int width, int height, params string[] files) : base(width, height)
            {
                Surface.DefaultForeground = Color.White;
                Surface.DefaultBackground = Color.Transparent;

                foreach (string file in files)
                {
                    // the post build step copies Resources next to the exe
                    string path = Path.Combine(AppContext.BaseDirectory, "Resources", "Asset", file);
                    if (!File.Exists(path)) continue;

                    tips.AddRange(File.ReadAllLines(path).Where(line => !string.IsNullOrWhiteSpace(line)));
                }

                NextTip();
            }

            private void NextTip()
            {
                offset = 0;
                current = tips.Count > 0
                    ? new string(' ', Surface.Width) + tips[Random.Shared.Next(tips.Count)]
                    : null; // no tips, nothing to scroll
            }

            public override void Update(TimeSpan delta)
            {
                base.Update(delta);
                if (current is null) return;

                sinceLastScroll += delta;
                if (sinceLastScroll < Cooldown) return;
                else sinceLastScroll = TimeSpan.Zero;

                Surface.Clear();
                Surface.Print(0, 0, current.Substring(offset, Math.Min(Surface.Width, current.Length - offset)));
                offset++;

                // the last character just went off the left edge
                if (offset >= current.Length)
                    NextTip();
            }
        }

        private class FightFeed : Console
        {
            /// <summary>
            /// How many wrapped rows to remember. Only Height of them are on screen.
            /// </summary>
            public const int MaxRows = 200;

            // already wrapped to Width, oldest first. Color null is the default foreground.
            private List<(string Text, Color? Color)> rows = new();

            // how many of the rows at the end of the list belong to the newest message
            private int latestRowCount;

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

            /// <summary>
            /// Adds a message. Don't put colour tags in line, WordWrap counts them as text.
            /// Pass color instead and it gets applied after wrapping.
            /// </summary>
            public void AddLine(string line, Color? color = null)
            {
                // wrap here so Print never runs off the end of a row and into the next one
                string[] wrapped = line.WordWrap(Width).ToArray();
                latestRowCount = wrapped.Length;
                rows.AddRange(wrapped.Select(text => (text, color)));

                if (rows.Count > MaxRows)
                    rows.RemoveRange(0, rows.Count - MaxRows);

                Redraw();
            }

            public void AddDivider()
            {
                AddLine(new string('=', Width), Color.DarkGray);
            }

            /// <summary>
            /// Newest row at the bottom, older ones above it.
            /// </summary>
            private void Redraw()
            {
                this.Clear();

                int visible = Math.Min(Height, rows.Count);
                StringBuilder sb = new();

                for (int i = 0; i < visible; i++)
                {
                    sb.Clear();
                    (string text, Color? color) = rows[rows.Count - 1 - i];

                    // the newest message is yellow, even when it wraps onto several rows
                    if (i < latestRowCount)
                        color = Color.Yellow;

                    if (color is not null)
                        sb.Append($"[c:r f:{color.Value.ToParser()}]");

                    sb.Append(text);
                    this.Print(0, Height - 1 - i, ColoredString.Parser.Parse(sb.ToString()));
                }
            }
        }
    }
}
