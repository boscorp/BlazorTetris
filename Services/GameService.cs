using BlazorTetris.Models;

namespace BlazorTetris.Services;

public class GameService
{
    private readonly Random _random = new();
    private readonly LocalStorageService _localStorage;
    private GameBoard _board = new();
    private Tetromino? _currentPiece;
    private Tetromino? _nextPiece;
    private Timer? _gameTimer;
    
    public GameState State { get; private set; } = GameState.Menu;
    public int Score { get; private set; }
    public int Level { get; private set; } = 1;
    public int LinesCleared { get; private set; }
    public int HighScore { get; private set; }
    public List<int> TopScores { get; private set; } = new();
    public TimeSpan DropInterval => TimeSpan.FromMilliseconds(Math.Max(50, 1000 - (Level - 1) * 50));

    public GameService(LocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public event Action? GameStateChanged;
    public event Action? BoardUpdated;

    public void StartGame()
    {
        _board = new GameBoard();
        _currentPiece = GenerateRandomTetromino();
        _nextPiece = GenerateRandomTetromino();
        Score = 0;
        Level = 1;
        LinesCleared = 0;
        State = GameState.Playing;
        
        StartGameTimer();
        GameStateChanged?.Invoke();
        BoardUpdated?.Invoke();
    }

    public void PauseGame()
    {
        if (State == GameState.Playing)
        {
            State = GameState.Paused;
            _gameTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            GameStateChanged?.Invoke();
        }
        else if (State == GameState.Paused)
        {
            State = GameState.Playing;
            StartGameTimer();
            GameStateChanged?.Invoke();
        }
    }

    public void RestartGame()
    {
        _gameTimer?.Dispose();
        StartGame();
    }

    public async Task EndGameAsync()
    {
        State = GameState.GameOver;
        _gameTimer?.Dispose();
        
        // Save score to localStorage
        await _localStorage.AddScoreAsync(Score);
        await LoadScoresAsync();
        
        GameStateChanged?.Invoke();
    }

    public async Task LoadScoresAsync()
    {
        HighScore = await _localStorage.GetHighScoreAsync();
        TopScores = await _localStorage.GetTopScoresAsync();
    }

    public bool MovePiece(int deltaX, int deltaY)
    {
        if (_currentPiece == null || State != GameState.Playing) return false;

        var newPiece = _currentPiece.Clone();
        newPiece.Position = newPiece.Position.Add(new Position2D(deltaX, deltaY));

        if (_board.IsValidPosition(newPiece))
        {
            _currentPiece = newPiece;
            BoardUpdated?.Invoke();
            return true;
        }

        return false;
    }

    public bool RotatePiece()
    {
        if (_currentPiece == null || State != GameState.Playing) return false;

        var newPiece = _currentPiece.Clone();
        newPiece.Rotation = (newPiece.Rotation + 1) % 4;

        if (_board.IsValidPosition(newPiece))
        {
            _currentPiece = newPiece;
            BoardUpdated?.Invoke();
            return true;
        }

        return false;
    }

    public void DropPiece()
    {
        if (_currentPiece == null || State != GameState.Playing) return;

        while (MovePiece(0, -1)) { }
        
        // Colocar la pieza inmediatamente
        _board.PlaceTetromino(_currentPiece);
        CheckForCompletedLayers();
        SpawnNextPiece();
        BoardUpdated?.Invoke();
    }

    private void StartGameTimer()
    {
        _gameTimer?.Dispose();
        _gameTimer = new Timer(GameTick, null, DropInterval, DropInterval);
    }

    private void GameTick(object? state)
    {
        if (State != GameState.Playing || _currentPiece == null) return;

        if (!MovePiece(0, -1))
        {
            // La pieza no puede moverse más abajo
            _board.PlaceTetromino(_currentPiece);
            CheckForCompletedLayers();
            SpawnNextPiece();
            BoardUpdated?.Invoke();
        }
    }

    private void CheckForCompletedLayers()
    {
        var completedLines = _board.GetCompletedLines();
        if (completedLines.Any())
        {
            _board.ClearLines(completedLines);
            var linesCount = completedLines.Count;
            LinesCleared += linesCount;
            
            // Scoring: 100, 300, 500, 800 points for 1, 2, 3, 4 lines
            var points = linesCount switch
            {
                1 => 100,
                2 => 300,
                3 => 500,
                4 => 800,
                _ => linesCount * 100
            };
            
            Score += points * Level;
            Level = (LinesCleared / 10) + 1;
            
            // Notify UI that score and lines have changed
            GameStateChanged?.Invoke();
            
            // Actualizar velocidad del timer
            if (State == GameState.Playing)
            {
                StartGameTimer();
            }
        }
    }

    private void SpawnNextPiece()
    {
        _currentPiece = _nextPiece;
        _nextPiece = GenerateRandomTetromino();

        if (_currentPiece != null && !_board.IsValidPosition(_currentPiece))
        {
            _ = Task.Run(async () => await EndGameAsync());
            return;
        }

        BoardUpdated?.Invoke();
    }

    private Tetromino GenerateRandomTetromino()
    {
        var types = Enum.GetValues<TetrominoType>();
        var randomType = types[_random.Next(types.Length)];
        var startPosition = new Position2D(GameBoard.Width / 2 - 1, GameBoard.Height - 4);
        return new Tetromino(randomType, startPosition);
    }

    public GameBoard GetBoard() => _board;
    public Tetromino? GetCurrentPiece() => _currentPiece;
    public Tetromino? GetNextPiece() => _nextPiece;

    public void Dispose()
    {
        _gameTimer?.Dispose();
    }
}