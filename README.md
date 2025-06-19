# Blazor Tetris

A classic Tetris game implementation built with Blazor Server and C#.

## Features

- Classic Tetris gameplay with all standard tetrominoes (I, O, T, S, Z, J, L)
- Real-time game mechanics including piece rotation, line clearing, and gravity
- Score tracking and level progression
- Local storage for high scores persistence
- Responsive web interface
- Keyboard controls for seamless gameplay

## Technologies Used

- **Blazor Server** - Interactive web UI framework
- **C#** - Game logic and server-side processing
- **JavaScript** - Client-side interactions and animations
- **CSS** - Styling and visual effects
- **HTML5** - Modern web standards

## Game Controls

- **Arrow Left/Right** - Move piece horizontally
- **Arrow Down** - Soft drop (faster piece descent)
- **Arrow Up**, **A**, **D**, or **R** - Rotate piece
- **Space** - Hard drop (instant piece placement)
- **P** - Pause/Resume game

## Project Structure

- `Models/` - Game entities (Tetromino, GameBoard, GameState, etc.)
- `Services/` - Business logic (GameService, LocalStorageService)
- `Pages/` - Blazor pages and components
- `wwwroot/` - Static assets (CSS, JavaScript)

## Getting Started

### Prerequisites

- .NET 9.0 or later
- Any modern web browser

### Running the Game

1. Clone the repository
2. Navigate to the project directory
3. Run the following commands:

```bash
dotnet restore
dotnet run
```

4. Open your browser and navigate to the displayed URL (typically `https://localhost:5001`)

## Game Mechanics

- **Line Clearing**: Complete horizontal lines are automatically cleared
- **Scoring**: Points awarded for line clears, with bonuses for multiple lines
- **Level Progression**: Game speed increases as you clear more lines
- **Game Over**: Occurs when pieces reach the top of the playing field

## Contributing

Feel free to submit issues and pull requests to improve the game!

## License

This project is open source and available under the MIT License.