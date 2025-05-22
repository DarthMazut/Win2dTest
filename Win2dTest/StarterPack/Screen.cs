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
            ThrowIfNegative(width, nameof(Width));
            ThrowIfNegative(height, nameof(Height));

            Width = width;
            Height = height;
        }

        public float Width { get; set; }

        public float Height { get; set; }

        public float Zoom { get; set; } = 1;

        public float ScreenWidth => Width / Zoom;

        public float ScreenHeight => Height / Zoom;

        public float X { get; set; }

        public float Y { get; set; }

        public float CanvasOffsetX { get; set; }

        public float CanvasOffsetY { get; set; }

        public void Render(CanvasDrawingSession drawingSession, ICollection<IRenderable> renderables)
        {
            drawingSession.DrawRectangle(0, 0, Width, Height, Colors.Red);

            foreach (IRenderable renderable in renderables.Where(ShouldRender))
            {
                if (ResolveRenderOrigin(renderable, out Vector2 renderOrigin))
                {
                    using CanvasActiveLayer layer = drawingSession.CreateLayer(1, new Rect(0, 0, Width, Height));
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
                renderOrigin = TranslateVector(new Vector2(renderable.X, renderable.Y), VectorTranslationType.RealToDrawing, Zoom);
                return true;
            }

            renderOrigin = default;
            return false;
        }

        // Pan(Vector2 panningVector)

        // ZoomByFactorToPoint()
        // ZoomByDeltaToPoint()

        // ZoomToValue(float newZoom, Vector2 point = default)
        // ZoomByFactor(float zoomFactor, Vector2 point = default)
        // ZoomByDelta(int steps, Vector2 point = default, float zoomFactor = 1.1, int stepSize = 120)

        public void ZoomToPoint(Vector2 point, float newZoom)
        {
            float deltaZoom = newZoom / Zoom;
            Vector2 viewPortPoint = TranslateVector(point, VectorTranslationType.DrawingToViewport, Zoom);

            Zoom = newZoom;
            X += viewPortPoint.X - viewPortPoint.X / deltaZoom;
            Y += viewPortPoint.Y - viewPortPoint.Y / deltaZoom;

        }

        public Vector2 TranslateVector(Vector2 vector, VectorTranslationType translationType, float scale = 1)
            => translationType switch
            {
                VectorTranslationType.RealToViewport => new Vector2(vector.X - X, vector.Y - Y),
                VectorTranslationType.RealToDrawing => new Vector2(vector.X - X, vector.Y - Y) * scale,
                VectorTranslationType.ViewportToReal => new Vector2(vector.X + X, vector.Y + Y),
                VectorTranslationType.ViewportToDrawing => new Vector2(vector.X, vector.Y) * scale,
                VectorTranslationType.DrawingToReal => new Vector2(vector.X / scale + X, vector.Y / scale + Y),
                VectorTranslationType.DrawingToViewport => new Vector2(vector.X, vector.Y) / scale,
                _ => throw new NotImplementedException(),
            };


        private static void ThrowIfNegative(float value, string name = "")
        {
            if (float.IsNegative(value))
            {
                throw new ArgumentOutOfRangeException(name, $"Value {name} cannot be null");
            }
        }

        public enum VectorTranslationType
        {
            RealToViewport,
            RealToDrawing,
            ViewportToReal,
            ViewportToDrawing,
            DrawingToReal,
            DrawingToViewport
        }
    }
}
