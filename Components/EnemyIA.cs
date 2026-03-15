using System.Linq;
using Godot;
using GridCombat.Units;
using GridCombat.Units.Enemy;

namespace GridCombat.Components;

public partial class EnemyIA : Node
{

    private Enemy _parent;

    public override void _Ready()
    {
        _parent = GetParent<Enemy>();
    }
    
    public Unit GetTargetUnit()
    {
        var targets = _parent.PlayerGroup.Units;

        targets.Sort((a, b) =>
            {
                return a.Position
                        .DistanceTo(_parent.GlobalPosition)
                        .CompareTo(b.Position.DistanceTo(_parent.GlobalPosition));

            });
        
        return targets.First();
    }

    public Vector2 GetReachableCellClosestToTarget(Unit target)
    {
        var currentPos = _parent.GridManager.LocalToMap(_parent.GlobalPosition);
        var targetPos = _parent.GridManager.LocalToMap(target.GlobalPosition);

        var path = _parent.GridManager.GetPathBetweenPoints(currentPos, targetPos)
            .Skip(1)
            .Select(_parent.GridManager.LocalToMap);
        
        var movableTiles = _parent.GridManager.GetMovableCellsInRange(_parent.GlobalPosition, 1);

        var closest = path.LastOrDefault(movableTiles.Contains);

        return closest == default ? new Vector2(-1, -1) : _parent.GridManager.MapToLocal(closest);
    }
}
