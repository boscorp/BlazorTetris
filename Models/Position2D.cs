namespace BlazorTetris.Models;

public record Position2D(int X, int Y)
{
    public Position2D Add(Position2D other) => new(X + other.X, Y + other.Y);
    public Position2D Subtract(Position2D other) => new(X - other.X, Y - other.Y);
}