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

    public override async void Move()
    {
        Unit targetUnit = _enemyIA.GetTargetUnit();

        Vector2 targetPosition = _enemyIA.GetReachableCellClosestToTarget(targetUnit);

        if (targetPosition.X >= 0 && targetPosition.Y >= 0)
        {
            if (targetPosition.X < Position.X)
            {
                FlipUnit("Left");
            } else
            {
                FlipUnit("Right");
            }

            GridManager.SetCellAsFree(GlobalPosition);
            await _movementComponent.Move(targetPosition);
            GridManager.SetCellAsOccupied(targetPosition);
        }

        if (!ExistsEnemyNear())
        {
            Attacked = true;
        } else
        {
            Unit attackTarget = GetAttackTarget();

            if (attackTarget != null)
            {
                Attack(attackTarget);
            }
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
    }

    public override void Attack(Unit target)
    {
        _player = target;

        if (_player.Position.X < Position.X)
        {
            FlipUnit("Left");
        } else
        {
            FlipUnit("Right");
        }

        _animationComponent.PlayAttack();
        Attacked = true;
        Moved = true;
        EmitSignal(Unit.SignalName.UnitHasAttacked);
    }

    public override async void TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        UpdateHealthContainer();

        await _animationComponent.PlayHurt();

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public override void PlayWeaponSound()
    {
        _weaponSound.Play();
    }

    private Unit _player; 

    public void HurtPlayer()
    {
        _player.TakeDamage(1);
    }

    public override Texture2D GetHealthFilledTexture()
    {
        return GD.Load<Texture2D>("res://Assets/UI/HealthFilled.png");
    }
}