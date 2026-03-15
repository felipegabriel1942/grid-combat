using Godot;
using GridCombat.Components;
using GridCombat.Global;
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

    private MovementComponent _movementComponent;
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

    public void Move()
    {
       GridManager.SetCellAsFree(GlobalPosition);
       var targetCell = GetTargetPosition();

       if (targetCell.X >= 0 && targetCell.Y >= 0)
        {
            _movementComponent.Move(targetCell);
            GridManager.SetCellAsOccupied(targetCell);
            EmitSignal(SignalName.UnitHasMoved);
            GameEvents.EmitUnitMoved(this);
            GameEvents.EmitClearHighlights();
        }
    }

    public bool HasMoved()
    {
        return _movementComponent.Moved;
    }

    public abstract Vector2 GetTargetPosition();

    public abstract void OnSelection();
    
}
