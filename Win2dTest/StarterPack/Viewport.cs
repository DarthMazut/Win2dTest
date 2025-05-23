using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Win2dTest.StarterPack
{
    public class Viewport
    {
        private float _drawingWidth;
        private float _drawingHeight;
        private float _maxZoom;
        private float _minZoom;

        public Viewport(float width, float height)
        {
            DrawingWidth = width;
            DrawingHeight = height;
        }

        public float DrawingWidth
        {
            get => _drawingWidth;
            set
            {
                ThrowIfNegative(value, nameof(DrawingWidth));
                _drawingWidth = value;
            }
        }

        public float DrawingHeight
        {
            get => _drawingHeight;
            set
            {
                ThrowIfNegative(value, nameof(DrawingHeight));
                _drawingHeight = value;
            }
        }

        public float Zoom { get; set; } = 1;

        public float MaxZoom
        {
            get => _maxZoom;
            set
            {
                ThrowIfNegative(value, nameof(MaxZoom));
                _maxZoom = value;
            }
        }

        public float MinZoom
        {
            get => _minZoom;
            set
            {
                ThrowIfNegative(value, nameof(MinZoom));
                _minZoom = value;
            }
        }

        public float ScreenWidth => DrawingWidth / Zoom;

        public float ScreenHeight => DrawingHeight / Zoom;

        public float X { get; set; }

        public float Y { get; set; }

        public float DrawingOffsetX { get; set; }

        public float DrawingOffsetY { get; set; }

        public float TopBound { get; set; } = float.NaN;

        public float BottomBound { get; set; } = float.NaN;

        public float LeftBound { get; set; } = float.NaN;

        public float RightBound { get; set; } = float.NaN;

        public ViewportDiagnostics Diagnostics { get; } = new();

        public void Render(CanvasDrawingSession drawingSession, ICollection<IRenderable> renderables)
        {
            foreach (IRenderable renderable in renderables.Where(ShouldRender))
            {
                if (ResolveRenderOrigin(renderable, out Vector2 renderOrigin))
                {
                    using CanvasActiveLayer layer = drawingSession.CreateLayer(1, new Rect(DrawingOffsetX, DrawingOffsetY, DrawingWidth, DrawingHeight));
                    // TODO: need to cut to boundaries...
                    renderable.Render(drawingSession, renderOrigin, Zoom);
                }
            }

            Diagnostics.Frame?.Render(this, drawingSession);
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
                renderOrigin = TranslatePoint(new Vector2(renderable.X, renderable.Y), TranslationType.RealToDrawing);
                return true;
            }

            renderOrigin = default;
            return false;
        }

        public void Pan(Vector2 panningVector, TranslationType translationType = TranslationType.DrawingToReal)
        {
            Vector2 translatedVector = TranslateVector(panningVector, translationType);

            X += translatedVector.X;
            Y += translatedVector.Y;
        }

        public void ZoomByDelta(int steps, Vector2 point = default, TranslationType translationType = TranslationType.DrawingToReal, float zoomFactor = 1.1f, int stepSize = 120)
        {
            float deltaZoom = (float)Math.Pow(zoomFactor, steps / stepSize);
            ZoomByFactor(deltaZoom, point,translationType);
        }

        public void ZoomToValue(float newZoom, Vector2 point = default, TranslationType translationType = TranslationType.DrawingToReal)
        {
            float deltaZoom = newZoom / Zoom;
            ZoomByFactor(deltaZoom, point, translationType);
        }

        public void ZoomByFactor(float zoomFactor, Vector2 point = default, TranslationType translationType = TranslationType.DrawingToReal)
        {
            Vector2 p1 = TranslatePoint(point, TranslationType.DrawingToReal);
            Zoom *= zoomFactor;
            Vector2 p2 = TranslatePoint(point, TranslationType.DrawingToReal);

            Vector2 movingVector = p1 - p2;

            X += movingVector.X;
            Y += movingVector.Y;
        }

        public Vector2 TranslateVector(Vector2 vector, TranslationType translationType)
            => translationType switch
            {
                TranslationType.None => vector,
                TranslationType.RealToDrawing => vector * Zoom,
                TranslationType.DrawingToReal => vector / Zoom,
                _ => throw new NotImplementedException(),
            };

        public Vector2 TranslatePoint(Vector2 point, TranslationType translationType)
            => translationType switch
            {
                TranslationType.None => point,
                TranslationType.RealToDrawing => new Vector2(point.X - X, point.Y - Y) * Zoom + new Vector2(DrawingOffsetX, DrawingOffsetY),
                TranslationType.DrawingToReal => new Vector2((point.X - DrawingOffsetX) / Zoom + X, (point.Y - DrawingOffsetY) / Zoom + Y),
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
            None,
            RealToDrawing,
            DrawingToReal
        }
    }
}
