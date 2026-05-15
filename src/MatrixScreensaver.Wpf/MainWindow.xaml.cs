using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MatrixScreensaver.Engine.Configuration;

namespace MatrixScreensaver.Wpf;

public partial class MainWindow : Window
{
    private readonly MatrixScreensaverOptions _options;
    private readonly string _configPath;
    private readonly DispatcherTimer _slideshowTimer = new();
    private int _imageIndex;
    private System.Windows.Point? _initialMousePosition;
    private readonly List<MatrixRainControl> _rainLayers = new();

    public MainWindow(MatrixScreensaverOptions options, string configPath)
    {
        _options = options;
        _configPath = configPath;
        InitializeComponent();

        Cursor = options.Screens.HideCursor ? System.Windows.Input.Cursors.None : System.Windows.Input.Cursors.Arrow;
        ScanLines.Visibility = options.Effects.ScanLines ? Visibility.Visible : Visibility.Collapsed;
        ScanLines.Opacity = options.Effects.ScanLineOpacity;
        Vignette.Visibility = options.Effects.Vignette ? Visibility.Visible : Visibility.Collapsed;
        CrtOverlay.Visibility = options.Effects.Crt ? Visibility.Visible : Visibility.Collapsed;
        CrtOverlay.Opacity = options.Effects.Crt ? 0.10 : 0.0;

        BackgroundImage.Opacity = options.Background.Opacity;
        BackgroundImage.Stretch = Stretch.Uniform;
        BackgroundStage.Opacity = Math.Clamp(options.Background.Opacity + 0.05, 0.0, 1.0);
        BackgroundStage.RenderTransform = Transform.Identity;

        UpdateBackgroundPlaneLayout();
        SizeChanged += (_, _) => UpdateBackgroundPlaneLayout();

        InitializeDepthLayers();

        Loaded += (_, _) =>
        {
            LoadBackground();
            if (options.Background.Mode.Equals("Slideshow", StringComparison.OrdinalIgnoreCase) && options.Background.Images.Count > 1)
            {
                _slideshowTimer.Interval = TimeSpan.FromSeconds(Math.Max(5, options.Background.ChangeIntervalSeconds));
                _slideshowTimer.Tick += (_, _) => NextBackground();
                _slideshowTimer.Start();
            }
        };
    }

    private void InitializeDepthLayers()
    {
        _rainLayers.Clear();
        BackLayersContainer.Children.Clear();
        FrontLayersContainer.Children.Clear();

        var configuredLayers = _options.Depth?.Layers ?? new List<DepthLayerOptions>();
        if (configuredLayers.Count == 0)
        {
            configuredLayers =
            [
                new() { Depth = 0.0, Opacity = 0.6, SpeedMultiplier = 0.4, DensityMultiplier = 0.6 },
                new() { Depth = 0.5, Opacity = 0.8, SpeedMultiplier = 0.8, DensityMultiplier = 0.9 },
                new() { Depth = 1.0, Opacity = 1.0, SpeedMultiplier = 1.2, DensityMultiplier = 1.0 }
            ];
        }

        var backLayers = configuredLayers.Where(l => l.Depth < 0.5).OrderBy(l => l.Depth).ToList();
        var frontLayers = configuredLayers.Where(l => l.Depth >= 0.5).OrderBy(l => l.Depth).ToList();

        const int targetBackLayers = 2;
        const int targetFrontLayers = 2;

        while (backLayers.Count < targetBackLayers)
        {
            var source = backLayers.LastOrDefault() ?? frontLayers.FirstOrDefault() ?? new DepthLayerOptions
            {
                Depth = 0.25,
                Opacity = 0.70,
                SpeedMultiplier = 0.90,
                DensityMultiplier = 1.00
            };
            var targetDepth = 0.16 + backLayers.Count * 0.18;
            backLayers.Add(CreateSyntheticLayer(source, targetDepth, 0.95, 1.12, 1.30));
        }

        while (frontLayers.Count < targetFrontLayers)
        {
            var source = frontLayers.LastOrDefault() ?? backLayers.LastOrDefault() ?? new DepthLayerOptions
            {
                Depth = 0.75,
                Opacity = 0.90,
                SpeedMultiplier = 1.10,
                DensityMultiplier = 0.95
            };
            var targetDepth = 0.66 + frontLayers.Count * 0.18;
            frontLayers.Add(CreateSyntheticLayer(source, targetDepth, 1.03, 1.08, 0.98));
        }

        var selectedBackLayers = backLayers.OrderBy(l => l.Depth).Take(targetBackLayers);
        var selectedFrontLayers = frontLayers.OrderBy(l => l.Depth).Take(targetFrontLayers);

        foreach (var layer in selectedBackLayers)
        {
            AddRainLayer(layer, MatrixDepth.Far, BackLayersContainer);
        }

        foreach (var layer in selectedFrontLayers)
        {
            AddRainLayer(layer, MatrixDepth.Near, FrontLayersContainer);
        }
    }

