# Implementation Roadmap - IAM Platform

This roadmap outlines the approximate phases and tasks required to build the IAM Platform from scratch, following the "Spec-Driven" approach.

## Phase 0: Project Scaffolding
Initial project structure and environment setup.

### 0.1 Solution & Architecture
- [x] **T0.1.1: .NET Solution & Projects Setup**: Create Solution and Projects (Domain, Application, Infrastructure, API) following DDD layers.
- [x] **T0.1.2: Global Dependencies**: Install and configure base libraries (MediatR, FluentValidation, Serilog).
- [x] **T0.1.3: Testing Environment**: Setup xUnit, FluentAssertions, and Testcontainers for Integration Testing.

## Phase 1: Foundation & Core Logic
Focus on the "Engine" and the physical data structure. Every task must result in less than 500 lines of change.

### 1.1 Persistence Engine
- [x] **T1.1.1: Persistence Infrastructure Setup**: Config EF Core, Npgsql and implement `Tenants` table.
- [x] **T1.1.2: Base Repository & Unit of Work**: Implement generic repository pattern for simple CRUDs.
- [x] **T1.1.3: Core Identity Model**: `Users` table with multi-tenant isolation and soft-delete logic.
- [x] **T1.1.4: Applications & Resources Model**: `Applications` and hierarchical `Resources` tables.
- [x] **T1.1.5: Operations & Actions Model**: `Operations` (global/tenant) and `Actions` bridge table.
- [x] **T1.1.6: Roles & Permissions Model**: `Roles` and the central `Permissions` assignment table.
- [x] **T1.1.7: Audit Infrastructure**: `AuditLogs` table and base entity audit tracking logic.


### 1.2 Authorization Engine
- [ ] **T1.2.1: Authorization Domain Types**: Decision enums (Allowed/Denied/Inherited) and evaluation models.
- [ ] **T1.2.2: Resource Tree Traversal**: Recursive logic to resolve parent resources in the hierarchy.
- [ ] **T1.2.3: Rule Resolution Engine**: Logic for matching rules and applying "Absolute Deny Precedence".
- [ ] **T1.2.4: Evaluation Pipeline Integration**: Final engine assembly with "Security by Design" default deny.

### 1.3 System Bootstrapping
- [ ] **T1.3.1: System Initialization Service**: Logic for creating the default System Tenant and Core App.
- [ ] **T1.3.2: Initial Identity Seeding**: Provisioning the System Admin and Auditor Service User.
- [ ] **T1.3.3: Self-Hosting Permissions**: Granting root `ADMIN` operation to the initial System Admin.

## Phase 2: Identity & Authentication
Focus on the entry point for users.
- [ ] **Tenant Context Discovery**: Implement the logic to identify the target Tenant from the request (Subdomains/Slugs).
- [ ] **Public Registration Flows**: Build the self-registration process for Users (facilitated by the **Anonymous System User**).
- [ ] **Invitation System**: Logic for Tenant Admins to invite users via secure links.
- [ ] **Authentication Provider**: Build the login flow requiring mandatory Tenant context.
- [ ] **Session & Token Management**: Implement JWT/Session issuance with SSO capabilities across applications within a tenant.

## Phase 3: Management Backend (APIs)
Focus on the administrative capabilities.
- [ ] **Tenant Onboarding (Registration)**: Implement the business logic for creating new Customer Tenants.
- [ ] **Tenant & Application Management**: APIs for System Admins (Global) and Tenant Admins (Self-service).
- [ ] **Resource & Action Sculpting**: APIs to define and manage application-specific resource trees and operations.
- [ ] **Permission Provisioning**: APIs to assign complex permissions (User + Role "AND" logic).

## Phase 4: Unified Management Console (UI)
Focus on the User Experience for Administrators.
- [ ] **Authentication & Dashboard**: Secure login to the System/Customer tenant.
- [ ] **Configuration UI**: Interfaces for managing identity, resources, and branding.
- [ ] **Role & Permission Editor**: Visual tool to manage the hierarchical permissions.

## Phase 5: Audit & Compliance
Finalizing the accountability features.
- [ ] **Audit Log Explorer**: UI for admins to track system-wide or tenant-specific actions.
- [ ] **System Reports**: Automated visibility into platform health and user activity.

## Phase 6: Integration & SDKs
- [ ] **External Registration API**: Implement secure endpoints for external Applications to register users directly into their Tenant's pool via headless API.
- [ ] **Programmatic Authorization API**: Endpoints for applications to create resources and manage permissions dynamically.
- [ ] **Application Integration**: Documentation and SDKs for external applications to consume our IAM.
- [ ] **OIDC/SAML Compatibility**: (Future) Enabling standard protocol support.
