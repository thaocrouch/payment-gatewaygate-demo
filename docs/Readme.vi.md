# Payment Gateway

Author: Thao Nguyen Van

Date update: 2025-08-22

Version: v1.0

## Tổng quan hệ thống

Hộ thống hỗ trợ người dùng thanh toán hoá đơn, gửi email thông báo khi Khách hàng thanh toán thành công. Quản trị viên có thể kiểm tra, quản lý đơn hàng.

### Functional Requirements

- Quản trị viên có thể tìm kiếm / loc theo tên đơn hàng.
- Khách hàng nhận được email khi thanh toán thành công.
- Đẩy thông tin thanh toán cho hệ thống thứ 3.
- Tạo hoá đơn khi thanh toán thành công.

### Non‑Functional Requirements

- Hiệu năng: Các api có thời gian xử lý dới < 400ms.
- Khả năng mở rộng: api có thể scale theo chiều ngang khi có lượng lớn truy cập.
- Độ tin cậy & Khả dụng: Hệ thống luôn ở trang thái sẵn sàng, uptime > 99.9%, có hệ thống backup khi xẩy ra sự cố.
- Bảo mật: Bảo mật hệ thống bằng OAuth2/OIDC, cấu hình rate limit theo client ip (max 10 request tạo mới đơn hàng trong 10 phút).
- Độ linh hoạt: Có thể triển khai được trên nhiều hệ tầng khác nhau, cả On-Premis, Cloud.
- Khả năng bảo trì: hệ thống đáp ứng chuẩn clean architecture, dễ nâng cấp và bảo trì. Có hệ thống ghi log để hỗ trợ khi xẩy ra sự cố.

### Main flow

1. Người dùng login vào hệ thống
2. Tạo đơn hàng mới (Nhập thông tin đơn hàng, giá tiền, hình thức thanh toán có thể, v.v)
3. Hệ thống kiểm tra và tạo mới đơn hàng, hiển thị thông tin thanh toán cho người dùng
4. Người dùng thanh toán đơn hàng.
5. Người dùng thanh toán thành công => Hiển thị thông báo thành công. (hoặc gửi mail). Nếu thanh toán thất bại => Hiển thị thông tin thanh toán thất bại.
6. Với đơn hàng thanh toán thành công, hệ thống ghi nhận và chuyển thông tin thanh toán cho hệ thống nội bộ.
7. Người dùng có thể kiểm tra đơn hàng của mình trên hệ thống. (trạng thái, lỗi nếu có thể)

## High‑Level Architecture

Hệ thống sử dụng clean architecture với các thành phần sau.

- API Gateway: Xác thực và điều hướng chức năng.
- Order service: Thiếu kế theo chuẩn REST-Full api, hỗ trợ giao tiếp với nhiều hệ thống khác nhau.
- Notification:
  - Email service: Thông báo khi người dùng thanh toán thành công đơn hàng.
  - Internal service: Gửi thông tin thanh toán đơn hàng cho hệ thống nội bộ.
- Invoice service: Xử lý các yêu cầu liên quán đến hoá đơn. (in hoá đơn, cập nhật, v.v)
- Logs service: Ghi lại nhật ký logs trong các ứng dụng.
- Message Broker service: Phân phối, chuyển phát các tin nhắn, sự kiện trong hệ thống.
- Cache (tuỳ chọn): Hệ thống có lượng lớn truy cập đồng thời niên tục trong thời gian dài và hiệu năng kém thì sẽ dùng Redis cache để lưu tạm giữ liệu giao dịch, cập nhật giao dịch xuống database theo batch data.
- Logs service: sử dụng OpenTelemetry để logs, monitor và trace.

Orverview về hệ thống.

![Orverview](/img/structure.svg "Orverview system")

## Data Model & Schemas (use Dbdocs nếu có thời gian)

### Table Orders (sử dụng cho OrderService)

