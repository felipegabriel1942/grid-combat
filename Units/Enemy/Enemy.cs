using Godot;
using GridCombat.Components;
using GridCombat.Managers;

namespace GridCombat.Units.Enemy;

public partial class Enemy : Unit
{
    [Export]
    public UnitGroupManager PlayerGroup;

    private EnemyIA _enemyIA;

    public override void _Ready()
    {
        base._Ready();

        _enemyIA = GetNode<EnemyIA>("EnemyIA");
    }

    public override void Move()
    {
        Unit targetUnit = _enemyIA.GetTargetUnit();

        Vector2 targetPosition = _enemyIA.GetReachableCellClosestToTarget(targetUnit);

        if (targetPosition.X >= 0 && targetPosition.Y >= 0)
        {
            GridManager.SetCellAsFree(GlobalPosition);
            _movementComponent.Move(targetPosition);
            GridManager.SetCellAsOccupied(targetPosition);
            EmitSignal(Unit.SignalName.UnitHasMoved);
        }
    }

    public override void OnSelection()
    {
        
    }
}