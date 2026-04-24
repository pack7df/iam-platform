# Development Workflow & Rules

This document outlines the mandatory "Way of Working" for the IAM Platform development.

## 1. Code Standards (The "No-Else" Policy)
To maintain high maintainability and readability, the following rules are mandatory:
- **Avoid Indentation**: Use guard clauses and early returns to keep logic flat.
- **No `else` Statements**: If you find yourself writing an `else`, refactor using early returns or polymorphism.
- **Small Methods**: Methods should have a single responsibility. If a method exceeds 15-20 lines, split it into smaller private methods.
- **Strict Typing**: No `any` in TypeScript; use strong types and interfaces for all data structures in C#.

## 2. Methodology
- **TDD (Test-Driven Development)**: Write the failing test first, then the minimum code to pass it, then refactor.
- **Vertical Slices**: Implement features from DB to UI in a single flow, rather than layer by layer across the whole system.
- **SOLID Principles**: Always adhere to Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, and Dependency Inversion.

## 3. Git & Pull Requests
- **Conventional Commits**: Use `feat:`, `fix:`, `docs:`, `style:`, `refactor:`, `test:`, `chore:`.
- **Atomic Commits**: Each commit should represent one logical change.
- **Reviewable PRs**: 
  - Maximum **400 lines** of code change per PR.
  - If a task is larger, split it into smaller sub-tasks.
- **Branching**: Develop in branches from `DEV` (e.g., `feat/T1.1.1-persistence-setup`).

## 4. Documentation & Language
- **Technical English**: All code, comments, and documentation must be in technical English.
- **Self-Documenting Code**: Use descriptive names for variables and functions. Prefer clarity over brevity.
