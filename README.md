# 🎮 Tank Battle

> Một tựa game bắn súng sinh tồn theo đợt (wave survival) góc nhìn từ trên xuống được phát triển bằng Unity (C#), lấy cảm hứng từ cơ chế chiến đấu của **BoxHead** và **Archero**.

---

## 📖 Giới thiệu

**Tank Battle** là game bắn súng sinh tồn nhìn từ trên xuống, nơi người chơi điều khiển một chiếc xe tăng chiến đấu qua các đợt kẻ thù ngày càng mạnh hơn. Mục tiêu là tồn tại càng lâu càng tốt, thu thập vàng trong trận đấu để **nâng cấp vĩnh viễn các chỉ số của xe tăng** và **mua các buff hỗ trợ** tại màn hình chính trước khi bắt đầu hành trình phá kỷ lục số wave mới.

---

## 🕹️ Cách chơi

| Phím | Hành động |
|------|-----------|
| `W` | Tiến về phía trước |
| `S` | Lùi về phía sau |
| `A` | Xoay trái |
| `D` | Xoay phải |
| `1` | Dùng Buff 1 — Hồi máu |
| `2` | Dùng Buff 2 — Tốc độ bắn |
| `3` | Dùng Buff 3 — Bất tử |

- Tiêu diệt toàn bộ kẻ thù để hoàn thành wave.
- Sau mỗi wave, chọn **1 trong 3 phần thưởng** để tăng sức mạnh tức thời trong trận.
- Tích lũy vàng thu thập từ kẻ thù để:
  - **Nâng cấp thuộc tính xe tăng** (HP, Shield, Sức tấn công) vĩnh viễn tại Garage.
  - **Mua trữ sẵn các Buff kích hoạt** (tối đa mang vào trận để sử dụng).

---

## ⚔️ Cơ chế chính

### 🚗 Xe tăng người chơi
- Điều khiển theo kiểu xe tăng thật: xoay thân rồi mới di chuyển theo hướng đó.
- Hệ thống máu kép: **Shield** hấp thụ sát thương trước, **HP** chỉ giảm khi Shield về 0.
- Shield tự hồi sinh sau vài giây không bị tấn công.

### 🆙 Hệ thống Nâng cấp thuộc tính (Upgrades)
Người chơi dùng vàng tích lũy được sau các trận đấu để nâng cấp các chỉ số cơ bản của xe tăng tại màn hình chính. Giá nâng cấp tăng dần theo từng cấp độ:
- **HP (Máu tối đa):** Tăng khả năng chịu đựng khi Shield bị vỡ.
- **Shield (Giáp/Khiên tối đa):** Tăng lượng sát thương hấp thụ trước khi bị trừ vào HP.
- **Attack Stat (Sức tấn công cơ bản):** Tăng lượng sát thương gây ra cho tất cả các loại vũ khí.

### 🔫 Hệ thống vũ khí — 4 loại nòng súng
| Vũ khí | Mô tả |
|--------|-------|
| Standard | Bắn 1 viên đạn thẳng, cân bằng |
| Triple Shot | Bắn 3 viên cùng lúc theo hình rẻ quạt |
| Explosive | Đạn nổ diện rộng khi chạm mục tiêu |
| Laser Beam | Tia laser liên tục gây sát thương theo thời gian |

> Vũ khí được chọn qua **phần thưởng cuối wave đầu tiên**.

### 👾 Loại kẻ thù
| Kẻ thù | Hành vi |
|--------|---------|
| **Boom Tank** | Lao thẳng vào người chơi, nổ khi va chạm |
| **Shooter Tank** | Giữ khoảng cách, bắn đạn từ xa |
| **Boss Tank** | HP và sát thương cao, xuất hiện ở các wave cột mốc |

### 🌊 Hệ thống Wave
- Wave càng cao, kẻ thù càng nhiều và mạnh hơn theo hệ số **mũ (×1.1 mỗi wave)**.
- Cứ mỗi **bội số của 10** (wave 10, 20, 30...) sẽ xuất hiện thêm Boss.
- Số wave cao nhất đạt được sẽ được lưu lại.

### 🎁 Phần thưởng cuối wave
Sau mỗi wave, chọn **1 trong 3** phần thưởng ngẫu nhiên:
- Tăng HP tối đa / Shield tối đa / Sát thương / Tốc độ bắn / Tốc độ di chuyển.
- Hoặc đổi sang vũ khí mới (chỉ ở wave 1).

### 💊 Cửa hàng & Buff kích hoạt (Buff Store)
Người chơi phải **dùng vàng để mua tích trữ các Buff** tại màn hình chính trước khi vào trận. Trong trận đấu, số lượng buff sẽ giảm đi mỗi khi sử dụng:
| Buff | Phím | Hiệu ứng chiến thuật | Cooldown |
|------|------|-----------------------|----------|
| **Heal** | `1` | Hồi ngay lập tức 15% HP tối đa | 7s |
| **Rapid Fire** | `2` | Tăng 100% tốc độ bắn (giảm 50% cooldown nòng) trong 5s | 10s |
| **Invincibility** | `3` | Kích hoạt trạng thái bất tử (miễn nhiễm sát thương) trong 5s | 15s |

### 🏆 Achievement
Hệ thống thành tích theo dõi các mốc trong suốt quá trình chơi:
- Tổng số kẻ thù đã tiêu diệt
- Số wave cao nhất đạt được
- Tổng vàng đã chi tiêu
- Tổng số ván đã chơi

Mở khóa thành tích sẽ nhận được **phần thưởng vàng**.

---

## 🛠️ Công nghệ sử dụng

- **Unity** — Game Engine
- **C#** — Ngôn ngữ lập trình chính
- **DOTween** — Hiệu ứng animation UI
- **TextMesh Pro** — Hiển thị văn bản
- **ScriptableObjects** — Cấu hình wave & phần thưởng
- **PlayerPrefs** — Lưu dữ liệu người chơi cục bộ

---

## 📐 Kiến trúc nổi bật

- **Singleton Pattern** — Quản lý tập trung: Game State, Audio, Data, Achievement
- **Observer Pattern** — Hệ thống sự kiện generic, decoupled giữa các module
- **Object Pooling** — Tái sử dụng đạn, kẻ thù, hiệu ứng VFX để tối ưu hiệu năng

---
