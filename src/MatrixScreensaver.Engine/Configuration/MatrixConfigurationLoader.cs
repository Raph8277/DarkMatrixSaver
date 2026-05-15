using System.Text.Json;

namespace MatrixScreensaver.Engine.Configuration;

public static class MatrixConfigurationLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public static MatrixScreensaverOptions Load(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            return new MatrixScreensaverOptions();

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<MatrixScreensaverOptions>(json, JsonOptions) ?? new MatrixScreensaverOptions();
    }

    public static void Save(string path, MatrixScreensaverOptions options)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        File.WriteAllText(path, JsonSerializer.Serialize(options, JsonOptions));
    }
}
