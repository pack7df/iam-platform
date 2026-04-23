# Product Requirement Document (PRD) - IAM Platform

## 1. Vision
The IAM Platform is a centralized web application designed to manage identity and authorization for multiple tenants and their respective applications. It aims to eliminate security fragmentation by providing a single source of truth for authentication, access decisions, and security auditing.

## 2. Actors
- **System User**: Global administrator managing the platform across all tenants.
- **Tenant Administrator**: Client identity managing configuration and users within a single tenant.
- **Final User (End-User)**: Subject whose access is evaluated by application rules.
- **Service User**: Non-human identity for programmatic administrative access.
- **Consuming Application**: External system delegating authentication and querying access decisions.

## 3. Domain Model
- **Tenant**: Primary boundary for functional isolation.
- **User**: Identity holder (System, Tenant, or Service level).
- **Role**: Set of permissions within a tenant.
- **Application**: Resource container belonging to a tenant.
- **Resource**: Specific entity within an application (hierarchical structure support).
- **Operation**: Possible action on a resource (e.g., Read, Write, Delete).

## 4. Authorization Engine (Logic)
- **Matching**: Rule selection based on resource and operation requested.
- **Inheritance**: Upward traversal in the resource tree to resolve effective permissions.
- **Decision Precedence**: "Deny" takes precedence over "Allow". Default is "Deny".

## 5. Non-Functional Requirements
- **Security**: Centralized audit logs for all administrative changes.
- **Scalability**: High-performance evaluation of access decisions.
- **Isolation**: Strict multi-tenant data separation.
