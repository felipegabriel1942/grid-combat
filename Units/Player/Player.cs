
using System.Collections.Generic;
using System.Linq;
using Godot;
using GridCombat.Global;

namespace GridCombat.Units.Player;

public partial class Player : Unit
{

    private AudioStreamPlayer _wrongOptionSFX;

    public override void _Ready()
    {
        base._Ready();
        
        _wrongOptionSFX = GetNode<AudioStreamPlayer>("%WrongOptionSFX");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (HasMoved())
        {
            _sprite2D.Modulate = new Color(0.5f, 0.5f, 0.5f, 1);
        } else
        {
            _sprite2D.Modulate = new Color(1f, 1f, 1f, 1);
        }
    }

    public override Vector2 GetTargetPosition()
    {
        var targetPos = GridManager.GetMousePosition();

        if (!GridManager.IsCellInRange(GlobalPosition, targetPos, 1))
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
        if (!HasMoved() && !_isSelected && !HighlightManager.IsHighlightActive())
        {
            _isSelected = true;

            GameEvents.EmitUnitSelected(this);

            List<Vector2> cells = GridManager.GetMovableCellsInRange(GlobalPosition, 1)
                .Select(GridManager.MapToLocal)
                .ToList();

            GameEvents.EmitClearHighlights();

            foreach(var cell in cells)
            {
                GameEvents.EmitHighlightCell(cell, new Color(0, 0, 1, 0.25f));
            }
        }
    }
}