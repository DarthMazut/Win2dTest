using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using System;

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
}
