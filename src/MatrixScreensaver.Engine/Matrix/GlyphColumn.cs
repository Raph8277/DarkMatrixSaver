using System;
using System.Collections.Generic;

namespace MatrixScreensaver.Engine.Matrix;

public sealed class GlyphColumn
{
    private readonly Random _random;
    private readonly char[] _alphabet;
    private char[] _glyphs = Array.Empty<char>();
    private double _mutationTimer;

    public int X { get; }
    public double Y { get; private set; }
    public double Speed { get; private set; }
    public int Length { get; private set; }

    public GlyphColumn(int x, int height, double speedFactor, char[] alphabet, Random random)
    {
        X = x;
        _alphabet = alphabet;
        _random = random;
        Reset(height, speedFactor, startAboveScreen: true);
    }

    public void Update(double deltaSeconds, int height, double speedFactor)
    {
        Y += Speed * speedFactor * deltaSeconds;

        _mutationTimer += deltaSeconds;
        if (_mutationTimer >= 0.04)
        {
            _mutationTimer = 0;
            var mutations = Math.Min(4, Math.Max(1, Length / 10));
            for (var i = 0; i < mutations; i++)
            {
                var index = _random.Next(Length);
                _glyphs[index] = _alphabet[_random.Next(_alphabet.Length)];
            }
        }

        if (Y - Length * 28 > height)
            Reset(height, speedFactor, startAboveScreen: false);
    }

    public IEnumerable<(char glyph, int index)> Glyphs()
    {
        for (var i = 0; i < Length; i++)
            yield return (_glyphs[i], i);
    }

    private void Reset(int height, double speedFactor, bool startAboveScreen)
    {
        Length = _random.Next(8, 38);
        _glyphs = new char[Length];
        for (var i = 0; i < Length; i++)
            _glyphs[i] = _alphabet[_random.Next(_alphabet.Length)];

        _mutationTimer = 0;
        Speed = _random.Next(70, 260) * Math.Max(0.1, speedFactor);
        Y = startAboveScreen ? -_random.Next(0, Math.Max(1, height)) : -Length * 28;
    }
}
