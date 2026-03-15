using Godot;

namespace GridCombat.Components;

public partial class SelectionComponent : Area2D
{

    [Signal]
    public delegate void SelectedEventHandler();

    public override void _Ready()
    {
        InputEvent += OnInputEvent;
    }
    
    private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                EmitSignal(SignalName.Selected);
            }
        }
    }
}
