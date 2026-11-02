using System.Diagnostics;

namespace LamMotCham.Infrastructure.Windows;

public sealed record SoftwarePackage(string Id, string Name, string Description);

public sealed record InstallResult(bool Success, int ExitCode, string Output);

public sealed class WingetInstaller
{
    public static IReadOnlyList<SoftwarePackage> Catalog { get; } =
    [
        new("Google.Chrome", "Google Chrome", "Trình duyệt web phổ biến"),
        new("Brave.Brave", "Brave Browser", "Trình duyệt tích hợp chặn quảng cáo"),
        new("7zip.7zip", "7-Zip", "Giải nén ZIP, 7z và nhiều định dạng"),
        new("VideoLAN.VLC", "VLC Media Player", "Phát video và âm thanh"),
        new("SumatraPDF.SumatraPDF", "SumatraPDF", "Đọc PDF gọn nhẹ")
    ];

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "winget.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            process.StartInfo.ArgumentList.Add("--version");
            process.Start();
            await process.WaitForExitAsync(cancellationToken);
            return process.ExitCode == 0;
        }
        catch (Exception) when (!cancellationToken.IsCancellationRequested)
        {
            return false;
        }
    }

    public async Task<InstallResult> InstallAsync(
        SoftwarePackage package,
        CancellationToken cancellationToken = default)
    {
        if (!Catalog.Any(item => item.Id == package.Id))
            throw new InvalidOperationException("Gói cài đặt không nằm trong danh sách được cho phép.");

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "winget.exe",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        foreach (var argument in new[]
        {
            "install", "--id", package.Id, "--exact", "--silent",
            "--accept-package-agreements", "--accept-source-agreements",
            "--disable-interactivity"
        })
        {
            process.StartInfo.ArgumentList.Add(argument);
        }

        process.Start();
        var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        var output = (await stdoutTask) + Environment.NewLine + (await stderrTask);
        return new InstallResult(process.ExitCode == 0, process.ExitCode,
            output.Length > 4000 ? output[^4000..] : output);
    }
}
