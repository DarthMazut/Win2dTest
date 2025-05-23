using Microsoft.Graphics.Canvas;
using System.Collections.Generic;
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
            Viewport?.Render(drawingSession, Items);
        }
    }
}
