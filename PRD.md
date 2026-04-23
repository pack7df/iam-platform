# Product Requirement Document (PRD) - IAM Platform

## 1. Vision
The IAM Platform is a multi-tenant Identity and Access Management system designed to provide centralized authentication and authorization. It enables organizations (Tenants) to manage their users' identities once and provide seamless access (Single Sign-On) across multiple applications within their corporate ecosystem.

## 2. Core Concepts

### 2.1 Actors
- **System Administrator**: A global user belonging to the **System Tenant**. Responsible for platform-wide maintenance, tenant provisioning, and global configuration via the Technical Management App.
- **Tenant Administrator (Owner)**: A user responsible for a specific **Customer Tenant**. They use the **Management Console** to manage their tenant's applications, branding, roles, and user base.
- **End User**: A consumer belonging to a Customer Tenant. They authenticate via the IAM to access the applications licensed or owned by their Tenant.
- **Service Identity**: A non-human identity (API Key/Service Account) used for machine-to-machine integrations and programmatic access to the IAM APIs.

### 2.2 Tenant Interior Structure
A **Tenant** is a self-contained ecosystem that owns and manages the following entities:
- **Applications**: The various software services provided by the organization. Each application defines its internal authorization metadata:
    - **Resource Tree**: A hierarchical structure of objects belonging to the application (e.g., "Project A > Documents > File 1").
    - **Operations**: General types of actions available in the application (e.g., "Read", "Write", "Admin").
    - **Actions**: Specific permission definitions that bind an Operation to a Resource (e.g., "Allow 'Read' on 'Project A'").
- **Users**: The corporate identities belonging to the Tenant. A user is created once at the Tenant level and can access any of its Applications based on their assigned roles.
- **Roles**: Collections of permissions used to define access levels. Roles are defined at the Tenant level and can aggregate permissions from multiple Applications, facilitating cross-app access management.

### 2.3 Multi-Tenancy
- **Tenant**: The primary organizational unit and isolation boundary.
- **System Tenant**: A special, pre-existing tenant that manages the platform itself. It hosts the administrative tools required for the system's operation.
- **Customer Tenant**: Standard tenants created for organizations to host their own applications and users.
- **Isolation Policy**: Data, users, and configurations are strictly isolated.

### 2.2 Core System Application
The **System Tenant** hosts the primary application for the entire platform:
1. **Unified Management Console**: A single portal for all administrative tasks. 
    - **System Administrators** access global technical configurations, platform monitoring, and tenant provisioning.
    - **Tenant Administrators** access their specific tenant's settings, application registrations, and user management.
    - Access to features is strictly controlled by role-based permissions.

### 2.3 Applications and SSO
- **Application**: Software services or resources that belong to a specific Tenant.
- **Single Sign-On (SSO)**: Users registered within a Tenant can authenticate once to access all applications associated with that same Tenant, ensuring a unified login experience.

### 2.4 Identity Model
- **Tenant User**: An identity created at the Tenant level. There are three primary types of users:
    - **Human User**: Standard people (Administrators, Employees) who authenticate via credentials.
    - **Service User**: Non-human identities used for machine-to-machine API access.
    - **System User (Virtual)**: Internal identities managed by the platform, such as the **Anonymous System User**, used to perform and audit public-facing operations.
- **Registration Methods**:
    - **Self-Registration**: Via the IAM's own public forms.
    - **Invitation**: Via a direct link sent by a Tenant Admin.
    - **Application-Initiated**: An external Application can register a user directly via API. In this case, the user is automatically associated with the Tenant that owns the Application.

### 2.5 Entity Model (High-Level)
- **Tenant**: The root entity. Contains configuration, branding settings, and isolation metadata.
- **User**: Belongs to exactly one Tenant. Stores credentials, profile data, and status.
- **Application**: Belongs to exactly one Tenant. Contains OAuth2/OIDC metadata (Client ID, Secrets, Redirect URIs).
- **Role**: Defined within a Tenant. Can be assigned to Users to grant permissions across the Tenant's applications.
- **Permission**: A rule that defines an access decision for a specific **Action** and **Resource**. It is assigned to either a **User** or a **Role** and results in one of three outcomes:
    - **Allowed**: Explicitly grants access.
    - **Denied**: Explicitly restricts access (overriding any "Allowed" decision).
    - **Inherited**: Access is determined by the decision of the parent resource in the hierarchy.
