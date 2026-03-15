using System.Threading.Tasks;
using Godot;
using GridCombat.Components;
using GridCombat.Managers;

namespace GridCombat.Units;

public abstract partial class Unit : Node2D
{

    [Signal]
    public delegate void UnitHasMovedEventHandler();
    
    [Export]
    public GridManager GridManager { private set; get; }

    [Export]
    public HighlightManager HighlightManager { private set; get; }

    protected MovementComponent _movementComponent;
    private SelectionComponent _selectionComponent;
    protected Sprite2D _sprite2D;
    protected bool _isSelected;
   
    public override void _Ready()
    {
        _movementComponent = GetNode<MovementComponent>("MovementComponent");
        _selectionComponent = GetNode<SelectionComponent>("SelectionComponent");
        _sprite2D = GetNode<Sprite2D>("Sprite2D");

        _selectionComponent.Selected += OnSelection;

        GridManager.SetCellAsOccupied(GlobalPosition);
    }

    public bool HasMoved()
    {
        return _movementComponent.Moved;
    }

    public abstract void OnSelection();

    public abstract void Move();

    public void SetMovedToFalse()
    {
        _movementComponent.Moved = false;
    }

    public async Task AwaitTime(float time)
    {
        await ToSignal(GetTree().CreateTimer(time), Timer.SignalName.Timeout);
    }
    
}
