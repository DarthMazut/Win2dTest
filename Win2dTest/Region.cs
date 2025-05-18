using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Svg;
using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Win2dTest
{
    public class Region : IRender, IDisposable
    {
        private static readonly string _idAttribute = "id";
        private static readonly string _pathAttribute = "d";

        private readonly string _name;
        private readonly CanvasGeometry _geometry;
        private readonly CanvasSvgNamedElement _svgElement;

        private bool _isSelected;
        private bool _isHighlighted;

        public Region(CanvasSvgNamedElement svgElement)
        {
            _name = svgElement.GetIdAttribute(_idAttribute);
            _svgElement = svgElement;
            _geometry = (svgElement.GetAttribute(_pathAttribute) as CanvasSvgPathAttribute)?.CreatePathGeometry() ??
                throw new ArgumentException("Could not create geometry from path attribute.");   
        }

        public string Name => _name;

        public bool IsSelected => _isSelected;

        public bool IsHighlighted => _isHighlighted;

        public void Select()
        {
            _svgElement.SetStringAttribute("stroke-dasharray", "4 4");
            _isSelected = true;
        }

        public void Deselect()
        {
            _svgElement.SetStringAttribute("stroke-dasharray", "none");
            _isSelected = false;
        }

        public void Highlight()
        {
            _svgElement.SetColorAttribute("fill", Colors.Red);
            _isHighlighted = true;
        }

        public void ClearHighlight()
        {
            _svgElement.SetColorAttribute("fill", Colors.Green);
            _isHighlighted = true;
        }

        public bool CheckPointerOver(Vector2 pointerCoords)
            => _geometry.FillContainsPoint(pointerCoords) &&
                !_geometry.StrokeContainsPoint(pointerCoords, 2);

        public void Render(CanvasDrawingSession drawingSession)
        {
            drawingSession.DrawGeometry(_geometry, new Vector2(50, 50), Colors.Orange);

        }

        public void Dispose() => _geometry.Dispose();
    }
}
