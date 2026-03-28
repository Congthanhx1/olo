# Hướng dẫn đưa DigitalStore lên Online

Xin chào @Congthanhx1, tớ đã cập nhật các file trong dự án của cậu để sẵn sàng deploy.

## Các file tớ đã chỉnh sửa (Vui lòng kiểm tra):
1. **Backend**: `DigitalStore/src/DigitalStore.API/appsettings.json` (Đã ẩn mật khẩu, dùng Env Var).
2. **Backend**: `DigitalStore/src/DigitalStore.API/Program.cs` (Cập nhật CORS).
3. **Frontend**: `Desktop/DigitalStore_Web/app.js` (Cập nhật URL API online).

## Các bước thực hiện:

### Bước 1: Đẩy mã nguồn lên GitHub
1. Mở thư mục `DigitalStore`, chuột phải chọn **Run with PowerShell** file `setup_git_backend.ps1`. Nhập link repo GitHub cho Backend.
2. Mở thư mục `DigitalStore_Web`, chuột phải chọn **Run with PowerShell** file `setup_git_frontend.ps1`. Nhập link repo GitHub cho Frontend.

### Bước 2: Deploy Backend lên Render
1. Lên [Render.com](https://render.com), chọn **New Web Service**, kết nối repo Backend.
2. Thêm các **Environment Variables** (như đã hướng dẫn ở tin nhắn trước).

### Bước 3: Deploy Frontend lên Render/Netlify
1. Chọn **New Static Site**, kết nối repo Frontend.
2. Render sẽ tự động cấp link (ví dụ: `digitalstore-web.onrender.com`).

Nếu vẫn không thấy các file này, cậu hãy nhấn **F5** hoặc **Refresh** thư mục nhé!
