using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Test
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        CanvasTextFormat textFormat;
        CanvasSolidColorBrush fgBrush, bgBrush;

        DispatcherQueueTimer timer;

        public MainWindow()
        {
            InitializeComponent();

            textFormat = new CanvasTextFormat()
            {
                FontSize = 24,
                FontFamily = "Cascadia Mono",
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Center
            };

            //fgBrush = new CanvasSolidColorBrush(null, Colors.Green);
            //bgBrush = new CanvasSolidColorBrush(null, Colors.Black);
        }
        private void CanvasControl_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender,
                             Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            args.DrawingSession.DrawEllipse(155, 115, 80, 30, Colors.Black, 3);
            args.DrawingSession.DrawText("Hello, world!", 100, 100, Colors.Yellow, textFormat);
        }
    }

}
