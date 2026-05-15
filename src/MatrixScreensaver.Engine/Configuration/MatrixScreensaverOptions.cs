namespace MatrixScreensaver.Engine.Configuration;

public sealed class MatrixScreensaverOptions
{
    public BackgroundOptions Background { get; set; } = new();
    public MatrixOptions Matrix { get; set; } = new();
    public EffectsOptions Effects { get; set; } = new();
    public DepthOptions Depth { get; set; } = new();
    public ScreenOptions Screens { get; set; } = new();
}

public sealed class BackgroundOptions
{
    public string Mode { get; set; } = "Single"; // Single | Slideshow
    public List<string> Images { get; set; } = [];
    public int ChangeIntervalSeconds { get; set; } = 30;
    public double Opacity { get; set; } = 1.0;
    public string Stretch { get; set; } = "UniformToFill";
}

public sealed class MatrixOptions
{
    public string GlyphColor { get; set; } = "#00FF66";
    public string HeadColor { get; set; } = "#D8FFE8";
    public string ShadowColor { get; set; } = "#003B1B";
    public int FontSize { get; set; } = 22;
    public double Speed { get; set; } = 1.2;
    public double Density { get; set; } = 0.85;
    public int SpawnMultiplier { get; set; } = 4;
    public double FadeOpacity { get; set; } = 0.08;
    public string FontFamily { get; set; } = "Consolas";
    public string Alphabet { get; set; } = "Katakana"; // Katakana | Hex | Runes | Demon | Binary
}

public sealed class EffectsOptions
{
    public bool Glow { get; set; } = true;
    public double GlowIntensity { get; set; } = 0.8;
    public bool ScanLines { get; set; } = true;
    public double ScanLineOpacity { get; set; } = 0.18;
    public bool Crt { get; set; } = true;
    public bool Vignette { get; set; } = true;
    public bool Smoke { get; set; } = false;
    public bool Particles { get; set; } = false;
}

public sealed class DepthLayerOptions
{
    public double Depth { get; set; } = 0.5;
    public double Opacity { get; set; } = 1.0;
    public double SpeedMultiplier { get; set; } = 1.0;
    public double DensityMultiplier { get; set; } = 1.0;
}

public sealed class DepthOptions
{
    public List<DepthLayerOptions> Layers { get; set; } = new()
    {
        new() { Depth = 0.0, Opacity = 0.6, SpeedMultiplier = 0.4, DensityMultiplier = 0.6 },
        new() { Depth = 0.5, Opacity = 0.8, SpeedMultiplier = 0.8, DensityMultiplier = 0.9 },
        new() { Depth = 1.0, Opacity = 1.0, SpeedMultiplier = 1.2, DensityMultiplier = 1.0 }
    };
}

public sealed class ScreenOptions
{
    public bool MultiScreen { get; set; } = true;
    public bool ExitOnMouseMove { get; set; } = true;
    public bool HideCursor { get; set; } = true;
}
