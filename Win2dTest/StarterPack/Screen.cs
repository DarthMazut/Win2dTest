using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Win2dTest.StarterPack
{
    public class Screen
    {
        private readonly List<IRenderable> _items = [];

        public Viewport? Viewport { get; set; }

        public List<IRenderable> Items => _items;

        public void Render(CanvasDrawingSession drawingSession)
        {

        }

        private
    }

    public interface IRenderable
    {
        public float X { get; }

        public float Y { get; }

        public void Render(CanvasDrawingSession drawingSession, Vector2 origin);
    }

    public class Viewport
    {
        public Viewport(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public float Width { get; set; }

        public float Height { get; set; }

        public float Zoom { get; set; }

        public float ScreenWidth { get; }

        public float ScreenHeight { get; }

        public float ScreenOffsetX { get; set; }

        public float ScreenOffsetY { get; set; }

        public float RenderOffsetX { get; set; }

        public float RenderOffsetY { get; set; }

        public void Render(CanvasDrawingSession drawingSession, ICollection<IRenderable> renderables)
        {
            foreach (IRenderable renderable in renderables.Where(ShouldRender))
            {
                if (ResolveRenderOrigin(renderable, out Vector2 renderOrigin))
                {
                    renderable.Render(drawingSession, renderOrigin);
                }
            }
        }

        public bool ShouldRender(IRenderable renderable)
        {

        }

        public bool ResolveRenderOrigin(IRenderable renderable, out Vector2 renderOrigin)
        {

        }

        private static void ThrowIfNegative(float value, string name = "")
        {
            if (float.IsNegative(value))
            {
                throw new ArgumentOutOfRangeException("Value cannot be null");
            }
        }
    }
}
