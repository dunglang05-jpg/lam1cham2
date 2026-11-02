using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LamMotCham.Application;
using LamMotCham.Domain;
using LamMotCham.Infrastructure.Windows;

namespace LamMotCham.App;

public partial class MainWindow : Window
{
    private readonly List<(string Id, CheckBox CheckBox)> _checks = [];
    private readonly IDeviceInfoProvider _deviceInfo = new WindowsDeviceInfoProvider();
    private readonly IChecklistStore _store = new JsonChecklistStore();
    private readonly WingetInstaller _installer = new();

    public MainWindow()
    {
        InitializeComponent();
        foreach (var item in DefaultChecklist.Items)
        {
            var check = new CheckBox { Content = new StackPanel { Children = { new TextBlock { Text = item.Title, FontSize = 15, FontWeight = FontWeights.SemiBold, Foreground = (Brush)FindResource("Ink") }, new TextBlock { Text = item.Detail, TextWrapping = TextWrapping.Wrap, Foreground = (Brush)FindResource("Muted"), Margin = new Thickness(0, 5, 0, 0) } } }, Padding = new Thickness(4, 15, 4, 15), BorderBrush = new SolidColorBrush(Color.FromRgb(232, 236, 243)), BorderThickness = new Thickness(0, 0, 0, 1) };
            check.Checked += (_, _) => UpdateProgress(); check.Unchecked += (_, _) => UpdateProgress();
            ChecklistPanel.Children.Add(check); _checks.Add((item.Id, check));
        }
        UpdateProgress();
        BuildSoftwareCatalog();
    }

    private void BuildSoftwareCatalog()
    {
        foreach (var package in WingetInstaller.Catalog)
        {
            var row = new Grid { Margin = new Thickness(0, 0, 0, 8) };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            var details = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            details.Children.Add(new TextBlock { Text = package.Name, FontWeight = FontWeights.SemiBold, FontSize = 11, Foreground = (Brush)FindResource("Ink") });
            details.Children.Add(new TextBlock { Text = package.Description, FontSize = 9, Foreground = (Brush)FindResource("Muted"), Margin = new Thickness(0, 2, 8, 0) });
            var installButton = new Button { Content = "Cài đặt", Padding = new Thickness(10, 7, 10, 7), FontSize = 10, Tag = package };
            installButton.Click += InstallSoftware_Click;
            Grid.SetColumn(installButton, 1);
            row.Children.Add(details);
            row.Children.Add(installButton);
            SoftwarePanel.Children.Add(row);
        }
    }

    private async void InstallSoftware_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not SoftwarePackage package)
            return;

        var answer = MessageBox.Show(
            $"Bạn muốn cài {package.Name} bằng Windows Package Manager (winget)?\n\nỨng dụng sẽ được tải từ nguồn winget đã cấu hình trên máy.",
            "Xác nhận cài đặt",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
        if (answer != MessageBoxResult.Yes)
            return;

        button.IsEnabled = false;
        button.Content = "Đang cài...";
        try
        {
            StatusText.Text = $"Đang kiểm tra winget để cài {package.Name}...";
            if (!await _installer.IsAvailableAsync())
            {
                StatusText.Text = "Máy chưa có winget. Hãy cập nhật App Installer từ Microsoft Store rồi thử lại.";
                return;
            }

            var result = await _installer.InstallAsync(package);
            StatusText.Text = result.Success
                ? $"Đã cài đặt {package.Name} thành công."
                : $"Cài {package.Name} chưa thành công (mã {result.ExitCode}). {result.Output}";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Không thể cài {package.Name}: {ex.Message}";
        }
        finally
        {
            button.IsEnabled = true;
            button.Content = "Cài đặt";
        }
    }

    private void UpdateProgress()
    {
        var count = _checks.Count(x => x.CheckBox.IsChecked == true);
        ProgressText.Text = $"{count} / {_checks.Count} mục hoàn tất";
    }

    private void ReadDevice_Click(object sender, RoutedEventArgs e)
    {
        var device = _deviceInfo.Capture();
        DeviceName.Text = device.ComputerName;
        DeviceDetails.Text = $"Người dùng: {device.UserName}\nHệ điều hành: {device.OperatingSystem}\nKiến trúc: {device.Architecture}\nĐọc lúc: {device.CapturedAt:dd/MM/yyyy HH:mm}";
        StatusText.Text = "Đã đọc thông tin trực tiếp từ máy này.";
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var completed = _checks.Where(x => x.CheckBox.IsChecked == true).Select(x => x.Id).ToArray();
            await _store.SaveAsync(completed);
            StatusText.Text = "Đã lưu checklist vào thư mục dữ liệu ứng dụng của Windows.";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Không thể lưu checklist: {ex.Message}";
        }
    }
}
