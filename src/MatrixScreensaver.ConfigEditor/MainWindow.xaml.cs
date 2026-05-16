using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MatrixScreensaver.Engine.Configuration;
using Forms = System.Windows.Forms;

namespace MatrixScreensaver.ConfigEditor;

public partial class MainWindow : Window
{
    private string _path;
    private MatrixScreensaverOptions _options = new();
    private readonly JsonSerializerOptions _json = new() { WriteIndented = true };
    private bool _updatingForm;

    public MainWindow()
    {
        InitializeComponent();
        SubscribeToChanges();

        _path = GetUserConfigPath();
        UpdateActivePathText();

        if (File.Exists(_path))
        {
            JsonBox.Text = File.ReadAllText(_path);
            ApplyJsonToForm();
        }
        else
        {
            var defaultPreset = ResolvePresetPath("demonbar.hell.matrix.json") ?? ResolvePresetPath("matrix.default.json");
            if (defaultPreset is not null && File.Exists(defaultPreset))
            {
                JsonBox.Text = File.ReadAllText(defaultPreset);
                ApplyJsonToForm();
            }
            else
            {
                LoadOptionsToForm(_options);
                JsonBox.Text = JsonSerializer.Serialize(_options, _json);
            }
        }
    }

    private void UpdateActivePathText()
    {
        if (string.IsNullOrWhiteSpace(_path))
        {
            ActivePathText.Text = "Chemin actif: (non defini)";
            return;
        }

        ActivePathText.Text = $"Chemin actif: {Path.GetFullPath(_path)}";
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
        var candidates = new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() };

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

    private string? ResolvePreviewPath(string image)
    {
        if (string.IsNullOrWhiteSpace(image))
            return null;

        if (Path.IsPathRooted(image))
            return File.Exists(image) ? Path.GetFullPath(image) : null;

        var roots = new[]
        {
            Path.GetDirectoryName(_path) ?? AppContext.BaseDirectory,
            Directory.GetCurrentDirectory(),
            AppContext.BaseDirectory
        };

        var normalized = image.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        var fileName = Path.GetFileName(normalized);

        foreach (var root in roots)
        {
            var candidate = Path.GetFullPath(Path.Combine(root, normalized));
            if (File.Exists(candidate))
                return candidate;

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                var fallback = Path.Combine(root, "Assets", "backgrounds", fileName);
                if (File.Exists(fallback))
                    return fallback;
            }
        }

