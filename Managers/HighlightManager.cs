using Godot;
using GridCombat.Global;

namespace GridCombat.Managers;

public partial class HighlightManager : Node2D
{

    public override void _Ready()
    {
        GameEvents.Instance.Connect(GameEvents.SignalName.HighlightCell, Callable.From<Vector2, Color>(HighlightCell));
        GameEvents.Instance.Connect(GameEvents.SignalName.ClearHighlights, Callable.From(Clear));
    }

    public void HighlightCell(Vector2 cell, Color color)
    {
        var rect = new ColorRect
        {
            Color = color,
            Size = new Vector2(16, 16),
            Position = new Vector2(cell.X - 8, cell.Y - 8),
            ZIndex = 1,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };

        AddChild(rect);   
    }

    public void Clear()
    {
        foreach(var child in GetChildren())
        {
            RemoveChild(child);
        }
    }

    public bool IsHighlightActive()
    {
        return GetChildCount() > 0;
    }
}
