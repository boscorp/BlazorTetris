using Microsoft.JSInterop;

namespace BlazorTetris.Services;

public class LocalStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<int> GetHighScoreAsync()
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "tetris_highscore");
            return int.TryParse(result, out var score) ? score : 0;
        }
        catch
        {
            return 0;
        }
    }

    public async Task SetHighScoreAsync(int score)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "tetris_highscore", score.ToString());
        }
        catch
        {
            // Ignore localStorage errors
        }
    }

    public async Task<List<int>> GetTopScoresAsync()
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "tetris_topscores");
            if (string.IsNullOrEmpty(result))
                return new List<int>();

            return result.Split(',')
                        .Select(s => int.TryParse(s, out var score) ? score : 0)
                        .Where(s => s > 0)
                        .OrderByDescending(s => s)
                        .Take(10)
                        .ToList();
        }
        catch
        {
            return new List<int>();
        }
    }

    public async Task AddScoreAsync(int score)
    {
        try
        {
            var scores = await GetTopScoresAsync();
            scores.Add(score);
            scores = scores.OrderByDescending(s => s).Take(10).ToList();
            
            var scoresString = string.Join(",", scores);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "tetris_topscores", scoresString);
            
            // Also update high score if necessary
            var currentHighScore = await GetHighScoreAsync();
            if (score > currentHighScore)
            {
                await SetHighScoreAsync(score);
            }
        }
        catch
        {
            // Ignore localStorage errors
        }
    }
}