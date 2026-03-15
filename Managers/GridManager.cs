using Godot;

using System.Collections.Generic;

namespace GridCombat.Managers;

public partial class GridManager : Node
{
    private TileMapLayer _tileMap;
    private int _cellSize = 16;
    private int _movementRange = 1;
    private AStarGrid2D _grid;

    public override void _Ready()
    {

        _tileMap = GetChild<TileMapLayer>(0);

        _grid = new AStarGrid2D()
        {
            Region = _tileMap.GetUsedRect(),
            CellSize = new Vector2I(_cellSize, _cellSize),
            DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never,
            DefaultComputeHeuristic = AStarGrid2D.Heuristic.Manhattan
        };

        _grid.Update();

        SetWalkableCells();
    }

    private void SetWalkableCells()
    {
        foreach (var cell in _tileMap.GetUsedCells())
        {
            bool isWalkable = (bool) _tileMap.GetCellTileData(cell).GetCustomData("is_walkable");

            _grid.SetPointSolid(cell, !isWalkable);
        }
    }

    /**
        Initialization: Ensure astarGrid.Update() has been called before querying, especially after setting solid points.
        Range Type: The example uses Manhattan Distance (Abs(x) + Abs(y)). Change the condition to Mathf.Sqrt(Mathf.Pow(x-center.X, 2) + Mathf.Pow(y-center.Y, 2)) <= range for Euclidean/Circular range.
        Performance: For large ranges, this is faster than calculating paths to every single cell.
        Diagonal Movement: If astarGrid.DiagonalMode is set to Never, you may need to adjust the logic to ensure connectivity.
    */

    public List<Vector2I> GetMovableCellsInRange(Vector2 center, int range)
    {
        List<Vector2I> reachableCells = new List<Vector2I>();

        Rect2I region = _grid.Region;

        for (int x = LocalToMap(center).X - range; x <= LocalToMap(center).X + range; x++)
        {
            for (int y = LocalToMap(center).Y - range; y <= LocalToMap(center).Y + range; y++)
            {
                Vector2I cell = new Vector2I(x, y);

                // 1. Check if inside grid boundaries
                if (region.HasPoint(cell))
                {
                     // 2. Check if cell is walkable (not solid) [2]
                    if (!_grid.IsPointSolid(cell))
                    {
                        // 3. Optional: Calculate distance for circular/Manhattan range
                        if (Mathf.Abs(x - LocalToMap(center).X) + Mathf.Abs(y - LocalToMap(center).Y) <= range)
                        {
                            GD.Print(cell);
                            reachableCells.Add(cell);
                        }
                    }
                }
            }
        }

        return reachableCells;
    }

    public bool IsCellOccupied(Vector2 cellPosition)
    {
        return _grid.IsPointSolid(LocalToMap(cellPosition));
    }

    public bool IsCellInRange(Vector2 currentPos, Vector2 targetPos, int range)
    {
        return GetMovableCellsInRange(currentPos, range).Contains(LocalToMap(targetPos));
    }

    public bool IsYourCell(Vector2 currentPos, Vector2 targetPos)
    {
        return LocalToMap(currentPos).Equals(LocalToMap(targetPos));
    }

    public void SetCellAsOccupied(Vector2 cellPosition)
    {
        _grid.SetPointSolid(LocalToMap(cellPosition), true);
    }

    public void SetCellAsFree(Vector2 cellPosition)
    {
        _grid.SetPointSolid(LocalToMap(cellPosition), false);
    }

    private static readonly Vector2I[] Directions4 =
    {
        Vector2I.Up,
        Vector2I.Down,
        Vector2I.Left,
        Vector2I.Right
    };

    public Vector2[] GetPathBetweenPoints(Vector2I current, Vector2I target) => _grid.GetPointPath(current, target, true);
    
    public Vector2I LocalToMap(Vector2 position) => _tileMap.LocalToMap(position);
    
    public Vector2 MapToLocal(Vector2I position) => _tileMap.MapToLocal(position);

    public Vector2 GetMousePosition() => MapToLocal(LocalToMap(_tileMap.GetGlobalMousePosition()));

}
