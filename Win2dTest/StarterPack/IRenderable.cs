using Microsoft.Graphics.Canvas;
using System.Numerics;

namespace Win2dTest.StarterPack
{
    public interface IRenderable
    {
        public float Width { get; }

        public float Height { get; }

        public float X { get; }

        public float Y { get; }

        public void Render(CanvasDrawingSession drawingSession, Vector2 origin, float scale);
    }
}
