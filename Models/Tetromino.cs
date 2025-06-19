namespace BlazorTetris.Models;

public class Tetromino
{
    public TetrominoType Type { get; set; }
    public Position2D Position { get; set; }
    public int Rotation { get; set; }
    public string Color { get; set; }
    public List<Position2D> Blocks { get; set; }

    public Tetromino(TetrominoType type, Position2D position)
    {
        Type = type;
        Position = position;
        Rotation = 0;
        Color = GetColorForType(type);
        Blocks = GetBlocksForType(type);
    }

    private static string GetColorForType(TetrominoType type) => type switch
    {
        TetrominoType.I => "#00FFFF", // Cyan
        TetrominoType.O => "#FFFF00", // Yellow
        TetrominoType.T => "#800080", // Purple
        TetrominoType.S => "#00FF00", // Green
        TetrominoType.Z => "#FF0000", // Red
        TetrominoType.J => "#0000FF", // Blue
        TetrominoType.L => "#FFA500", // Orange
        _ => "#FFFFFF"
    };

    private static List<Position2D> GetBlocksForType(TetrominoType type) => type switch
    {
        TetrominoType.I => [new(0,0), new(1,0), new(2,0), new(3,0)],
        TetrominoType.O => [new(0,0), new(1,0), new(0,1), new(1,1)],
        TetrominoType.T => [new(1,0), new(0,1), new(1,1), new(2,1)],
        TetrominoType.S => [new(1,0), new(2,0), new(0,1), new(1,1)],
        TetrominoType.Z => [new(0,0), new(1,0), new(1,1), new(2,1)],
        TetrominoType.J => [new(0,0), new(0,1), new(1,1), new(2,1)],
        TetrominoType.L => [new(2,0), new(0,1), new(1,1), new(2,1)],
        _ => []
    };

    public List<Position2D> GetAbsoluteBlocks()
    {
        return Blocks.Select(block => ApplyRotation(block, Rotation).Add(Position)).ToList();
    }

    private static Position2D ApplyRotation(Position2D position, int rotation)
    {
        var (x, y) = position;
        return (rotation % 4) switch
        {
            0 => new(x, y),
            1 => new(-y, x),
            2 => new(-x, -y),
            3 => new(y, -x),
            _ => position
        };
    }

    public Tetromino Clone()
    {
        return new Tetromino(Type, Position)
        {
            Rotation = Rotation,
            Blocks = new List<Position2D>(Blocks)
        };
    }
}