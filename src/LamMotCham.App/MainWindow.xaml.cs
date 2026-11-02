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
