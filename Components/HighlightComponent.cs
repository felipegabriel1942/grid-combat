using Godot;
using System.Collections.Generic;

namespace GridCombat.Components;

public partial class HighlightComponent : Node2D
{
    
    public void HighlightCells(List<Vector2> cells, Color color)
    {
        foreach (var cell in cells)
        {
            var rect = new ColorRect
            {
                Color = color,
                Size = new Vector2(16, 16),
                Position = new Vector2(cell.X - 8 , cell.Y - 8),
                ZIndex = 4,
                MouseFilter = Control.MouseFilterEnum.Ignore
            };

            GetParent().GetParent().AddChild(rect);
        }
    }

    public void Clear()
    {
        foreach(var child in GetChildren())
        {
            RemoveChild(child);
        }
    }
}
