namespace BlazorTetris.Models;

public class GameBoard
{
    public const int Width = 10;
    public const int Height = 20;

    private string?[,] _grid;

    public GameBoard()
    {
        _grid = new string?[Width, Height];
    }

    public bool IsValidPosition(Tetromino tetromino)
    {
        var blocks = tetromino.GetAbsoluteBlocks();
        return blocks.All(block => 
            block.X >= 0 && block.X < Width &&
            block.Y >= 0 && block.Y < Height &&
            _grid[block.X, block.Y] == null);
    }

    public void PlaceTetromino(Tetromino tetromino)
    {
        var blocks = tetromino.GetAbsoluteBlocks();
        foreach (var block in blocks)
        {
            if (block.X >= 0 && block.X < Width &&
                block.Y >= 0 && block.Y < Height)
            {
                _grid[block.X, block.Y] = tetromino.Color;
            }
        }
    }

    public List<int> GetCompletedLines()
    {
        var completedLines = new List<int>();
        
        for (int y = 0; y < Height; y++)
        {
            bool isComplete = true;
            for (int x = 0; x < Width && isComplete; x++)
            {
                if (_grid[x, y] == null)
                    isComplete = false;
            }
            
            if (isComplete)
                completedLines.Add(y);
        }
        
        return completedLines;
    }

    public void ClearLines(List<int> lines)
    {
        
        if (lines.Count == 0) return;
        
        // Create a new grid without the completed lines
        var newGrid = new string?[Width, Height];
        int targetY = 0; // Position in new grid (start from bottom)
        
        for (int sourceY = 0; sourceY < Height; sourceY++)
        {
            // If this line is not in the list of lines to clear, copy it
            if (!lines.Contains(sourceY))
            {
                for (int x = 0; x < Width; x++)
                {
                    newGrid[x, targetY] = _grid[x, sourceY];
                }
                targetY++;
            }
        }
        
        // The remaining top lines will stay null (empty)
        _grid = newGrid;
    }

    public string?[,] GetGrid() => (string?[,])_grid.Clone();

    public List<FilledBlock> GetFilledBlocks()
    {
        var blocks = new List<FilledBlock>();
        
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (_grid[x, y] != null)
                {
                    blocks.Add(new FilledBlock(new Position2D(x, y), _grid[x, y]!));
                }
            }
        }
        
        return blocks;
    }
}