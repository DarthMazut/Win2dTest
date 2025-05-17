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
    public class Map
    {
        private readonly ImmutableList<Region> _regions;

        public Map(CanvasSvgDocument mapSvg, string[] ids)
        {
            _regions = ids
                .Select(id => mapSvg.FindElementById(id))
                .Select(e => new Region(this, e))
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
    }
}
