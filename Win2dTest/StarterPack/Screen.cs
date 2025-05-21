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
                fps = (int)Math.Round(1 / (float)timeSpan.TotalSeconds);
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
                renderOrigin = ResolveLocalPosition(new(renderable.X, renderable.Y));//new Vector2((renderable.X - X) * Zoom, (renderable.Y - Y) * Zoom);
                return true;
            }

            renderOrigin = default;
            return false;
        }

        public Vector2 ResolveLocalPosition(Vector2 screenCoords) => new(screenCoords.X - X, screenCoords.Y - Y);

        public Vector2 ResolveRealPosition(Vector2 viewportCoords) => new(viewportCoords.X + X, viewportCoords.Y + Y);

        private static void ThrowIfNegative(float value, string name = "")
        {
            if (float.IsNegative(value))
            {
                throw new ArgumentOutOfRangeException(name, $"Value {name} cannot be null");
            }
        }
    }
}
