
using System.Collections.Generic;
using System.Linq;
using Godot;
using GridCombat.Global;
using GridCombat.Managers;

namespace GridCombat.Units.Player;

public partial class Player : Unit
{

    [Export]
    public UnitGroupManager EnemyGroup;

    private AudioStreamPlayer _wrongOptionSFX;

    public override void _Ready()
    {
        base._Ready();
        
        _wrongOptionSFX = GetNode<AudioStreamPlayer>("%WrongOptionSFX");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (Moved && Attacked)
        {
            _sprite2D.Modulate = new Color(0.5f, 0.5f, 0.5f, 1);
        } else
        {
            _sprite2D.Modulate = new Color(1f, 1f, 1f, 1);
        }
    }

    public override void AttackOrMove()
    {
        Unit attackTarget = GetAttackTarget();

        if (attackTarget != null)
        {
            Attack(attackTarget);
        } else
        {
            Move();
        }
    }

    private Unit GetAttackTarget()
    {
        var currentPos = GridManager.MapToLocal(GridManager.LocalToMap(GlobalPosition));
        var targetPos = GridManager.GetMousePosition();

        List<Vector2I> attackCells = GridManager.GetAttackCellsInRange(currentPos, AttackRange);

        foreach (var unit in EnemyGroup.Units)
        {
            var enemyPos = GridManager.MapToLocal(GridManager.LocalToMap(unit.GlobalPosition));

            if (enemyPos == targetPos && attackCells.Contains(GridManager.LocalToMap(targetPos)))
            {
                return unit;
            }
        }

        return null;
    } 

    private bool ExistsEnemyNear()
    {
        var currentPos = GridManager.MapToLocal(GridManager.LocalToMap(GlobalPosition));

        List<Vector2I> attackCells = GridManager.GetAttackCellsInRange(currentPos, AttackRange);

        foreach (var unit in EnemyGroup.Units)
        {
            var enemyPos = GridManager.MapToLocal(GridManager.LocalToMap(unit.GlobalPosition));

            if (attackCells.Contains(GridManager.LocalToMap(enemyPos)))
            {
                return true;
            }
        }

        return false;
    }

    public override void Attack(Unit unit)
    {
        unit.TakeDamage(1);
        _isSelected = false;
        Attacked = true;
        Moved = true;
        EmitSignal(Unit.SignalName.UnitHasAttacked);
        GameEvents.EmitUnitMoved(this);
        GameEvents.EmitClearHighlights();
    }

    public override void Move()
    {
       var targetCell = GetTargetPosition();

       if (targetCell.X >= 0 && targetCell.Y >= 0)
        {
            Moved = true;
            _isSelected = false;

            GridManager.SetCellAsFree(GlobalPosition);
            _movementComponent.Move(targetCell);
            GridManager.SetCellAsOccupied(targetCell);

            EmitSignal(Unit.SignalName.UnitHasMoved);

            if (!ExistsEnemyNear())
            {
                Attacked = true;
                EmitSignal(Unit.SignalName.UnitHasAttacked);
            }
            
            GameEvents.EmitUnitMoved(this);
            GameEvents.EmitClearHighlights();
        }
    }

    private Vector2 GetTargetPosition()
    {
        var targetPos = GridManager.GetMousePosition();

        if (!GridManager.IsCellInRange(GlobalPosition, targetPos, MovementRange))
        {
            GD.Print("Selected position is out of range.");
            _wrongOptionSFX.Play();
            return new Vector2(-1, -1);
        }

        if (GridManager.IsCellOccupied(targetPos))
        {
            GD.Print("Selected position is occupied.");
            _wrongOptionSFX.Play();
            return new Vector2(-1, -1);
        }

        if (GridManager.IsYourCell(GlobalPosition, targetPos))
        {
            GD.Print("You already in this position.");
            _wrongOptionSFX.Play();
            return new Vector2(-1, -1);
        }

        return targetPos;
    
    }

    public override void OnSelection()
    {
        if ((!Moved || !Attacked) && !_isSelected && !HighlightManager.IsHighlightActive())
        {
            _isSelected = true;

            GameEvents.EmitUnitSelected(this);

            HighlightMoveCells();
            HighlightAttackCells();
        }        
    }

    private void HighlightMoveCells()
    {
        if (Moved)
        {
            return;
        }

        List<Vector2> cells = GridManager.GetMovableCellsInRange(GlobalPosition, MovementRange)
            .Select(GridManager.MapToLocal)
            .ToList();

        GameEvents.EmitClearHighlights();

        foreach(var cell in cells)
        {
            GameEvents.EmitHighlightCell(cell, new Color(0, 0, 1, 0.25f));
        }
    }

    private void HighlightAttackCells()
    {
        if (Attacked)
        {
            return;
        }

        foreach (var unit in EnemyGroup.Units)
        {
            var currentPos = GridManager.MapToLocal(GridManager.LocalToMap(GlobalPosition));
            var targetPos = GridManager.LocalToMap(unit.GlobalPosition);

            List<Vector2I> cells = GridManager.GetAttackCellsInRange(currentPos, AttackRange);

            foreach(Vector2I cell in cells)
            {
                if (cell == targetPos)
                {
                     GameEvents.EmitHighlightCell(GridManager.MapToLocal(targetPos), new Color(1, 0, 0, 0.25f));
                }
            }
        }
    }

    public override void TakeDamage(int damage)
    {
        Die();
    }
}