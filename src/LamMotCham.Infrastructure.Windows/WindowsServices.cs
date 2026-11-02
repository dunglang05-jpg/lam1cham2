using System.Text.Json;
using LamMotCham.Application;

namespace LamMotCham.Infrastructure.Windows;

public sealed class WindowsDeviceInfoProvider : IDeviceInfoProvider
{
    public DeviceSnapshot Capture() => new(
        Environment.MachineName,
        Environment.UserName,
        $"{Environment.OSVersion.Platform} {Environment.OSVersion.Version}",
        Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit",
        DateTime.Now);
}

public sealed class JsonChecklistStore : IChecklistStore
{
    private readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LamMotCham", "checklist.json");

    public async Task SaveAsync(IReadOnlyCollection<string> completedIds, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        var data = new { SavedAt = DateTimeOffset.Now, CompletedIds = completedIds };
        await using var stream = File.Create(_path);
        await JsonSerializer.SerializeAsync(stream, data, new JsonSerializerOptions { WriteIndented = true }, cancellationToken);
    }
}
