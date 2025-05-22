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

        public void Render(CanvasDrawingSession drawingSession, PointF origin, float scale);
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
                if (ResolveRenderOrigin(renderable, out PointF renderOrigin))
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

        public bool ResolveRenderOrigin(IRenderable renderable, out PointF renderOrigin)
        {
            if (ShouldRender(renderable))
            {
                renderOrigin = TranslatePoint(new PointF(renderable.X, renderable.Y), TranslationType.RealToDrawing, Zoom);
                return true;
            }

            renderOrigin = default;
            return false;
        }

        public void PanByDrawingVector(Vector2 panningVector)
        {
            Vector2 translatedVector = TranslateVector(panningVector, TranslationType.DrawingToViewport, Zoom);

            X += translatedVector.X;
            Y += translatedVector.Y;
        }

        public void PanByRealVector(Vector2 panningVector)
        {
            X += panningVector.X;
            Y += panningVector.Y;
        }

        // ZoomByFactorToPoint()
        // ZoomByDeltaToPoint()

        // ZoomToValue(float newZoom, Vector2 point = default)
        // ZoomByFactor(float zoomFactor, Vector2 point = default)
        // ZoomByDelta(int steps, Vector2 point = default, float zoomFactor = 1.1, int stepSize = 120)

        public void ZoomToPoint(PointF point, float newZoom)
        {
            float deltaZoom = newZoom / Zoom;
            PointF viewPortPoint = TranslatePoint(point, TranslationType.DrawingToViewport, Zoom);

            Zoom = newZoom;
            X += viewPortPoint.X - viewPortPoint.X / deltaZoom;
            Y += viewPortPoint.Y - viewPortPoint.Y / deltaZoom;

        }

        public Vector2 TranslateVector(Vector2 vector, TranslationType translationType, float scale = 1)
            => translationType switch
            {
                TranslationType.RealToViewport => vector,
                TranslationType.RealToDrawing => vector * scale,
                TranslationType.ViewportToReal => vector,
                TranslationType.ViewportToDrawing => vector * scale,
                TranslationType.DrawingToReal => vector / scale,
                TranslationType.DrawingToViewport => vector / scale,
                _ => throw new NotImplementedException(),
            };

        public PointF TranslatePoint(PointF point, TranslationType translationType, float scale = 1)
            => translationType switch
            {
                TranslationType.RealToViewport => new PointF(point.X - X, point.Y - Y),
                TranslationType.RealToDrawing => new PointF((point.X - X) * scale + DrawingOffsetX, (point.Y - Y) * scale + DrawingOffsetY),
                TranslationType.ViewportToReal => new PointF(point.X + X, point.Y + Y),
                TranslationType.ViewportToDrawing => new PointF(point.X * scale + DrawingOffsetX, point.Y * scale + DrawingOffsetY),
                TranslationType.DrawingToReal => new PointF((point.X - DrawingOffsetX) / scale + X, (point.Y - DrawingOffsetY) / scale + Y),
                TranslationType.DrawingToViewport => new PointF((point.X - DrawingOffsetX) / scale, (point.Y - DrawingOffsetY) / scale),
                _ => throw new NotImplementedException(),
            };


        private static void ThrowIfNegative(float value, string name = "")
        {
            if (float.IsNegative(value))
            {
                throw new ArgumentOutOfRangeException(name, $"Value {name} cannot be null");
            }
        }

        public enum TranslationType
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
