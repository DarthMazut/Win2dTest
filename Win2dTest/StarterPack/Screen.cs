using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Win2dTest.StarterPack
{
    public static class FpsHelper
    {
        public static void DrawFps(CanvasAnimatedDrawEventArgs args)
        {
            TimeSpan timeSpan = args.Timing.ElapsedTime;
            int fps = 0;
            if (timeSpan != TimeSpan.Zero)
            {
                fps = (int)Math.Round(1 / timeSpan.TotalSeconds);
            }

            args.DrawingSession.DrawText($"FPS: {fps}", 0, 0, Colors.Yellow);
        }
    }

    public class Screen
    {
        private readonly List<IRenderable> _items = [];

        public Viewport? Viewport { get; set; }

        public List<IRenderable> Items => _items;

        public void Render(CanvasDrawingSession drawingSession)
        {
            Viewport?.Render(drawingSession, Items);
        }
    }

    public interface IRenderable
    {
        public float Width { get; }

        public float Height { get; }

        public float X { get; }

        public float Y { get; }

        public void Render(CanvasDrawingSession drawingSession, Vector2 origin, float scale);
    }

    public class Viewport
    {
        public Viewport(float width, float height)
        {
            ThrowIfNegative(width, nameof(DrawingWidth));
            ThrowIfNegative(height, nameof(DrawingHeight));

            DrawingWidth = width;
            DrawingHeight = height;
        }

        public float DrawingWidth { get; set; }

        public float DrawingHeight { get; set; }

        public float Zoom { get; set; } = 1;

        public float ScreenWidth => DrawingWidth / Zoom;

        public float ScreenHeight => DrawingHeight / Zoom;

        public float X { get; set; }

        public float Y { get; set; }

        public float DrawingOffsetX { get; set; }

        public float DrawingOffsetY { get; set; }

        public void Render(CanvasDrawingSession drawingSession, ICollection<IRenderable> renderables)
        {
            drawingSession.DrawRectangle(DrawingOffsetX, DrawingOffsetY, DrawingWidth, DrawingHeight, Colors.Red);

            foreach (IRenderable renderable in renderables.Where(ShouldRender))
            {
                if (ResolveRenderOrigin(renderable, out Vector2 renderOrigin))
                {
                    using CanvasActiveLayer layer = drawingSession.CreateLayer(1, new Rect(DrawingOffsetX, DrawingOffsetY, DrawingWidth, DrawingHeight));
                    renderable.Render(drawingSession, renderOrigin, Zoom);
                }
            }
        }

        public bool ShouldRender(IRenderable renderable)
        {
            RectangleF viewportRect = new(X, Y, ScreenWidth, ScreenHeight);
            RectangleF renderableRect = new(renderable.X, renderable.Y, renderable.Width, renderable.Height);

            return viewportRect.IntersectsWith(renderableRect);
        }

        public bool ResolveRenderOrigin(IRenderable renderable, out Vector2 renderOrigin)
        {
            if (ShouldRender(renderable))
            {
                renderOrigin = TranslatePoint(new Vector2(renderable.X, renderable.Y), CoordinatesType.Real, CoordinatesType.Drawing);
                return true;
            }

            renderOrigin = default;
            return false;
        }

        public void Pan(Vector2 panningVector, CoordinatesType coordsType = CoordinatesType.Drawing)
        {
            Vector2 translatedVector = TranslateVector(panningVector, coordsType, CoordinatesType.Viewport);

            X += translatedVector.X;
            Y += translatedVector.Y;
        }

        public void ZoomToValue(float newZoom, Vector2 point = default, CoordinatesType coordsType = CoordinatesType.Drawing)
        {
            float deltaZoom = newZoom / Zoom;
            Vector2 viewportPoint = TranslatePoint(point, coordsType, CoordinatesType.Viewport);

            Zoom = newZoom;
            X += viewportPoint.X - viewportPoint.X / deltaZoom;
            Y += viewportPoint.Y - viewportPoint.Y / deltaZoom;
        }

        public void ZoomByFactor(float zoomFactor, Vector2 point = default, CoordinatesType coordsType = CoordinatesType.Drawing)
        {
            Vector2 viewportPoint = TranslatePoint(point, coordsType, CoordinatesType.Viewport);

            Zoom *= zoomFactor;
            X += viewportPoint.X - viewportPoint.X / zoomFactor;
            Y += viewportPoint.Y - viewportPoint.Y / zoomFactor;
        }

        public void ZoomByDelta(int steps, Vector2 point = default, CoordinatesType coordsType = CoordinatesType.Drawing, float zoomFactor = 1.1f, int stepSize = 120)
        {
            float deltaZoom = (float)Math.Pow(zoomFactor, steps / stepSize);
            ZoomByFactor(deltaZoom, point, coordsType);
        }

        public Vector2 TranslateVector(Vector2 vector, CoordinatesType inputCoordsType, CoordinatesType outputCoordsType)
        {
            if (inputCoordsType == CoordinatesType.Real)
            {
                return outputCoordsType switch
                {
                    CoordinatesType.Real => vector,
                    CoordinatesType.Viewport => vector,
                    CoordinatesType.Drawing => vector * Zoom,
                    _ => throw new NotImplementedException(),
                };
            }

            if (inputCoordsType == CoordinatesType.Viewport)
            {
                return outputCoordsType switch
                {
                    CoordinatesType.Real => vector,
                    CoordinatesType.Viewport => vector,
                    CoordinatesType.Drawing => vector * Zoom,
                    _ => throw new NotImplementedException(),
                };
            }

            if (inputCoordsType == CoordinatesType.Drawing)
            {
                return outputCoordsType switch
                {
                    CoordinatesType.Real => vector / Zoom,
                    CoordinatesType.Viewport => vector / Zoom,
                    CoordinatesType.Drawing => vector,
                    _ => throw new NotImplementedException(),
                };
            }

            throw new NotImplementedException();
        }

        public Vector2 TranslatePoint(Vector2 point, CoordinatesType inputCoordsType, CoordinatesType outputCoordsType)
        {
            if (inputCoordsType == CoordinatesType.Real)
            {
                return outputCoordsType switch
                {
                    CoordinatesType.Real => point,
                    CoordinatesType.Viewport => new Vector2(point.X - X, point.Y - Y),
                    CoordinatesType.Drawing => new Vector2(point.X - X, point.Y - Y) * Zoom + new Vector2(DrawingOffsetX, DrawingOffsetY),
                    _ => throw new NotImplementedException(),
                };
            }

            if (inputCoordsType == CoordinatesType.Viewport)
            {
                return outputCoordsType switch
                {
                    CoordinatesType.Real => new Vector2(point.X + X, point.Y + Y),
                    CoordinatesType.Viewport => point,
                    CoordinatesType.Drawing => new Vector2(point.X, point.Y) * Zoom + new Vector2(DrawingOffsetX, DrawingOffsetY),
                    _ => throw new NotImplementedException(),
                };
            }
            
            if (inputCoordsType == CoordinatesType.Drawing)
            {
                return outputCoordsType switch
                {
                    CoordinatesType.Real => new Vector2((point.X - DrawingOffsetX) / Zoom + X, (point.Y - DrawingOffsetY) / Zoom + Y),
                    CoordinatesType.Viewport => new Vector2(point.X - DrawingOffsetX, point.Y - DrawingOffsetY) / Zoom,
                    CoordinatesType.Drawing => point,
                    _ => throw new NotImplementedException(),
                };
            }

            throw new NotImplementedException();
        }

        private static void ThrowIfNegative(float value, string name = "")
        {
            if (float.IsNegative(value))
            {
                throw new ArgumentOutOfRangeException(name, $"Value {name} cannot be null");
            }
        }

        public enum CoordinatesType
        {
            Real,
            Viewport,
            Drawing
        }
    }
}
