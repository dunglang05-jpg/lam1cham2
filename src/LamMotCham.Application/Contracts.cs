namespace LamMotCham.Application;

public sealed record DeviceSnapshot(string ComputerName, string UserName, string OperatingSystem, string Architecture, DateTime CapturedAt);

public interface IDeviceInfoProvider { DeviceSnapshot Capture(); }
public interface IChecklistStore { Task SaveAsync(IReadOnlyCollection<string> completedIds, CancellationToken cancellationToken = default); }
