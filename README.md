# 🎮 Offline Games Collection

> Một bộ sưu tập các trò chơi giải đố 2D được phát triển bằng Unity (C#), tập trung vào trải nghiệm tựa game giải đó đơn giản, dễ thao tác và thú vị.

---

## 📖 Giới thiệu

**Offline Games Collection** là một ứng dụng tổng hợp hơn 10 mini-game logic và giải đố kinh điển (Sudoku, Minesweeper, Tetris, Wordle, Water Sort, ...). Dự án được thiết kế chuẩn chỉ để chạy ngoại tuyến (offline) với mục tiêu hiệu năng (90 FPS target), giao diện trực quan sống động nhờ DOTween và hệ thống bảo mật dữ liệu cá nhân hóa nâng cao.

---

## 🕹️ Cách chơi

Vì đây là một bộ sưu tập game trí tuệ giải đố, các thao tác điều khiển chủ yếu xoay quanh việc tương tác chuột:

| Thao tác | Hành động chung |
|----------|-----------------|
| `Chuột trái (Left Click)` | Lựa chọn game, nhấn số điền ô, lật thẻ bài, đổ nước hoặc chọn tọa độ bắn. |
| `Nút Replay (R)` | Chơi lại ván hiện tại ngay lập tức. |
| `Nút Back Home` | Quay trở về màn hình danh sách game chính. |

---

## ⚔️ Cơ chế chính của các Game tiêu biểu

### 🧠 Sudoku Generator & Solver
- **Sinh lưới tự động:** Tạo lưới Sudoku ngẫu nhiên theo 3 độ khó (Easy, Medium, Hard) bằng phương pháp xáo trộn số kết hợp đệ quy.
- **Bảo chứng độc nhất:** Loại bỏ các ô trống (Digging Holes) nhưng luôn kiểm tra thông qua bộ giải thuật (**Recursive Backtracking DFS**) để đảm bảo mỗi ván chơi chỉ có duy nhất **1 đáp án đúng**.
- **Hỗ trợ thời gian thực:** Kiểm tra tính hợp lệ của số điền vào hàng, cột và phân vùng 3x3 ngay lập tức.

### 💣 Minesweeper (Dò mìn)
- **Tạo bản đồ an toàn:** Rải bom ngẫu nhiên sau lượt click đầu tiên của người chơi để tránh chết ngay lập tức.
- **Thuật toán loang:** Áp dụng giải thuật **Flood-fill** đệ quy để mở rộng nhanh toàn bộ vùng ô trống xung quanh khi người chơi click trúng ô không có mìn lân cận.

### 🧱 Tetris (Xếp gạch)
- **Lưới tọa độ động:** Kiểm tra va chạm xoay khối gạch (Super Rotation System đơn giản) và di chuyển mượt mà.
- **Xử lý xóa dòng:** Phát hiện nhanh dòng gạch đầy để thực hiện hiệu ứng xóa hàng, dồn gạch phía trên xuống và cộng điểm tích lũy.

### 🧪 Water Sort (Rót nước)
- **Cơ chế xếp chồng:** Sử dụng cấu trúc dữ liệu **Stack** cho mỗi ống nghiệm. Người chơi chỉ có thể đổ nước sang ống nghiệm khác nếu cùng màu nước ở đỉnh hoặc ống đích còn trống.

---

## 🧪 Hệ thống quản lý & Lưu trữ dữ liệu

### 💾 Hệ thống Save Game bảo mật
Nhằm chống lại việc chỉnh sửa file lưu trữ thủ công hoặc hack điểm số cục bộ, dự án triển khai lớp tiện ích nâng cao:
* **Mã hóa tùy biến:** Sử dụng giải thuật **XOR Encryption** mã hóa dữ liệu JSON thành tệp tin nhị phân `.dat`.
* **Khóa động theo phần cứng:** Key mã hóa được tạo trực tiếp từ định danh duy nhất của thiết bị người chơi (`SystemInfo.deviceUniqueIdentifier`).
* **Tính độc lập:** Mỗi game lưu dữ liệu điểm số/high-score tách biệt dưới dạng enum an toàn (`eData`).

---

## 🛠️ Công nghệ sử dụng

- **Unity** — Game Engine (URP)
- **C#** — Ngôn ngữ lập trình chính (.NET)
- **DOTween** — Thiết kế hiệu ứng menu chuyển cảnh và chuyển động UI động
- **TextMesh Pro** — Hệ thống hiển thị phông chữ sắc nét
- **ScriptableObjects** — Quản lý cấu hình cấu trúc dữ liệu
- **File I/O & System.IO** — Quản lý lưu trữ tệp tin cục bộ mã hóa

---

## 📐 Kiến trúc nổi bật

- **Singleton Pattern** — Triển khai cấu trúc lõi `MainGameManager` và `AudioManager` để quản lý tập trung trạng thái toàn cục (Global states) và âm thanh.
- **Interface-driven Architecture** — Sử dụng `IGameManager` để kết nối lỏng (decouple) giữa lõi hệ thống và các mini-game, giúp tích hợp một trò chơi mới vào hệ thống chỉ trong vài giây.
- **Generic Object Pooling** — Thiết kế `PoolingManager` dùng `Queue<GameObject>` phối hợp cùng đầu mục `HashSet<int>` tra cứu Instance ID, tối ưu hóa triệt để thời gian khởi tạo phần tử và triệt tiêu giật lag do Garbage Collection (GC spikes).
- **Observer Pattern** — Hệ thống sự kiện `ObserverManager` hỗ trợ giao tiếp bất đồng bộ giữa các thành phần UI và logic lõi trò chơi một cách tinh gọn.

---
