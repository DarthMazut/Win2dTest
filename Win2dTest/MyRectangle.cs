using Microsoft.Graphics.Canvas;
using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Win2dTest.StarterPack;

namespace Win2dTest
{
    public class MyRectangle : IRenderable
    {
        public float Width => 100;

        public float Height => 100;

        public float X => 50;

        public float Y => 50;

        public void Render(CanvasDrawingSession drawingSession, Vector2 origin, float scale)
        {
            drawingSession.DrawRectangle(origin.X, origin.Y, Width * scale, Height * scale, Colors.Green);
        }
    }
}
