---
name: use-case-template
description: Template and best practices for creating use cases (USECASES) in the IAM platform
---

# Skill: Use Case Template

This skill provides a standardized template for documenting use cases in the IAM Platform project.

## Use Case Structure

Each use case should be located in `USECASES/UC-XXX-descriptive-name.md` containing:

```markdown
# UC-XXX: [Use Case Title]

**Description**: [Brief description of the use case]

**Actors**:
- [Primary actor]
- [Secondary actors if applicable]

**Preconditions**:
- [Conditions required before execution]

**Postconditions**:
- [State of the system after completion]

**Main Flow**:

1. [Step 1]
2. [Step 2]
3. ...

**Alternative Flows**:

- **A. [Condition Name]**:
  - [Alternative steps]

**Security Requirements**:

- [SR-001: Description]
- [SR-002: Description]

**Business Rules**:

- [BR-001: Description]
- [BR-002: Description]

**Non-Functional Requirements**:

- [NFR-001: Description]
```

## Useful Commands

To create a new use case:

```bash
# From the project root
mkdir -p USECASES
cat > USECASES/UC-$(date +%Y%m%d)-name.md << 'EOF'
# UC-$(date +%Y%m%d): [Title]

**Description**:

**Actors**:

-

**Preconditions**:

-

**Postconditions**:

-

**Main Flow**:

1.

**Alternative Flows**:

- **A. [Name]**:

**Security Requirements**:

- SR-:

**Business Rules**:

- BR-:

**Non-Functional Requirements**:

- NFR-:
EOF
```

To list all use cases:

```bash
ls USECASES/*.md
```

To search across all use cases:

```bash
grep -r "pattern" USECASES/
```

## Conventions

- **IDs**: `UC-` followed by numeric sequence (e.g., 001, 002) or date for uniqueness.
- **Cross-references**: Use `UC-XXX` to link related use cases.
- **Rule Codes**: `BR-NNN` (Business Rules), `SR-NNN` (Security Requirements), `NFR-NNN` (Non-Functional).

## Example: Authentication

See `USECASES/UC-002-authentication.md` for reference.
