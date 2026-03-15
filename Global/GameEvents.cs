using Godot;
using GridCombat.Units;

namespace GridCombat.Global;

public partial class GameEvents : Node
{
    public static GameEvents Instance { get; private set; }

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            Instance = this;
        }
    }

    [Signal]
    public delegate void HighlightCellEventHandler(Vector2 cell, Color color);

    [Signal]
    public delegate void ClearHighlightsEventHandler();

    [Signal]
    public delegate void UnitSelectedEventHandler(Unit unit);

    [Signal]
    public delegate void UnitMovedEventHandler(Unit unit);

    public static void EmitHighlightCell(Vector2 cell, Color color)
    {
        Instance.EmitSignal(SignalName.HighlightCell, cell, color);
    }

    public static void EmitClearHighlights()
    {
        Instance.EmitSignal(SignalName.ClearHighlights);
    }

    public static void EmitUnitSelected(Unit unit)
    {
        Instance.EmitSignal(SignalName.UnitSelected, unit);
    }

    public static void EmitUnitMoved(Unit unit)
    {
        Instance.EmitSignal(SignalName.UnitMoved, unit);
    }
}