        return null;
    }

    private void UpdateBackgroundPreview()
    {
        var firstImage = BackgroundImagesBox.Text
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .FirstOrDefault(s => s.Length > 0);

        var resolved = ResolvePreviewPath(firstImage ?? string.Empty);
        if (string.IsNullOrWhiteSpace(resolved) || !File.Exists(resolved))
        {
            BackgroundPreviewImage.Source = null;
            BackgroundPreviewFallback.Text = "Image introuvable";
            BackgroundPreviewFallback.Visibility = Visibility.Visible;
            return;
        }

        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(resolved, UriKind.Absolute);
            bitmap.EndInit();
            bitmap.Freeze();

            BackgroundPreviewImage.Source = bitmap;
            BackgroundPreviewFallback.Visibility = Visibility.Collapsed;
        }
        catch
        {
            BackgroundPreviewImage.Source = null;
            BackgroundPreviewFallback.Text = "Erreur de chargement";
            BackgroundPreviewFallback.Visibility = Visibility.Visible;
        }
    }

    private void SubscribeToChanges()
    {
        var textBoxes = new[]
        {
            BackgroundModeBox, BackgroundStretchBox, BackgroundOpacityBox, BackgroundIntervalBox, BackgroundImagesBox,
            MatrixGlyphColorBox, MatrixHeadColorBox, MatrixShadowColorBox, MatrixFontSizeBox, MatrixSpeedBox, MatrixDensityBox,
            MatrixSpawnMultiplierBox, MatrixFadeOpacityBox, MatrixFontFamilyBox, MatrixAlphabetBox,
            EffectsGlowIntensityBox, EffectsScanLineOpacityBox
        };

        foreach (var tb in textBoxes)
            tb.TextChanged += (_, _) => { if (!_updatingForm) OnFormChanged(); };

        var checkBoxes = new[]
        {
            EffectsGlowBox, EffectsScanLinesBox, EffectsCrtBox, EffectsVignetteBox, EffectsSmokeBox, EffectsParticlesBox,
            ScreensMultiScreenBox, ScreensExitOnMoveBox, ScreensHideCursorBox
        };

        foreach (var cb in checkBoxes)
        {
            cb.Checked += (_, _) => { if (!_updatingForm) OnFormChanged(); };
            cb.Unchecked += (_, _) => { if (!_updatingForm) OnFormChanged(); };
        }
    }

    private void OnFormChanged()
    {
        try
        {
            _options = ReadOptionsFromForm();
            JsonBox.Text = JsonSerializer.Serialize(_options, _json);
            UpdateColorPreviews();
            UpdateBackgroundPreview();
            UpdateGlyphPreview();
        }
        catch
        {
            // Ignore temporary parse errors while typing.
        }
    }

    private void Open_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog { Filter = "JSON (*.json)|*.json|Tous les fichiers|*.*" };
        if (dialog.ShowDialog() != true) return;

        JsonBox.Text = File.ReadAllText(dialog.FileName);
        ApplyJsonToForm();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        ApplyFormToJson();

        var fullPath = Path.GetFullPath(_path);
        var dir = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrWhiteSpace(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(fullPath, JsonBox.Text);
        UpdateActivePathText();
        System.Windows.MessageBox.Show($"Configuration active sauvegardee: {fullPath}");
    }

    private void Demon_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var presetPath = ResolvePresetPath("demonbar.hell.matrix.json");
            if (presetPath is null)
            {
                System.Windows.MessageBox.Show("Preset introuvable: demonbar.hell.matrix.json", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var json = File.ReadAllText(presetPath);
            var options = JsonSerializer.Deserialize<MatrixScreensaverOptions>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (options is null)
            {
                System.Windows.MessageBox.Show("Le fichier de preset est invalide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _options = options;
            LoadOptionsToForm(_options);
            JsonBox.Text = JsonSerializer.Serialize(_options, _json);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Erreur lors du chargement du preset: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ActivePathText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        try
        {
            var directory = Path.GetDirectoryName(Path.GetFullPath(_path));
            if (!string.IsNullOrWhiteSpace(directory))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = directory,
                    UseShellExecute = true
                });
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Impossible d'ouvrir le dossier: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void AddImages_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Images (*.jpg;*.jpeg;*.png;*.webp;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.webp;*.bmp;*.gif",
            Multiselect = true
        };

        if (dialog.ShowDialog() != true)
            return;

        var existing = BackgroundImagesBox.Text
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => s.Length > 0)
            .ToList();

        foreach (var file in dialog.FileNames)
        {
            if (!existing.Any(x => x.Equals(file, StringComparison.OrdinalIgnoreCase)))
                existing.Add(file);
        }

        BackgroundImagesBox.Text = string.Join(Environment.NewLine, existing);
        OnFormChanged();
    }

    private void ClearImages_Click(object sender, RoutedEventArgs e)
    {
        BackgroundImagesBox.Text = string.Empty;
        OnFormChanged();
    }

    private void ApplyAlphabetPreset_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not System.Windows.Controls.Button button || button.Tag is not string preset)
            return;

        MatrixAlphabetBox.Text = preset;
        if (!_updatingForm)
            OnFormChanged();
    }

    private void UpdateGlyphPreview()
    {
        var alphabet = MatrixAlphabetBox.Text?.Trim() ?? string.Empty;
        var key = alphabet.ToLowerInvariant();

        var preview = key switch
        {
            "katakana" => "\u30AB \u30BF \u30AB \u30CA \u30DE \u30C8\n\u30EA \u30AF \u30B9 \u30CD \u30AA \u30F3",
            "hex" => "A3 F9 7C 1E D4 8B\n0F 2A 9D C1 7B E0",
            "runes" => "\u16A0 \u16A2 \u16A6 \u16A8 \u16B1 \u16B2\n\u16B7 \u16B9 \u16BA \u16BE \u16C1 \u16C7",
            "demon" => "\u26E7 \u16B2 \u16C9 \u27C1 \u26E4 \u263D\n\u2620 \u269A \u2720 \u16A6 \u16DF \u16CB",
            "binary" => "01001101 01000001\n01010100 01010010",
            _ => "A B C D E F\n1 2 3 4 5 6"
        };

        // Use a symbol-capable font for rune/demon previews.
        MatrixGlyphPreviewText.FontFamily = (key == "runes" || key == "demon")
            ? new System.Windows.Media.FontFamily("Segoe UI Symbol")
            : new System.Windows.Media.FontFamily("Consolas");

        MatrixGlyphPreviewText.Text = preview;
    }
    private void PickMatrixGlyphColor_Click(object sender, RoutedEventArgs e) => PickColorInto(MatrixGlyphColorBox);
    private void PickMatrixHeadColor_Click(object sender, RoutedEventArgs e) => PickColorInto(MatrixHeadColorBox);
    private void PickMatrixShadowColor_Click(object sender, RoutedEventArgs e) => PickColorInto(MatrixShadowColorBox);

    private void PickColorInto(System.Windows.Controls.TextBox target)
    {
        var dialog = new Forms.ColorDialog();
        try
        {
            var converted = System.Windows.Media.ColorConverter.ConvertFromString(target.Text);
            if (converted is System.Windows.Media.Color c)
                dialog.Color = System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
        }
        catch
        {
            // keep default selection
        }

        if (dialog.ShowDialog() == Forms.DialogResult.OK)
        {
            target.Text = $"#{dialog.Color.R:X2}{dialog.Color.G:X2}{dialog.Color.B:X2}";
            OnFormChanged();
        }
    }

    private void UpdateColorPreviews()
    {
        SetPreview(MatrixGlyphColorPreview, MatrixGlyphColorBox.Text);
        SetPreview(MatrixHeadColorPreview, MatrixHeadColorBox.Text);
        SetPreview(MatrixShadowColorPreview, MatrixShadowColorBox.Text);
    }

    private static void SetPreview(Border border, string colorText)
    {
        try
        {
            var converted = System.Windows.Media.ColorConverter.ConvertFromString(colorText);
            if (converted is System.Windows.Media.Color color)
            {
                border.Background = new SolidColorBrush(color);
                return;
            }
        }
        catch
        {
            // fallback below
        }

        border.Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#333333"));
    }

    private void ApplyFormToJson_Click(object sender, RoutedEventArgs e) => ApplyFormToJson();
    private void ApplyJsonToForm_Click(object sender, RoutedEventArgs e) => ApplyJsonToForm();

    private void ApplyFormToJson()
    {
        try
        {
            _options = ReadOptionsFromForm();
            JsonBox.Text = JsonSerializer.Serialize(_options, _json);
            UpdateColorPreviews();
            UpdateBackgroundPreview();
            UpdateGlyphPreview();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Erreur de formulaire: {ex.Message}");
        }
    }

    private void ApplyJsonToForm()
    {
        try
        {
            var parsed = JsonSerializer.Deserialize<MatrixScreensaverOptions>(JsonBox.Text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            _options = parsed ?? new MatrixScreensaverOptions();
            LoadOptionsToForm(_options);
            JsonBox.Text = JsonSerializer.Serialize(_options, _json);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"JSON invalide: {ex.Message}");
        }
    }

    private void LoadOptionsToForm(MatrixScreensaverOptions options)
    {
        _updatingForm = true;

        BackgroundModeBox.Text = options.Background.Mode;
        BackgroundStretchBox.Text = options.Background.Stretch;
        BackgroundOpacityBox.Text = options.Background.Opacity.ToString(CultureInfo.InvariantCulture);
        BackgroundIntervalBox.Text = options.Background.ChangeIntervalSeconds.ToString(CultureInfo.InvariantCulture);
        BackgroundImagesBox.Text = string.Join(Environment.NewLine, options.Background.Images);

        MatrixGlyphColorBox.Text = options.Matrix.GlyphColor;
        MatrixHeadColorBox.Text = options.Matrix.HeadColor;
        MatrixShadowColorBox.Text = options.Matrix.ShadowColor;
        MatrixFontSizeBox.Text = options.Matrix.FontSize.ToString(CultureInfo.InvariantCulture);
        MatrixSpeedBox.Text = options.Matrix.Speed.ToString(CultureInfo.InvariantCulture);
        MatrixDensityBox.Text = options.Matrix.Density.ToString(CultureInfo.InvariantCulture);
        MatrixSpawnMultiplierBox.Text = options.Matrix.SpawnMultiplier.ToString(CultureInfo.InvariantCulture);
        MatrixFadeOpacityBox.Text = options.Matrix.FadeOpacity.ToString(CultureInfo.InvariantCulture);
        MatrixFontFamilyBox.Text = options.Matrix.FontFamily;
        MatrixAlphabetBox.Text = options.Matrix.Alphabet;

        EffectsGlowBox.IsChecked = options.Effects.Glow;
        EffectsGlowIntensityBox.Text = options.Effects.GlowIntensity.ToString(CultureInfo.InvariantCulture);
        EffectsScanLinesBox.IsChecked = options.Effects.ScanLines;
        EffectsScanLineOpacityBox.Text = options.Effects.ScanLineOpacity.ToString(CultureInfo.InvariantCulture);
        EffectsCrtBox.IsChecked = options.Effects.Crt;
        EffectsVignetteBox.IsChecked = options.Effects.Vignette;
        EffectsSmokeBox.IsChecked = options.Effects.Smoke;
        EffectsParticlesBox.IsChecked = options.Effects.Particles;

        ScreensMultiScreenBox.IsChecked = options.Screens.MultiScreen;
        ScreensExitOnMoveBox.IsChecked = options.Screens.ExitOnMouseMove;
        ScreensHideCursorBox.IsChecked = options.Screens.HideCursor;

        UpdateColorPreviews();
        UpdateBackgroundPreview();
            UpdateGlyphPreview();
        _updatingForm = false;
    }

    private MatrixScreensaverOptions ReadOptionsFromForm()
    {
        return new MatrixScreensaverOptions
        {
            Background = new BackgroundOptions
            {
                Mode = BackgroundModeBox.Text.Trim(),
                Stretch = BackgroundStretchBox.Text.Trim(),
                Opacity = ParseDouble(BackgroundOpacityBox.Text, 1.0),
                ChangeIntervalSeconds = ParseInt(BackgroundIntervalBox.Text, 30),
                Images = BackgroundImagesBox.Text
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => s.Length > 0)
                    .ToList()
            },
            Matrix = new MatrixOptions
            {
                GlyphColor = MatrixGlyphColorBox.Text.Trim(),
                HeadColor = MatrixHeadColorBox.Text.Trim(),
                ShadowColor = MatrixShadowColorBox.Text.Trim(),
                FontSize = ParseInt(MatrixFontSizeBox.Text, 22),
                Speed = ParseDouble(MatrixSpeedBox.Text, 1.2),
                Density = ParseDouble(MatrixDensityBox.Text, 0.85),
                SpawnMultiplier = ParseInt(MatrixSpawnMultiplierBox.Text, 4),
                FadeOpacity = ParseDouble(MatrixFadeOpacityBox.Text, 0.08),
                FontFamily = MatrixFontFamilyBox.Text.Trim(),
                Alphabet = MatrixAlphabetBox.Text.Trim()
            },
            Effects = new EffectsOptions
            {
                Glow = EffectsGlowBox.IsChecked == true,
                GlowIntensity = ParseDouble(EffectsGlowIntensityBox.Text, 0.8),
                ScanLines = EffectsScanLinesBox.IsChecked == true,
                ScanLineOpacity = ParseDouble(EffectsScanLineOpacityBox.Text, 0.18),
                Crt = EffectsCrtBox.IsChecked == true,
                Vignette = EffectsVignetteBox.IsChecked == true,
                Smoke = EffectsSmokeBox.IsChecked == true,
                Particles = EffectsParticlesBox.IsChecked == true
            },
            Screens = new ScreenOptions
            {
                MultiScreen = ScreensMultiScreenBox.IsChecked == true,
                ExitOnMouseMove = ScreensExitOnMoveBox.IsChecked == true,
                HideCursor = ScreensHideCursorBox.IsChecked == true
            }
        };
    }

    private static int ParseInt(string text, int fallback)
        => int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) ? value : fallback;

    private static double ParseDouble(string text, double fallback)
        => double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : fallback;

    // Depth Layers Management
    private void DepthAddLayer_Click(object sender, RoutedEventArgs e)
    {
        if (_options.Depth == null)
            _options.Depth = new DepthOptions();

        var newLayer = new DepthLayerOptions 
        { 
            Depth = _options.Depth.Layers.Count > 0 ? _options.Depth.Layers.Max(l => l.Depth) + 0.25 : 0.5,
            Opacity = 1.0, 
            SpeedMultiplier = 1.0, 
            DensityMultiplier = 1.0 
        };
        
        _options.Depth.Layers.Add(newLayer);
        LoadDepthLayersToForm();
        OnFormChanged();
    }

    private void DepthRemoveLayer_Click(object sender, RoutedEventArgs e)
    {
        if (DepthLayersListBox.SelectedItem is DepthLayerOptions layer && _options.Depth?.Layers.Contains(layer) == true)
        {
            _options.Depth.Layers.Remove(layer);
            LoadDepthLayersToForm();
            OnFormChanged();
        }
    }

    private void DepthLayer_Changed(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (!_updatingForm)
            OnFormChanged();
    }

    private void LoadDepthLayersToForm()
    {
        _updatingForm = true;
        var layers = _options.Depth?.Layers ?? new System.Collections.Generic.List<DepthLayerOptions>();
        var sortedLayers = layers.OrderBy(l => l.Depth).ToList();
        
        DepthLayersListBox.ItemsSource = null; // Reset binding
        DepthLayersListBox.ItemsSource = sortedLayers;
        DepthRemoveLayerBtn.IsEnabled = sortedLayers.Count > 0;
        
        _updatingForm = false;
    }

    private void DepthLayersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        DepthRemoveLayerBtn.IsEnabled = DepthLayersListBox.SelectedItem != null;
    }}