    private static DepthLayerOptions CreateSyntheticLayer(
        DepthLayerOptions source,
        double depth,
        double opacityScale,
        double speedScale,
        double densityScale)
    {
        return new DepthLayerOptions
        {
            Depth = depth,
            Opacity = Math.Clamp(source.Opacity * opacityScale, 0.2, 1.0),
            SpeedMultiplier = Math.Clamp(source.SpeedMultiplier * speedScale, 0.1, 3.0),
            DensityMultiplier = Math.Clamp(source.DensityMultiplier * densityScale, 0.1, 3.0)
        };
    }

    private void AddRainLayer(DepthLayerOptions layerConfig, MatrixDepth depth, System.Windows.Controls.Panel targetContainer)
    {
        var rainControl = new MatrixRainControl();
        _rainLayers.Add(rainControl);

        var screenWidth = Width > 0 ? Width : ActualWidth;
        var screenHeight = Height > 0 ? Height : ActualHeight;
        var isLandscape = screenWidth >= screenHeight && screenHeight > 0;
        var landscapeRatio = screenHeight > 0 ? screenWidth / screenHeight : 1.0;
        var isWideLandscape = isLandscape && landscapeRatio >= 1.30;
        var isBackLayer = depth == MatrixDepth.Far;

        var backOpacityBoost = isWideLandscape ? 1.40 : (isLandscape ? 1.28 : 1.12);
        var backSpeedBoost = isWideLandscape ? 1.52 : (isLandscape ? 1.36 : 1.14);
        var backDensityBoost = isWideLandscape ? 1.75 : (isLandscape ? 1.50 : 1.24);

        var frontOpacityBoost = isWideLandscape ? 0.72 : (isLandscape ? 0.78 : 0.88);
        var frontSpeedBoost = isWideLandscape ? 0.92 : (isLandscape ? 0.96 : 1.00);
        var frontDensityBoost = isWideLandscape ? 0.45 : (isLandscape ? 0.52 : 0.75);

        var effectiveOpacity = isBackLayer
            ? Math.Clamp(layerConfig.Opacity * backOpacityBoost, 0.62, 1.00)
            : Math.Clamp(layerConfig.Opacity * frontOpacityBoost, 0.12, 0.55);

        var effectiveSpeedMultiplier = isBackLayer
            ? Math.Clamp(layerConfig.SpeedMultiplier * backSpeedBoost, 0.20, 3.00)
            : Math.Clamp(layerConfig.SpeedMultiplier * frontSpeedBoost, 0.20, 2.20);

        var effectiveDensityMultiplier = isBackLayer
            ? Math.Clamp(layerConfig.DensityMultiplier * backDensityBoost, 0.20, 3.00)
            : Math.Clamp(layerConfig.DensityMultiplier * frontDensityBoost, 0.20, 1.10);

        var effectiveFadeOpacity = isBackLayer
            ? Math.Clamp(_options.Matrix.FadeOpacity * effectiveOpacity, 0.03, 0.24)
            : 0.0;

        var modifiedOptions = new MatrixScreensaverOptions
        {
            Background = _options.Background,
            Matrix = new MatrixOptions
            {
                GlyphColor = _options.Matrix.GlyphColor,
                HeadColor = _options.Matrix.HeadColor,
                ShadowColor = _options.Matrix.ShadowColor,
                FontSize = _options.Matrix.FontSize,
                Speed = Math.Clamp(_options.Matrix.Speed * effectiveSpeedMultiplier, 0.30, 3.50),
                Density = Math.Clamp(_options.Matrix.Density * effectiveDensityMultiplier, 0.05, 1.50),
                SpawnMultiplier = _options.Matrix.SpawnMultiplier,
                FadeOpacity = effectiveFadeOpacity,
                FontFamily = _options.Matrix.FontFamily,
                Alphabet = _options.Matrix.Alphabet
            },
            Effects = _options.Effects,
            Screens = _options.Screens
        };

        rainControl.Configure(modifiedOptions, depth);
        rainControl.Opacity = effectiveOpacity;
        rainControl.Visibility = Visibility.Visible;
        targetContainer.Children.Add(rainControl);
    }
    private void NextBackground()
    {
        if (_options.Background.Images.Count == 0) return;
        _imageIndex = (_imageIndex + 1) % _options.Background.Images.Count;
        LoadBackground();
    }

