# Technical Stack - IAM Platform

This document defines the official technology stack and architectural patterns for the IAM Platform.

## 1. Backend
- **Framework**: .NET 9 (ASP.NET Core Web API)
- **Language**: C# 13
- **Architecture**: Domain-Driven Design (DDD) with Clean Architecture.
- **Patterns**:
  - **CQRS**: Using **MediatR** for command and query separation.
  - **Validation**: FluentValidation for input and business rules.
  - **Mapping**: Manual mapping or Mapperly for DTOs (avoiding AutoMapper complexity).

## 2. Frontend
- **Framework**: React 18+
- **Tooling**: Vite + TypeScript
- **State Management**: React Context or TanStack Query (depending on complexity).
- **Styling**: Vanilla CSS (focused on high aesthetics and performance).

## 3. Persistence & Infrastructure
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core (EF Core) with Npgsql.
- **Migrations**: EF Core Migrations.
- **Communication**: RESTful APIs (standard) and potentially gRPC for internal service-to-service communication.

## 4. Testing
- **Unit Testing**: xUnit + FluentAssertions.
- **Integration Testing**: WebApplicationFactory + Testcontainers (PostgreSQL).
- **Database Isolation**: Respawn for fast database cleanup between tests.
- **Mocking**: NSubstitute.
- **Frontend Testing**: Vitest + React Testing Library.
