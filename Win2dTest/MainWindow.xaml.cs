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
                {
                    DrawingOffsetX = 100,
                    DrawingOffsetY = 50
                }
            };

            _screen.Items.Add(new SelectableItem()
            {
                X = 100,
                Y = 100,
                FillColor = Colors.BlueViolet,
                BorderColor = Colors.Red
            });

            _screen.Items.Add(new SelectableItem()
            {
                X = 220,
                Y = 100,
                FillColor = Colors.BlueViolet,
                BorderColor = Colors.Red
            });

            _screen.Items.Add(new SelectableItem()
            {
                X = 100,
                Y = 220,
                FillColor = Colors.BlueViolet,
                BorderColor = Colors.Red
            });

            _screen.Items.Add(new SelectableItem()
            {
                X = 220,
                Y = 220,
                FillColor = Colors.BlueViolet,
                BorderColor = Colors.Red
            });
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
                _screen.Viewport.Pan(new Vector2(-1, 0), Viewport.CoordinatesType.Viewport);
            }

            if (e.Key == VirtualKey.Left)
            {
                _screen.Viewport.Pan(new Vector2(1, 0), Viewport.CoordinatesType.Viewport);
            }

            if (e.Key == VirtualKey.Up)
            {
                _screen.Viewport.Pan(new Vector2(0, 1), Viewport.CoordinatesType.Viewport);
            }

            if (e.Key == VirtualKey.Down)
            {
                _screen.Viewport.Pan(new Vector2(0, -1), Viewport.CoordinatesType.Viewport);
            }

            if (e.Key == VirtualKey.Subtract)
            {
                _screen.Viewport.ZoomByFactor(0.9f, default, Viewport.CoordinatesType.Viewport);
            }

            if (e.Key == VirtualKey.Add)
            {
                _screen.Viewport.ZoomByFactor(1.1f, default, Viewport.CoordinatesType.Viewport);
            }
        }

        private void PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pointerPoint = e.GetCurrentPoint(xe_Canvas);
            Point pointerPosition = pointerPoint.Position;
            Vector2 clickPosition = new Vector2((float)pointerPosition.X, (float)pointerPosition.Y);
            Vector2 realClickPosition = _screen.Viewport.TranslatePoint(clickPosition, Viewport.CoordinatesType.Drawing, Viewport.CoordinatesType.Real);

            foreach (SelectableItem item in _screen.Items.Where(i => i is SelectableItem))
            {
                item.IsSelected = item.ContainsPoint(realClickPosition);

            }
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

                if (currentPoint.Properties.IsMiddleButtonPressed)
                {
                    _screen.Viewport.Pan(moveDelta);
                }
                else if (currentPoint.Properties.IsLeftButtonPressed)
                {
                    List<SelectableItem> selectedItems = _screen.Items.Where(i => (i as SelectableItem)?.IsSelected == true).Cast<SelectableItem>().ToList();
                    Vector2 realMoveDelta = _screen.Viewport.TranslateVector(moveDelta, Viewport.CoordinatesType.Drawing, Viewport.CoordinatesType.Real);
                    selectedItems.ForEach(i =>
                    {
                        i.X -= realMoveDelta.X;
                        i.Y -= realMoveDelta.Y;
                    });
                }
            }

            _lastPointerPoint = currentPoint;
        }

        private void WheelChanged(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pointer = e.GetCurrentPoint(xe_Canvas);
            Vector2 cursor = new((float)pointer.Position.X, (float)pointer.Position.Y);
            _screen.Viewport.ZoomByDelta(pointer.Properties.MouseWheelDelta, cursor);
        }

        private void ClearElements(object sender, RoutedEventArgs e)
        {
            
        }

        private async void CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {

        }
    }
}