- **Audit Log**: Global entity tracking administrative actions across all tenants for security compliance.

### 2.7 Authorization Engine Logic
- **Targeting**: Permissions can be applied directly to a **User** or through assigned **Roles**.
- **Absolute Deny Precedence**: The "Denied" decision is absolute. If the evaluation process results in a "Denied" at any level (local or inherited from any parent), it nullifies all "Allowed" decisions found across all roles or direct assignments.
- **Hierarchical Inheritance**: When a permission is set to **Inherited**, the engine evaluates the same Action on the parent Resource. This process continues recursively up to the root.
- **Default State**: If the final resolved decision is neither "Allowed" nor "Denied" (all nodes are inherited up to the root), the default outcome is **Denied** (Security by Design).
- **Programmatic Management**: Applications (via Service Identities) can perform automated permission and resource management through the IAM API, enabling dynamic security modeling within external software.

### 2.8 Mandatory User Context
- **Ubiquitous Accountability**: Every operation performed within the platform must be associated with a **User**. There are no truly "anonymous" actions at the system level.
- **System/Anonymous Users**: Public operations (such as self-registration or password resets) are executed on behalf of specialized **System Users** (e.g., `Anonymous_Public_User`).
- **Audit Consistency**: This ensures that every entry in the **Audit Log** always contains a valid `UserId`, facilitating security tracking and compliance.

### 2.9 Tenant Customization (Branding)
- Each Tenant can customize the user experience for their applications.
- **Branding Assets**: Support for custom logos, primary colors, and organization names on login/profile pages.
- **Domain Mapping**: (Future) Ability to use custom domains for authentication endpoints.

### 2.10 System Bootstrapping
- Upon initial deployment, the platform automatically initializes a default state to ensure immediate operability and accountability:
- **Default System Tenant**: A dedicated tenant created to host global management resources.
- **Platform Management Application**: The core administrative application. At bootstrap, it initializes its own **Resource Tree** and **Operations** to manage the platform itself.
- **Initial System Administrator**: A primary human user granted global administrative privileges via an explicit `Allowed` permission at the root of the system's Resource Tree.
- **Auditor Service User**: A non-human identity pre-configured with specific permissions required for logging, background processing, and initial registration flows.
- **Self-Hosting Policy**: The IAM uses its own Authorization Engine to manage internal platform security, ensuring consistency across the entire infrastructure.

### 2.11 Authentication & SSO Flow
- **Mandatory Tenant Context**: Authentication MUST always specify a target Tenant. There is no "global" or anonymous login. This rule applies to all applications, including the **Unified Management Console**.
- **Discovery Mechanism**: Tenants are identified via unique URLs, subdomains, or explicit identifiers during the login process to ensure the correct user pool is targeted.
- **Single Sign-On (SSO)**: Once a user is authenticated at the Tenant level, they receive a session token that permits access to any registered Application under that tenant, without requiring further credentials.
- **Strict Isolation**: A person belonging to multiple tenants will have unrelated credentials and sessions, as each login is isolated within its tenant's context.

## 3. High-Level Requirements

### 3.1 Functional
- **Tenant Management**: Ability to create and configure isolated tenants.
- **Application Registration**: Tenants can register multiple applications under their umbrella.
- **Application-initiated Registration**: Support for external applications to programmatically register users via API.
- **Centralized Authentication**: A unified login interface that handles authentication for all apps within a tenant.
- **Identity Isolation**: Mechanisms to ensure no data leakage or cross-access between different tenants.

### 3.2 Non-Functional
- **Scalability**: Support for a large number of tenants and high-frequency authentication requests.
- **Security**: Robust isolation at the database and application levels.
- **Interoperability**: Standardized protocols for applications to integrate with the IAM (e.g., OIDC/SAML patterns).
