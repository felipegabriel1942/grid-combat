using Godot;

using System.Linq;
using System.Collections.Generic;
using GridCombat.Units;

namespace GridCombat.Managers;

public partial class UnitGroupManager : Node2D
{
    [Signal]
    public delegate void AllUnitsMovedEventHandler();

    public List<Unit> Units;

    public override void _Ready()
    {
        Units = [.. GetChildren().Cast<Unit>()];

        foreach (Unit unit in Units)
        {
            unit.UnitHasMoved += CheckIfAllUnitsMoved;
        }
    }

    private void CheckIfAllUnitsMoved()
    {
        var unitNotMoved = Units
            .Where(u => !u.HasMoved())
            .ToList();

        if (unitNotMoved.Count == 0)
        {
            EmitSignal(SignalName.AllUnitsMoved);
        }
    }
}
