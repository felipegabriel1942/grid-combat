using System.Collections.Generic;
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
        }

        if (!ExistsEnemyNear())
        {
            Attacked = true;
        }

        Moved = true;
        EmitSignal(Unit.SignalName.UnitHasMoved);
    }

    private bool ExistsEnemyNear()
    {
        var currentPos = GridManager.MapToLocal(GridManager.LocalToMap(GlobalPosition));

        List<Vector2I> attackCells = GridManager.GetAttackCellsInRange(currentPos, AttackRange);

        foreach (var unit in PlayerGroup.Units)
        {
            var enemyPos = GridManager.MapToLocal(GridManager.LocalToMap(unit.GlobalPosition));

            if (attackCells.Contains(GridManager.LocalToMap(enemyPos)))
            {
                return true;
            }
        }

        return false;
    }

    public override void OnSelection()
    {
        
    }

     private Unit GetAttackTarget()
    {
        var currentPos = GridManager.MapToLocal(GridManager.LocalToMap(GlobalPosition));
        var targetPos = GridManager.MapToLocal(GridManager.LocalToMap(_enemyIA.GetTargetUnit().GlobalPosition));

        List<Vector2I> attackCells = GridManager.GetAttackCellsInRange(currentPos, AttackRange);

        foreach (var unit in PlayerGroup.Units)
        {
            var unitPos = GridManager.MapToLocal(GridManager.LocalToMap(unit.GlobalPosition));

            if (unitPos == targetPos && attackCells.Contains(GridManager.LocalToMap(targetPos)))
            {
                return unit;
            }
        }

        return null;
    } 

    public override void AttackOrMove()
    {
        Move();

        Unit attackTarget = GetAttackTarget();

        if (attackTarget != null)
        {
            Attack(attackTarget);
        }
    }

    public override void Attack(Unit target)
    {
        target.TakeDamage(1);
        Attacked = true;
        Moved = true;
        EmitSignal(Unit.SignalName.UnitHasAttacked);
    }

    public override void TakeDamage(int damage)
    {
        Die();
    }
}