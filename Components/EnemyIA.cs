using System.Collections.Generic;
using System.Linq;
using Godot;
using GridCombat.Units;

namespace GridCombat.Components;

public partial class EnemyIA : Node
{

    private Unit _parent;

    public override void _Ready()
    {
        _parent = GetParent<Unit>();
    }
    
    // private Unit GetTargetUnit()
    // {
    //     var targets = [];

    //     targets.Sort((a, b) =>
    //         {
    //             return a.Position
    //                     .DistanceTo(SelectedUnit.Position)
    //                     .CompareTo(b.Position.DistanceTo(SelectedUnit.Position));

    //         });
        
    //     return targets.First();
    // }

    private Vector2? GetReachableCellClosestToTarget(Unit target)
    {
        var currentPos = _parent.GridManager.LocalToMap(_parent.GlobalPosition);
        var targetPos = _parent.GridManager.LocalToMap(target.GlobalPosition);

        var path = _parent.GridManager.GetPathBetweenPoints(currentPos, targetPos)
            .Skip(1)
            .Select(_parent.GridManager.LocalToMap);
        
        var movableTiles = _parent.GridManager.GetMovableCellsInRange(_parent.GridManager.LocalToMap(_parent.GlobalPosition), 1);

        var closest = path.LastOrDefault(movableTiles.Contains);

        return closest == default ? null : _parent.GridManager.MapToLocal(closest);
    }
}
