using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Svg;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using Win2dTest.StarterPack;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.WebUI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Win2dTest
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private Screen _screen;

        public MainWindow()
        {
            InitializeComponent();
            _screen = new Screen
            {
                Viewport = new Viewport(800, 600)
            };

            _screen.Items.Add(new MyRectangle());
        }

        private void Render(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            try
            {
                FpsHelper.DrawFps(args);
                _screen.Render(args.DrawingSession);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Right)
            {
                _screen.Viewport.X++;
            }

            if (e.Key == VirtualKey.Left)
            {
                _screen.Viewport.X--;
            }

            if (e.Key == VirtualKey.Up)
            {
                _screen.Viewport.Y--;
            }

            if (e.Key == VirtualKey.Down)
            {
                _screen.Viewport.Y++;
            }

            if (e.Key == VirtualKey.Subtract)
            {
                _screen.Viewport.Zoom -= 0.1f;
            }

            if (e.Key == VirtualKey.Add)
            {
                _screen.Viewport.Zoom += 0.1f;
            }
        }

        private void PointerReleased(object sender, PointerRoutedEventArgs e)
        {

        }

        private PointerPoint? _lastPointerPoint;

        private void PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint currentPoint = e.GetCurrentPoint(xe_Canvas);

            if (_lastPointerPoint is not null)
            {
                Vector2 moveDelta = new (
                    (float)(_lastPointerPoint.Position.X - currentPoint.Position.X),
                    (float)(_lastPointerPoint.Position.Y - currentPoint.Position.Y));

                if (currentPoint.Properties.IsMiddleButtonPressed && moveDelta != Vector2.Zero)
                {
                    _screen.Viewport.X += moveDelta.X;
                    _screen.Viewport.Y += moveDelta.Y;
                }
            }

            _lastPointerPoint = currentPoint;
        }

        private void WheelChanged(object sender, PointerRoutedEventArgs e)
        {
            //Take mouseCoords
            //Translate to worldCoords
            //Get screen coords of worldCoords
            //Apply delta(3 - 1)

            
            Point screenPosition = e.GetCurrentPoint(xe_Canvas).Position;
            Vector2 screenPos = new((float)screenPosition.X, (float)screenPosition.Y);
            Vector2 globalScreenPosition = _screen.Viewport.ResolveRealPosition(screenPos);//new(screenPosition.X + _screen.Viewport.X, screenPosition.Y + _screen.Viewport.Y);
            _screen.Viewport.Zoom += e.GetCurrentPoint(xe_Canvas).Properties.MouseWheelDelta / 1000f;
            Vector2 screenPositionAfterZoom = _screen.Viewport.ResolveLocalPosition(globalScreenPosition);
            Vector2 delta = screenPositionAfterZoom - screenPos;

            _screen.Viewport.X += delta.X;
            _screen.Viewport.Y += delta.Y;

        }

        private void ClearElements(object sender, RoutedEventArgs e)
        {

        }

        private async void CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {

        }
    }
}
