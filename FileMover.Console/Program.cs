// See https://aka.ms/new-console-template for more information
using System.Text.Json;

namespace DownloadMover;
public class Config
{
    public List<string>? ImageExtensions { get; set; }
    public List<string>? ApplicationExtensions { get; set; }
    public List<string>? VideoExtensions { get; set; }
    public List<string>? OtherExtensions { get; set; }
    public string? ImageFolder { get; set; }
    public string? VideoFolder { get; set; }
    public string? ApplicationFolder { get; set; }
    public string? OtherFolder { get; set; }
    public string? DownloadFolder { get; set; }
}

public class Program
{
    private static Config? _config;
    public static void Main(string[] args)
    {
        _config = JsonSerializer.Deserialize<Config>(File.ReadAllText(@".\config.json"));
        while (true)
        {
            using var watcher = new FileSystemWatcher(_config!.DownloadFolder!);
            watcher.NotifyFilter = NotifyFilters.Attributes
                                      | NotifyFilters.CreationTime
                                      | NotifyFilters.DirectoryName
                                      | NotifyFilters.FileName
                                      | NotifyFilters.LastAccess
                                      | NotifyFilters.LastWrite
                                      | NotifyFilters.Security
                                      | NotifyFilters.Size;
            watcher.Renamed += OnRenamed;
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            Thread.Sleep(1000);
        }
    }

    private static void OnRenamed(object sender, RenamedEventArgs e)
    {
        var file = new FileInfo(e.FullPath);
        if (_config?.ImageExtensions?.Contains(file.Extension) ?? false)
        {
            File.Move(e.FullPath, Path.Combine(_config!.ImageFolder!, file.Name), true);
            return;
        }
        if (_config?.VideoExtensions?.Contains(file.Extension) ?? false)
        {
            File.Move(e.FullPath, Path.Combine(_config!.VideoFolder!, file.Name), true);
            return;
        }
        if (_config?.ApplicationExtensions?.Contains(file.Extension) ?? false)
        {
            File.Move(e.FullPath, Path.Combine(_config!.ApplicationFolder!, file.Name), true);
            return;
        }
        if (_config?.OtherExtensions?.Contains(file.Extension) ?? false)
        {
            File.Move(e.FullPath, Path.Combine(_config!.OtherFolder!, file.Name), true);
            return;
        }
    }
}
