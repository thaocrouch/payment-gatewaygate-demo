# Payment Gateway

Author: Thao Nguyen Van

Date update: 2025-08-22

Version: v1.0

## System Overview

The system supports users in paying invoices and sends email notifications when customers successfully complete payments. Administrators can check and manage orders.

### Functional Requirements

- Administrators can search/filter by order name.
- Customers receive an email when payment is successful.
- Push payment information to third-party systems.
- Generate invoices upon successful payment.

### Non-Functional Requirements

- Performance: APIs have a processing time of less than 400ms.
- Scalability: APIs can scale horizontally to handle large volumes of traffic.
- Reliability & Availability: The system is always in a ready state, with uptime > 99.9%, and has a backup system in case of failures.
- Security: System security is ensured using OAuth2/OIDC, with rate limiting configured based on client IP (max 10 requests to create new orders within 10 minutes).
- Flexibility: Can be deployed on various platforms, both On-Premises and Cloud.
- Maintainability: The system adheres to clean architecture standards, making it easy to upgrade and maintain. A logging system is implemented to support troubleshooting.

### Main Flow

1. Users log into the system.
2. Create a new order (enter order details, amount, available payment methods, etc.).
3. The system validates and creates the new order, displaying payment information to the user.
4. The user completes the payment for the order.
5. Upon successful payment, the system displays a success notification (or sends an email). If the payment fails, it displays failure information.
6. For successfully paid orders, the system records the transaction and forwards payment information to the internal system.
7. Users can check their orders on the system (status, errors if applicable).

## High-Level Architecture

The system uses clean architecture with the following components:

- API Gateway: Handles authentication and function routing.
- Order Service: Designed according to RESTful API standards, supporting interaction with various systems.
- Notification:
  - Email Service: Notifies users when they successfully pay for an order.
  - Internal Service: Sends order payment information to the internal system.
- Invoice Service: Handles invoice-related requests (printing invoices, updating, etc.).
- Logs Service: Records logs for applications.
- Message Broker Service: Distributes and delivers messages and events within the system.
- Cache (optional): If the system experiences high concurrent traffic for an extended period with poor performance, Redis cache is used to temporarily store transaction data, updating transactions to the database in batches.
- Logs Service: Uses OpenTelemetry for logging, monitoring, and tracing.

Overview of the system:

![Overview](/img/structure.svg "Overview system")

## Data Model & Schemas (use Dbdocs if time permits)

### Table Orders (used for OrderService)

- Id: (UUID), PrimaryKey.
- UserId: (UUID), User identifier in the system.
- Name: (string), Order name.
- Amount: (number), Order value.
- Note: (string), Order notes.
- PaymentMethod: (int), Payment method.
- PaymentSubType: (int), Type of payment method: one-time, recurring...
- Status: (int), Order status.
- CallbackUrl: (string), URL to redirect to the corresponding client (web: invoice details, mobile: deeplink or others).
- IpnUrl: (string), URL for recording and sending data between backend-backend external systems.
- ErrorCode: (int), Error code recorded during processing between systems.
- ErrorMessage: (string), Details of the error recorded from systems.
- ExpireDate: (datetime), Order expiration date.
- CreatedDate: (datetime), Order creation date.
- UpdatedDate: (datetime), Date of updates from systems.

### Table OrderRetry (Used for retry calls in case of errors when recording to the internal system)

- OrderId: (UUID), PrimaryKey.
- Attempts: (int), Number of retry attempts recorded.
- ErrorCode: (int), Error code from the internal system.
- ErrorMessage: (string), Error details.

### Messages Events

- OrderMessage events
  - Id: (UUID), Order ID (equivalent to OrderId).
  - UserId: (UUID), Customer identifier in the system.
  - Status: (int), Order status.

## APIs

APIs serving the system:

### Create New Order

![Overview](/img/create_order.png "Overview system")

### Update Order

![Overview](/img/update_order.png "Overview system")

### Delete Order

![Overview](/img/delete_order.png "Overview system")

### Search Orders

![Overview](/img/get_orders.png "Overview system")

### Order Details

![Overview](/img/detail_order.png "Overview system")

### IPN - Record order information between backend systems (banking, payment partners (Stripe, Momo, ZaloPay, etc.))

![Overview](/img/ipn_order.png "Overview system")

## Source Code

Uses Clean Architecture for the project.

![Overview](/img/source_code.png "Overview system")

For the scope of this demo, I am applying Aspire to the system.

- Domain: Entities, Enums.
- Application: Business logic, Use Cases, DTOs.
- Infrastructure: Persistence, Services, Implementations.
- PaymentGateway.Api: Controllers, Filters, Dependency Injection.
- PaymentGateway.Worker: Consumer worker, distributes messages, notifies clients.
- WebClient: Simulated web client for users.
- PaymentGateway.AppHost: Manages and orchestrates services.
- PaymentGateway.ServiceDefaults: Service configurations.

## Deployment

Run on Aspire AppHost or build and run on a Kubernetes system.

![Overview](/img/aspire_run.png "Overview system")

![Overview](/img/aspire1.png "Overview system")

![Overview](/img/aspire2.png "Overview system")

**Demo**: [here](./img/demo.mp4)

## Technologies Used in the Project

- SQL Server: Database storage.
- EntityFrameworkCore: Connects and interacts with the database.
- RabbitMQ: Distributes messages and data.
- SignalR: Connects client and server for communication.
- Blazor: Simulated web client.

## Implementation Timeline

- System Design: Requirement analysis, architecture selection, technology selection, design diagrams: 6h (done).
- Implementation:
  - Code base: 3h (done).
  - CRUD Order: 3h (done).
  - Notify Service: (pending).
  - Publish/Consumer to RabbitMQ: 2h (done).
  - Build client web app: 6h (done).
    - Integrate SignalR.
    - Use Blazor for interface display.
  - Integrate with Aspire: 4h (done).
