# Implementation Roadmap - IAM Platform

This roadmap outlines the approximate phases and tasks required to build the IAM Platform from scratch, following the "Spec-Driven" approach.

## Phase 1: Foundation & Core Logic
Focus on the "Engine" and the physical data structure.
- [ ] **Infrastructure & Database**: Implement the relational schema (PostgreSQL) with multi-tenant isolation, soft-deletes, and auditing columns.
- [ ] **Authorization Engine**: Develop the core logic for recursive Resource Tree traversal and Permission resolution (Allow/Deny/Inherit logic).
- [ ] **System Bootstrapping**: Create the automated script to initialize the **System Tenant**, the **System Admin**, and the **Auditor Service User**.

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
