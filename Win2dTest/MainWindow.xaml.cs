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
using Windows.Foundation;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Win2dTest
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private Map? _map;
        private CanvasSvgDocument? _svg;
        private PointerPoint? _prevPoint;
        private Rect _svgViewBox;
        private Rect _svgDestinationRect = new (50, 50, 800, 800);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Render(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            try
            {
                DrawFps(args);

                if (_svg is not null)
                {
                    args.DrawingSession.DrawSvg(
                        _svg,
                        new Size(
                            _svgDestinationRect.Width,
                            _svgDestinationRect.Height),
                        (float)_svgDestinationRect.X,
                        (float)_svgDestinationRect.Y);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_map is not null)
            {
                Vector2 svgCoords = GetTransformedPointerPosition(e.GetCurrentPoint(xe_Canvas));
                Region? region = _map.GetRegionFromCoords(svgCoords);
                if (region is not null)
                {
                    _map.SelectSingleRegion(region);
                }
                else
                {
                    _map.Regions.ForEach(r => r.Deselect());
                }
            }
        }

        private void PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pointerPoint = e.GetCurrentPoint(xe_Canvas);
            _prevPoint = pointerPoint;

            if (_map is null)
            {
                return;
            }

            Vector2 svgCoords = GetTransformedPointerPosition(pointerPoint);

            Region? region = _map.GetRegionFromCoords(svgCoords);
            if (region is not null)
            {
                _map.HighlightSingleRegion(region);
            }
            else
            {
                _map.Regions.ForEach(r => r.ClearHighlight());
            }
            
        }

        private void WheelChanged(object sender, PointerRoutedEventArgs e)
        {

        }

        private void DrawFps(CanvasAnimatedDrawEventArgs args)
        {
            TimeSpan timeSpan = args.Timing.ElapsedTime;
            int fps = 0;
            if (timeSpan != TimeSpan.Zero)
            {
                fps = (int)Math.Round(1 / (float)timeSpan.TotalSeconds);
            }

            args.DrawingSession.DrawText($"FPS: {fps}", 0, 0, Colors.Yellow);
        }

        private void ClearElements(object sender, RoutedEventArgs e)
        {

        }

        private async void CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            try
            {
                using IRandomAccessStream svgStream = File.OpenRead(@"C:\Users\AsyncMilk\Desktop\LandTest.svg").AsRandomAccessStream();
                _svg = await CanvasSvgDocument.LoadAsync(sender.Device, svgStream);

                CanvasSvgNamedElement root = _svg.Root;
                string[] viewBoxValues = root.GetStringAttribute("viewBox").Split(' ');
                _svgViewBox = new Rect(
                    double.Parse(viewBoxValues[0], CultureInfo.InvariantCulture),
                    double.Parse(viewBoxValues[1], CultureInfo.InvariantCulture),
                    double.Parse(viewBoxValues[2], CultureInfo.InvariantCulture),
                    double.Parse(viewBoxValues[3], CultureInfo.InvariantCulture)
                );

                _map = new Map(_svg, ["R1", "R2", "R3"]);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private Vector2 GetTransformedPointerPosition(PointerPoint pointerPoint)
        {
            Point rawPosition = pointerPoint.Position;

            if (!_svgDestinationRect.Contains(rawPosition))
            {
                return Vector2.Zero;
            }

            double scaleX = _svgDestinationRect.Width / _svgViewBox.Width;
            double scaleY = _svgDestinationRect.Height / _svgViewBox.Height;

            float x = (float)((rawPosition.X - _svgDestinationRect.Left) / scaleX);
            float y = (float)((rawPosition.Y - _svgDestinationRect.Top) / scaleY);

            return new Vector2(x, y);
        }
    }
}
