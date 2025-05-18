using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Svg;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Win2dTest
{
    public class Map : IRender, IDisposable
    {
        private readonly ImmutableList<Region> _regions;

        public Map(CanvasSvgDocument mapSvg, string[] ids)
        {
            _regions = ids
                .Select(id => mapSvg.FindElementById(id))
                .Select(e => new Region(e))
                .ToImmutableList();
        }

        public ImmutableList<Region> Regions => _regions;

        public Region? GetRegionFromCoords(Vector2 coords)
            => Regions.FirstOrDefault(r => r.CheckPointerOver(coords));

        public void HighlightSingleRegion(Region region)
        {
            Regions.ForEach(r => r.ClearHighlight());
            region.Highlight();
        }

        public void SelectSingleRegion(Region region)
        {
            Regions.ForEach(r => r.Deselect());
            region.Select();
        }

        public void Render(CanvasDrawingSession drawingSession) => Regions.ForEach(r => r.Render(drawingSession));

        public void Dispose() => Regions.ForEach(r => r.Dispose());
    }
}
