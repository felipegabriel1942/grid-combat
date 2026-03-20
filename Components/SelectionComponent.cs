using Godot;
using GridCombat.Global;

namespace GridCombat.Components;

public partial class SelectionComponent : Area2D
{

    private bool _inputBlocked = false;

    [Signal]
    public delegate void SelectedEventHandler();

    public override void _Ready()
    {
        InputEvent += OnInputEvent;

        GameEvents.Instance.Connect(GameEvents.SignalName.ChangeInputBlockStatus, Callable.From<bool>(OnBlockInputStatusChange));
    }

    private void OnBlockInputStatusChange(bool isBlocked)
    {
        _inputBlocked = isBlocked;
    }
    
    private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                if (_inputBlocked)
                    return;

                EmitSignal(SignalName.Selected);
            }
        }
    }
}