- Id: (UUID), PrimaryKey.
- UserId: (UUID), Mã định danh của user trên hệ thống.
- Name: (string), Tên đơn hàng.
- Amount: (number) Giá trị đơn hàng
- Note: (string) Ghi chú của đơn hàng.
- PaymentMethod: (int) Hình thức thanh toán.
- PaymentSubType: (int) Loại hình thức thanh toán: onetime, recurring...
- Status: (int) Trạng thái đơn hàng.
- CallbackUrl: (string) Url chuyển hướng về client tương ứng. (web: detail invoice, mobile: deeplink or any.)
- IpnUrl: (string) Url để ghi nhận gủi dữ liệu giữa backend-backend external.
- ErrorCode: (int) Mã lỗi ghi nhận khi xử lý giữa các hệ thống.
- ErrorMessage: (string) Chi tiết mã lỗi ghi nhận từ các hệ thống.
- ExpireDate: (datetime) Thời gian hết hạn của đơn hàng.
- CreatedDate: (datetime) Thời gian tạo đơn hàng.
- UpdatedDate: (datetime) Thời gian cập nhật của các hệ thống.

### Table OrderRetry (Phục vụ cho việc gọi retry nếu ghi nhận sang hệ thống internal lỗi)

- OrderId: (UUID), PK.
- Attempts: (int) Ghi nhận lại số lần gọi Retry.
- ErrorCode: (int) mã lỗi từ hệ thống internal.
- ErrorMessage: (string) Thông tin mã lỗi.

### Messages Events

- OrderMessage events
  - Id: (UUID) Mã đơn hàng. (tương đương OrderId)
  - UserId: (UUID) Mã định danh khách hàng trên hệ thống.
  - Status: (int) Trạng thái đơn hàng.

## APIs

Các api phục vụ cho hệ thống.

### Tạo mới đơn hàng

![Orverview](/img/create_order.png "Orverview system")

### Cập nhật đơn hàng

![Orverview](/img/update_order.png "Orverview system")

### Xoá đơn hàng

![Orverview](/img/delete_order.png "Orverview system")

### Tìm kiếm đơn hàng

![Orverview](/img/get_orders.png "Orverview system")

### Chi tiết đơn hàng

![Orverview](/img/detail_order.png "Orverview system")

### IPN - Ghi nhận thông tin đơn hàng giữa các hệ thống BE. (banking, payment partner(Stripe, Momo, ZaloPay, v.v))

![Orverview](/img/ipn_order.png "Orverview system")

## Source code

Sử dụng Clean Architecture cho dự án.

![Orverview](/img/source_code.png "Orverview system")

Ở phạm vi của bài demo này thì tôi đang áp dụng Aspire vào hệ thống.

- Domain: Entities, Enums
- Application: Business logic, Use Cases, DTOs
- Infrastructure: Persistence, Services, Implementations
- PaymentGateway.Api: Controllers, Filters, DI.
- PaymentGateway.Worker: Consumer worker, phân phối message, notity đến client.
- WebClient: Giả định web client cho người dùng.
- PaymentGateway.AppHost: Quản lý và điều phối các service
- PaymentGateway.ServiceDefaults: Cấu hình các dịch vụ.

## Triển khai

Run trên Aspire AppHost hoặc build and run on Kubernetes system.

![Orverview](/img/aspire_run.png "Orverview system")

![Orverview](/img/aspire1.png "Orverview system")

![Orverview](/img/aspire2.png "Orverview system")

Demo. [here](./img/demo.mp4)

## Những kỹ thuật trong dự án.

- SQL Server: Lưu trữ database.
- EntityFrameworkCore: Kết nối và xử lý với database.
- RabbitMQ: Phân phối message, data.
- SignalR: Kết nối client, server để giao tiếp.
- Blazor: Fake web-client.

## Thời gian thực hiện

- Thiết kế hệ thống: Phân tích yêu cầu, lựa chọn kiến trúc, công nghệ, vẽ thiết kế: 6h. (done)
- Thực hiện triển khai:
  - Code base: 3h (done)
  - CRUD Order: 3h (done)
  - Notify service: (pending)
  - Publish/Consumer to RabbitMQ: 2h (done)
  - Build client web app: 6h (done)
    - Tích hợp SignalR
    - Sử dụng Blazor đê hiển thị giao diện.
  - Tích hợp với Aspire: 4h (done)
