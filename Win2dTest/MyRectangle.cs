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
    public class MyRectangle : IRenderable
    {
        public MyRectangle(float width, float height)
        {
            Width = width;
            Height = height;
        }

        //X,
        //Y,
        //Width,
        //Height,
        //BorderThickness,
        //BorderColor
        //Color
        //Fill

        public float Width { get; set; }

        public float Height { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public Color FillColor { get; set; }

        public Color BorderColor { get; set; }

        public void Render(CanvasDrawingSession drawingSession, Vector2 origin, float scale)
        {
            //drawingSession.DrawRectangle(origin.X, origin.Y, Width * scale, Height * scale, BorderColor);
            drawingSession.DrawCircle(origin + new Vector2(Width / 2 * scale), Width / 2 * scale, BorderColor);
        }
    }
}