    private void LoadBackground()
    {
        var image = _options.Background.Images.ElementAtOrDefault(_imageIndex);
        if (string.IsNullOrWhiteSpace(image)) return;

        var fullPath = ResolveBackgroundPath(image);
        if (string.IsNullOrWhiteSpace(fullPath) || !File.Exists(fullPath)) return;

        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
        bitmap.EndInit();
        bitmap.Freeze();

        BackgroundImage.Source = bitmap;
        UpdateBackgroundPlaneLayout(bitmap);
    }

    private void UpdateBackgroundPlaneLayout(BitmapSource? bitmap = null)
    {
        bitmap ??= BackgroundImage.Source as BitmapSource;

        var availableWidth = Math.Max(1.0, ActualWidth - 240.0);
        var availableHeight = Math.Max(1.0, ActualHeight - 240.0);

        if (bitmap is null || bitmap.PixelWidth <= 0 || bitmap.PixelHeight <= 0)
        {
            BackgroundStage.Width = availableWidth;
            BackgroundStage.Height = availableHeight;
            return;
        }

        var imageRatio = bitmap.PixelWidth / (double)bitmap.PixelHeight;
        var screenRatio = availableWidth / availableHeight;

        double width, height;
        if (screenRatio > imageRatio)
        {
            height = availableHeight;
            width = height * imageRatio;
        }
        else
        {
            width = availableWidth;
            height = width / imageRatio;
        }

        BackgroundStage.Width = Math.Max(1.0, width);
        BackgroundStage.Height = Math.Max(1.0, height);
    }

    private string? ResolveBackgroundPath(string image)
    {
        if (string.IsNullOrWhiteSpace(image))
            return null;

        if (Path.IsPathRooted(image))
            return File.Exists(image) ? Path.GetFullPath(image) : null;

        var configDir = Path.GetDirectoryName(_configPath) ?? AppContext.BaseDirectory;
        var candidate = Path.GetFullPath(Path.Combine(configDir, image));
        if (File.Exists(candidate))
            return candidate;

        var normalized = image.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        var trimmed = normalized;
        while (trimmed.StartsWith($"..{Path.DirectorySeparatorChar}") || trimmed.StartsWith($".{Path.DirectorySeparatorChar}"))
        {
            var index = trimmed.IndexOf(Path.DirectorySeparatorChar);
            if (index < 0 || index + 1 >= trimmed.Length)
            {
                trimmed = string.Empty;
                break;
            }

            trimmed = trimmed[(index + 1)..];
        }

        var fileName = Path.GetFileName(normalized);
        var roots = new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory(), configDir };

        foreach (var root in roots)
        {
            var dir = new DirectoryInfo(root);
            for (var i = 0; i < 10 && dir is not null; i++)
            {
                var c1 = Path.GetFullPath(Path.Combine(dir.FullName, normalized));
                if (File.Exists(c1)) return c1;

                if (!string.IsNullOrWhiteSpace(trimmed))
                {
                    var c2 = Path.GetFullPath(Path.Combine(dir.FullName, trimmed));
                    if (File.Exists(c2)) return c2;
                }

                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    var c3 = Path.Combine(dir.FullName, "Assets", "backgrounds", fileName);
                    if (File.Exists(c3)) return c3;
                }

                dir = dir.Parent;
            }
        }

        return null;
    }

    private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        => System.Windows.Application.Current.Shutdown();

    private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (!_options.Screens.ExitOnMouseMove) return;
        var pos = e.GetPosition(this);
        _initialMousePosition ??= pos;
        if (Math.Abs(pos.X - _initialMousePosition.Value.X) > 8 || Math.Abs(pos.Y - _initialMousePosition.Value.Y) > 8)
            System.Windows.Application.Current.Shutdown();
    }
}











