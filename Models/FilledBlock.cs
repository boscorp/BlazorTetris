namespace BlazorTetris.Models;

public class FilledBlock
{
    public Position2D Position { get; set; }
    public string Color { get; set; }

    public FilledBlock(Position2D position, string color)
    {
        Position = position;
        Color = color;
    }
}