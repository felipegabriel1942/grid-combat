using System.Threading.Tasks;
using Godot;
using GridCombat.Components;
using GridCombat.Managers;

namespace GridCombat.Units;

public abstract partial class Unit : Node2D
{

    [Signal]
    public delegate void UnitHasMovedEventHandler();

    [Signal]
    public delegate void UnitHasAttackedEventHandler();

    [Signal]
    public delegate void UnitHasDiedEventHandler(Unit unit);
    
    [Export]
    public GridManager GridManager { private set; get; }

    [Export]
    public HighlightManager HighlightManager { private set; get; }

    [Export]
    public int AttackRange = 1;

    [Export]
    public int MovementRange = 1;

    public bool Moved = false;

    public bool Attacked = false;

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

    public abstract void OnSelection();

    public abstract void AttackOrMove();

    public abstract void Attack(Unit target);

    public abstract void Move();

    public async Task AwaitTime(float time)
    {
        await ToSignal(GetTree().CreateTimer(time), Timer.SignalName.Timeout);
    }

    public abstract void TakeDamage(int damage);

    public void Die()
    {
        GridManager.SetCellAsFree(GlobalPosition);
        EmitSignal(SignalName.UnitHasDied, this);
        QueueFree();
    }
    
}
