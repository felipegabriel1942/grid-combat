using Godot;

namespace GridCombat.UI;

public partial class SelectionCursor : Node2D
{
    [Signal]
    public delegate void SelectionCursorClickedEventHandler();

    private Area2D _area2D;

    public override void _Ready()
    {
        _area2D = GetNode<Area2D>("Area2D");
        _area2D.InputEvent += OnClick;
    }

    private void OnClick(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                EmitSignal(SignalName.SelectionCursorClicked);
            }
        }
    }
}
