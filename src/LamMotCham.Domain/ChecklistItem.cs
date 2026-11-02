namespace LamMotCham.Domain;

public sealed record ChecklistItem(string Id, string Title, string Detail, bool IsComplete = false);

public static class DefaultChecklist
{
    public static IReadOnlyList<ChecklistItem> Items { get; } =
    [
        new("windows-update", "Cập nhật Windows", "Kiểm tra bản cập nhật và khởi động lại nếu cần."),
        new("drivers", "Kiểm tra trình điều khiển", "Xác nhận các thiết bị chính hoạt động bình thường."),
        new("security", "Thiết lập bảo mật", "Kiểm tra Defender, tường lửa và trạng thái bảo vệ."),
        new("apps", "Cài ứng dụng cần thiết", "Cài theo nhu cầu của khách hàng và chính sách cửa hàng."),
        new("handover", "Bàn giao và hướng dẫn", "Xác nhận phụ kiện, tài khoản và hướng dẫn sử dụng.")
    ];
}
