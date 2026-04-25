# Audit Strategy

This document outlines the architecture and strategy for auditing within the IAM Platform.

## Current Architecture

Auditing is currently implemented **synchronously** using an EF Core Interceptor (`AuditInterceptor`).

### Operational Flow
1. The interceptor captures changes in the `ChangeTracker` before they are persisted.
2. It generates audit records in the `AuditLogs` table within the same database transaction.
3. The system leverages `IUserContext` to attribute changes to a specific user.

### Advantages
- **Atomic Consistency**: Business changes and their audit trails are saved together, or not at all.
- **Simplicity**: No additional infrastructure (queues, workers, etc.) is required.
- **Decoupling**: By using interceptors, the `DbContext` and repositories remain free of auditing logic.

## Performance Considerations (Overhead)

The current approach adds an extra load to every write operation (Write Amplification). In high-concurrency scenarios, this could potentially become a bottleneck.

## Future Scalability Strategy

Should database performance be impacted by audit volume, the following evolutions are proposed:

### 1. Asynchronous Auditing (Recommended)
- Modify `AuditInterceptor` to send logs to a message bus (e.g., RabbitMQ, Kafka) instead of the database.
- Implement a background service to consume these messages and persist them.

### 2. Specialized Storage
- Move audit logs to a database optimized for high-volume writes and log searches (e.g., Elasticsearch, MongoDB, or a cloud service like AWS DynamoDB).

### 3. Change Data Capture (CDC)
- Use tools like Debezium to read PostgreSQL transaction logs (WAL) and generate audits externally to the application code.

---

> [!NOTE]
> Due to the decoupled design using interceptors, any change in this strategy will only require modifications in the infrastructure layer (`AuditInterceptor`), without impacting the domain or application logic.
