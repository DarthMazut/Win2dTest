using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.UI;
using Windows.UI;

namespace Win2dTest.StarterPack
{
    public class ViewportFrame
    {
        public Color Color { get; set; } = Colors.Red;

        public float Thickness { get; set; } = 1f;

        public CanvasStrokeStyle StrokeStyle { get; set; } = new();

        public void Render(Viewport targetViewport, CanvasDrawingSession drawingSession)
        {
            drawingSession.DrawRectangle(
                targetViewport.DrawingOffsetX,
                targetViewport.DrawingOffsetY,
                targetViewport.DrawingWidth,
                targetViewport.DrawingHeight,
                Color,
                Thickness,
                StrokeStyle);
        }
    }
}
