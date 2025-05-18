using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win2dTest
{
    public interface IRender
    {
        public void Render(CanvasDrawingSession drawingSession);
    }
}
