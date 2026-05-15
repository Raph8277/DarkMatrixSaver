using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using MatrixScreensaver.Engine.Configuration;

namespace MatrixScreensaver.Wpf;

public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var args = e.Args.Select(a => a.Trim()).ToArray();
        var mode = args.FirstOrDefault(a => a.Equals("/s", StringComparison.OrdinalIgnoreCase)
                                        || a.Equals("/c", StringComparison.OrdinalIgnoreCase)
                                        || a.StartsWith("/p", StringComparison.OrdinalIgnoreCase));

        var configPath = GetArgumentValue(args, "--config") ?? ResolveDefaultConfigPath();
        var options = MatrixConfigurationLoader.Load(configPath);

        if (mode?.Equals("/c", StringComparison.OrdinalIgnoreCase) == true)
        {
            System.Windows.MessageBox.Show("Lance MatrixScreensaver.ConfigEditor pour modifier le JSON.", "Matrix Screensaver");
            Shutdown();
            return;
        }

        if (options.Screens.MultiScreen)
        {
            var screens = Screen.AllScreens;
            var screenCount = screens.Length;

            foreach (var screen in screens)
            {
                var perScreenOptions = CreateOptionsForScreen(options, screenCount, screen, screen.Primary);
                ShowScreen(screen, perScreenOptions, configPath);
            }
        }
        else
        {
            ShowScreen(Screen.PrimaryScreen!, options, configPath);
        }
    }

    private static string? GetArgumentValue(string[] args, string name)
    {
        for (var i = 0; i < args.Length - 1; i++)
            if (args[i].Equals(name, StringComparison.OrdinalIgnoreCase))
                return args[i + 1];
        return null;
    }

    private static string GetUserConfigPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var folder = Path.Combine(appData, "DarkMatrixSaver");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, "matrix.custom.json");
    }

    private static string? ResolvePresetPath(string fileName)
    {
        var candidates = new[]
        {
            AppContext.BaseDirectory,
            Directory.GetCurrentDirectory()
        };

        foreach (var start in candidates)
        {
            var dir = new DirectoryInfo(start);
            for (var i = 0; i < 10 && dir is not null; i++)
            {
                var candidate = Path.Combine(dir.FullName, "presets", fileName);
                if (File.Exists(candidate))
                    return candidate;

                dir = dir.Parent;
            }
        }

        return null;
    }

    private static string ResolveDefaultConfigPath()
    {
        var userConfig = GetUserConfigPath();
        if (File.Exists(userConfig))
            return userConfig;

        var demonPreset = ResolvePresetPath("demonbar.hell.matrix.json");
        if (demonPreset is not null)
            return demonPreset;

        var fallbackPreset = ResolvePresetPath("matrix.default.json");
        return fallbackPreset ?? userConfig;
    }

    private static MatrixScreensaverOptions CreateOptionsForScreen(MatrixScreensaverOptions baseOptions, int screenCount, Screen screen, bool isPrimary)
    {
        var options = CloneOptions(baseOptions);
        var baseDensity = baseOptions.Matrix.Density;

        var scale = screenCount switch
        {
            <= 1 => 1.0,
            2 => 0.95,
            3 => 0.85,
            _ => 0.75
        };

        if (isPrimary)
        {
            scale = Math.Min(1.0, scale + 0.25);
        }

        options.Matrix.Density = Math.Clamp(baseDensity * scale, 0.2, 1.0);

        if (!isPrimary && screenCount >= 3)
        {
            var isPortrait = screen.Bounds.Height > screen.Bounds.Width;
            var secondaryDensityFactor = isPortrait ? 0.60 : 0.78;
            var secondaryFontBoost = isPortrait ? 2 : 0;
            options.Matrix.Density = Math.Clamp(options.Matrix.Density * secondaryDensityFactor, 0.25, 0.95);
            options.Matrix.FontSize = Math.Min(44, options.Matrix.FontSize + secondaryFontBoost);
            options.Matrix.Speed = Math.Clamp(options.Matrix.Speed * 1.00, 0.7, 2.2);

            // Keep image/effects consistent across screens to avoid "image in back" perception on landscape panels.
            options.Background.Opacity = baseOptions.Background.Opacity;
            options.Effects.Glow = baseOptions.Effects.Glow;
            options.Effects.ScanLines = baseOptions.Effects.ScanLines;
            options.Effects.Vignette = baseOptions.Effects.Vignette;
            options.Effects.Crt = baseOptions.Effects.Crt;
        }

        return options;
    }

    private static MatrixScreensaverOptions CloneOptions(MatrixScreensaverOptions source)
    {
        return new MatrixScreensaverOptions
        {
            Background = new BackgroundOptions
            {
                Mode = source.Background.Mode,
                Images = [.. source.Background.Images],
                ChangeIntervalSeconds = source.Background.ChangeIntervalSeconds,
                Opacity = source.Background.Opacity,
                Stretch = source.Background.Stretch
            },
            Matrix = new MatrixOptions
            {
                GlyphColor = source.Matrix.GlyphColor,
                HeadColor = source.Matrix.HeadColor,
                ShadowColor = source.Matrix.ShadowColor,
                FontSize = source.Matrix.FontSize,
                Speed = source.Matrix.Speed,
                Density = source.Matrix.Density,
                SpawnMultiplier = source.Matrix.SpawnMultiplier,
                FadeOpacity = source.Matrix.FadeOpacity,
                FontFamily = source.Matrix.FontFamily,
                Alphabet = source.Matrix.Alphabet
            },
            Effects = new EffectsOptions
            {
                Glow = source.Effects.Glow,
                GlowIntensity = source.Effects.GlowIntensity,
                ScanLines = source.Effects.ScanLines,
                ScanLineOpacity = source.Effects.ScanLineOpacity,
                Crt = source.Effects.Crt,
                Vignette = source.Effects.Vignette,
                Smoke = source.Effects.Smoke,
                Particles = source.Effects.Particles
            },
            Depth = new DepthOptions
            {
                Layers = source.Depth?.Layers?.Select(l => new DepthLayerOptions
                {
                    Depth = l.Depth,
                    Opacity = l.Opacity,
                    SpeedMultiplier = l.SpeedMultiplier,
                    DensityMultiplier = l.DensityMultiplier
                }).ToList() ?? []
            },
            Screens = new ScreenOptions
            {
                MultiScreen = source.Screens.MultiScreen,
                ExitOnMouseMove = source.Screens.ExitOnMouseMove,
                HideCursor = source.Screens.HideCursor
            }
        };
    }

    private static void ShowScreen(Screen screen, MatrixScreensaverOptions options, string configPath)
    {
        var window = new MainWindow(options, configPath)
        {
            Left = screen.Bounds.Left,
            Top = screen.Bounds.Top,
            Width = screen.Bounds.Width,
            Height = screen.Bounds.Height,
            WindowStyle = WindowStyle.None,
            ResizeMode = ResizeMode.NoResize,
            Topmost = true,
            WindowState = WindowState.Normal
        };
        window.Show();
    }
}

