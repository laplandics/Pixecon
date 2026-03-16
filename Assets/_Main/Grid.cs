using UnityEngine;

public class Grid
{
    private const float CELL_SIZE = 1f;
    
    public Vector2 GridOrigin {get; }
    public int RowsCount { get; }
    public int ColumnsCount { get; }
    public Vector2Int[] GridPositions { get; }
    
    public Grid()
    {
        var camera = Core.Cam.Get;
        var cameraSIze = camera.orthographicSize;
        var height = cameraSIze * 2;
        var width = height * camera.aspect;
        ColumnsCount = Mathf.FloorToInt(width / CELL_SIZE);
        RowsCount = Mathf.FloorToInt(height / CELL_SIZE) * 2;
        
        var gridWorldWidth = ColumnsCount * CELL_SIZE;
        var gridWorldHeight = RowsCount * CELL_SIZE;
        
        var offsetX = (width - gridWorldWidth) / 2f;
        var offsetY = (height - gridWorldHeight) / 2f;
        
        GridOrigin = new Vector2( -width / 2 + offsetX, -height / 2 + offsetY);
        GridOrigin = new Vector2(GridOrigin.x, GridOrigin.y / 2);
        
        GridPositions = new Vector2Int[ColumnsCount * RowsCount];
        for (var x = 0; x < ColumnsCount; x++) { for (var y = 0; y < RowsCount; y++)
            { GridPositions[y * ColumnsCount + x] = new Vector2Int(x, y); } }
    }

    public Vector2Int WorldToGrid(Vector2 worldPos)
    {
        var x = Mathf.FloorToInt((worldPos.x - GridOrigin.x) / CELL_SIZE);
        var y = Mathf.FloorToInt((worldPos.y - GridOrigin.y) / CELL_SIZE);
        return new Vector2Int(x, y);
    }
    
    public Vector2 GridToWorld(Vector2Int cell)
    {
        var x = GridOrigin.x + cell.x * CELL_SIZE + CELL_SIZE * 0.5f;
        var y = GridOrigin.y + cell.y * CELL_SIZE + CELL_SIZE * 0.5f;
        return new Vector2(x, y);
    }
}