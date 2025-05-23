using Microsoft.Graphics.Canvas;
using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Win2dTest.StarterPack;
using Windows.UI;

namespace Win2dTest
{
    public class SelectableItem : IRenderable
    {
        public float Width { get; set; } = 100;

        public float Height { get; set; } = 100;

        public float X { get; set; }

        public float Y { get; set; }

        public Color FillColor { get; set; }

        public Color BorderColor { get; set; }

        public bool IsSelected { get; set; }

        public bool ContainsPoint(Vector2 point)
            => new System.Drawing.RectangleF(X, Y, Width, Height).Contains(point.X, point.Y);

        public void Render(CanvasDrawingSession drawingSession, Vector2 origin, float scale)
        {
            if (_targetMove is not null && DateTimeOffset.Now < _targetTime)
            {
                Vector2 targetVector = _targetMove.Value - new Vector2(X, Y);

            }

            if (IsSelected)
            {
                drawingSession.FillRectangle(origin.X - 1 * scale, origin.Y - 1 * scale, (Width + 2) * scale, (Height + 2) * scale, BorderColor);
            }
            
            drawingSession.FillRectangle(origin.X, origin.Y, Width * scale, Height * scale, FillColor);
            return;

        }

        private DateTimeOffset? _targetTime;
        private Vector2? _targetMove;

        public void IssueMoveOrder(Vector2 targetMove, TimeSpan duration)
        {
            _targetMove = targetMove;
            _targetTime = DateTimeOffset.Now + duration;
        }
    }
}
