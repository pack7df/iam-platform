# Product Requirement Document (PRD) - IAM Platform

## 1. Vision
The IAM Platform is a multi-tenant Identity and Access Management system designed to provide centralized authentication and authorization. It enables organizations (Tenants) to manage their users' identities once and provide seamless access (Single Sign-On) across multiple applications within their corporate ecosystem.

## 2. Core Concepts

### 2.1 Actors
- **System Administrator**: A global user belonging to the **System Tenant**. Responsible for platform-wide maintenance, tenant provisioning, and global configuration via the Technical Management App.
- **Tenant Administrator (Owner)**: A user responsible for a specific **Customer Tenant**. They use the **Management Console** to manage their tenant's applications, branding, roles, and user base.
- **End User**: A consumer belonging to a Customer Tenant. They authenticate via the IAM to access the applications licensed or owned by their Tenant.
- **Service Identity**: A non-human identity (API Key/Service Account) used for machine-to-machine integrations and programmatic access to the IAM APIs.

### 2.2 Multi-Tenancy
- **Tenant**: The primary organizational unit and isolation boundary.
- **System Tenant**: A special, pre-existing tenant that manages the platform itself. It hosts the administrative tools required for the system's operation.
- **Customer Tenant**: Standard tenants created for organizations to host their own applications and users.
- **Isolation Policy**: Data, users, and configurations are strictly isolated.

### 2.2 Core System Applications
The **System Tenant** hosts two primary applications to manage the platform:
1. **Technical Management App**: Used by system engineers and global admins for technical configuration, infrastructure monitoring, and global platform maintenance.
2. **Management Console**: A self-service portal where customers (Tenant Admins) manage their own tenants, register applications, and administer their users.

### 2.3 Applications and SSO
- **Application**: Software services or resources that belong to a specific Tenant.
- **Single Sign-On (SSO)**: Users registered within a Tenant can authenticate once to access all applications associated with that same Tenant, ensuring a unified login experience.

### 2.4 Identity Model
- **Tenant User**: An identity created at the Tenant level. This user profile is shared across the tenant's application portfolio.
- **Registration**: Users are onboarded into a specific Tenant's user pool.

### 2.5 Entity Model (High-Level)
- **Tenant**: The root entity. Contains configuration, branding settings, and isolation metadata.
- **User**: Belongs to exactly one Tenant. Stores credentials, profile data, and status.
- **Application**: Belongs to exactly one Tenant. Contains OAuth2/OIDC metadata (Client ID, Secrets, Redirect URIs).
- **Role**: Defined within a Tenant. Can be assigned to Users to grant permissions across the Tenant's applications.
- **Audit Log**: Global entity tracking administrative actions across all tenants for security compliance.

## 3. High-Level Requirements

### 3.1 Functional
- **Tenant Management**: Ability to create and configure isolated tenants.
- **Application Registration**: Tenants can register multiple applications under their umbrella.
- **Centralized Authentication**: A unified login interface that handles authentication for all apps within a tenant.
- **Identity Isolation**: Mechanisms to ensure no data leakage or cross-access between different tenants.

### 3.2 Non-Functional
- **Scalability**: Support for a large number of tenants and high-frequency authentication requests.
- **Security**: Robust isolation at the database and application levels.
- **Interoperability**: Standardized protocols for applications to integrate with the IAM (e.g., OIDC/SAML patterns).
