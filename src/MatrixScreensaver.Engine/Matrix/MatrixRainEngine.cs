using MatrixScreensaver.Engine.Configuration;

namespace MatrixScreensaver.Engine.Matrix;

public sealed class MatrixRainEngine
{
    private readonly Random _random = new();
    private readonly List<GlyphColumn> _columns = [];
    private int _lastWidth;
    private int _lastHeight;
    private int _lastFontSize;
    private double _lastDensity;
    private string _lastAlphabet = string.Empty;
    private int _lastSpawnMultiplier = 4;

    public IReadOnlyList<GlyphColumn> Columns => _columns;

    public void EnsureLayout(int width, int height, MatrixOptions options)
    {
        if (width <= 0 || height <= 0) return;
        if (_lastWidth == width && _lastHeight == height && _lastFontSize == options.FontSize &&
            Math.Abs(_lastDensity - options.Density) < 0.001 && _lastAlphabet == options.Alphabet && _lastSpawnMultiplier == Math.Clamp(options.SpawnMultiplier, 1, 96)) return;

        _columns.Clear();
        var alphabet = AlphabetProvider.GetAlphabet(options.Alphabet);
        var step = Math.Max(8, options.FontSize);
        var spawnMultiplier = Math.Clamp(options.SpawnMultiplier, 1, 96);
        var density = Math.Clamp(options.Density, 0.05, 1.0);
        var effectiveDensity = Math.Clamp(Math.Pow(density, 1.7), 0.02, 1.0);
        var spawnStep = Math.Max(1, (int)Math.Round(step / spawnMultiplier / Math.Max(0.35, effectiveDensity)));

        for (var x = 0; x < width; x += spawnStep)
        {
            if (_random.NextDouble() <= effectiveDensity)
                _columns.Add(new GlyphColumn(x, height, options.Speed, alphabet, _random));
        }

        _lastWidth = width;
        _lastHeight = height;
        _lastFontSize = options.FontSize;
        _lastDensity = options.Density;
        _lastAlphabet = options.Alphabet;
        _lastSpawnMultiplier = spawnMultiplier;
    }

    public void Update(double deltaSeconds, int height, MatrixOptions options)
    {
        foreach (var column in _columns)
            column.Update(deltaSeconds, height, options.Speed);
    }
}




