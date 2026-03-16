using Godot;

using System.Linq;
using System.Collections.Generic;
using GridCombat.Units;

namespace GridCombat.Managers;

public partial class UnitGroupManager : Node2D
{
    [Signal]
    public delegate void AllUnitsMovedEventHandler();

    [Signal]
    public delegate void AllUnitsDiedEventHandler();

    public List<Unit> Units;

    public override void _Ready()
    {
        Units = [.. GetChildren().Cast<Unit>()];

        foreach (Unit unit in Units)
        {
            unit.UnitHasMoved += CheckIfAllUnitsActed;
            unit.UnitHasAttacked += CheckIfAllUnitsActed;
            unit.UnitHasDied += RemoveDeadUnit;
        }
    }

    private void RemoveDeadUnit(Unit unit)
    {
        Units.Remove(unit);

        if (Units.Count == 0)
        {
            EmitSignal(SignalName.AllUnitsDied);
        }
    }

    private void CheckIfAllUnitsActed()
    {
        var unitActed = Units
            .Where(u => u.Moved && u.Attacked)
            .ToList();

        if (unitActed.Count == Units.Count)
        {
            EmitSignal(SignalName.AllUnitsMoved);
        }
    }
}
