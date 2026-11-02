# Làm 1 Chạm

Ứng dụng desktop Windows WPF trên .NET 10 để hỗ trợ kỹ thuật viên chuẩn bị và bàn giao laptop theo checklist.

## Phạm vi hiện tại

- Đọc một số thông tin thiết bị từ Windows khi người dùng bấm nút.
- Checklist bàn giao do kỹ thuật viên tự đánh dấu.
- Lưu tiến độ checklist thành JSON trong `%LocalAppData%/LamMotCham/checklist.json`.
- Không tự cài ứng dụng, cập nhật Windows hay thay đổi cấu hình máy.

## Yêu cầu

- Windows 10/11
- .NET 10 SDK

## Build

```powershell
dotnet build LamMotCham.slnx -c Release
```

Đây là bản mã nguồn được khôi phục lại từ phạm vi đã trao đổi trong chat; không phải bản phục hồi nguyên trạng từ ổ đĩa cũ.
