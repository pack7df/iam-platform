---
name: git-workflow
description: Complete guide for conventional commits, PUSH, and Pull Requests
---

# Git Workflow Skill

This skill provides best practices, conventions, and commands for working with Git, commits, PUSH, and Pull Requests.

## Branching Strategy

**⚠️ IMPORTANT: Branch Policy**

- **DEV branch**: All feature development, bug fixes, and changes start from `DEV`
- **MAIN branch**: Used only for deployment/production releases
- **Workflow**:
  1. Create feature branches from `DEV`
  2. Push and create PRs **targeting DEV**
  3. After QA/staging approval, merge DEV → MAIN for deployment
  4. Hotfixes: branch from `MAIN`, fix, PR to both `MAIN` and `DEV`

**Never push directly to MAIN. All code goes through DEV first.**

## Conventional Commits

Use the format:

```
<type>[optional scope]: <description>

[optional body]

[optional footer]
```

### Commit Types

| Type | Use | Example |
|------|-----|---------|
| `feat` | New feature | `feat(auth): add OAuth login` |
| `fix` | Bug fix | `fix(api): fix email validation` |
| `docs` | Documentation changes | `docs: update README` |
| `style` | Format, style (no code change) | `style: format with prettier` |
| `refactor` | Code restructuring | `refactor(utils): extract helper` |
| `perf` | Performance improvement | `perf(db): optimize query` |
| `test` | Tests | `test(api): add unit tests` |
| `build` | Build system changes | `build: update dependencies` |
| `ci` | CI/CD changes | `ci: add lint stage` |
| `chore` | Miscellaneous tasks | `chore: update .gitignore` |

### Full Examples

```
feat(auth): support for 2FA

- add endpoint to verify token
- update UI with verification code
- add integration tests

Closes #123
Refs #456
```

```
fix(api): fix memory leak in streaming

The issue occurred when processing large files without closing stream.
Now using defer to guarantee closure.

Closes #789
```

## Useful Git Commands

### Initial Setup

```bash
# Configure user
git config user.name "Your Name"
git config user.email "you@email.com"

# Set default editor
git config core.editor "code --wait"  # VS Code
git config core.editor "vim"          # Vim

# View config
git config --list
```

### Daily Work

```bash
# View status
git status
git status -sb  # short version

# View changes
git diff
git diff --staged
git diff HEAD~1  # view last commit

# Add changes
git add <file>
git add .                    # all changes
git add -u                   # only tracked files
git add -p                   # add by chunks

# Commit with inline message
git commit -m "feat: message"

# Commit with editor
git commit

# Amend (modify last commit)
git commit --amend -m "new message"
git commit --amend --no-edit  # only add files

# Discard changes
git checkout -- <file>     # discard working directory changes
git reset HEAD <file>      # unstage file
```

### Branches

```bash
# Create branch
git branch <name>
git checkout -b <name>      # create and switch
git switch -c <name>        # modern alternative

# Switch branch
git checkout <branch>
git switch <branch>

# List branches
git branch          # local
git branch -a       # all (including remote)
git branch -v       # with last commit

# Delete branch
git branch -d <branch>    # safe delete
git branch -D <branch>    # force delete

# Rename branch
git branch -m <new-name>
```

### Merge and Rebase

```bash
# Merge (preferred for PRs)
git checkout DEV
git pull origin DEV
git merge <feature-branch>

# Rebase (to clean history)
git checkout <feature-branch>
git rebase DEV

# Resolve conflicts
# 1. Edit conflicted files
# 2. git add <resolved-files>
# 3. git rebase --continue
# Or abort: git rebase --abort
```

### Stash

```bash
# Save temporary changes
git stash
git stash push -m "message"
git stash push -p  # by chunks

# List stashes
git stash list

# Retrieve
git stash apply      # apply without deleting
git stash pop        # apply and delete
git stash apply stash@{2}  # specific one

# Delete
git stash drop stash@{0}
git stash clear
```

### Git Log

```bash
# Basic log
git log --oneline

# Log with graph
git log --oneline --graph --all

# Log with details
git log -p            # with diff
git log --stat        # with statistics

# Custom log
git log --pretty=format:"%h - %an, %ad : %s" --date=short
```

### Reset and Revert

```bash
# Reset (¡careful!)
git reset --soft HEAD~1    # Only moves HEAD, keeps changes staged
git reset --mixed HEAD~1   # Default, changes unstaged
git reset --hard HEAD~1    # ¡DANGEROUS! discards all changes

# Revert (safe)
git revert HEAD            # Creates new commit that undoes changes
git revert <commit-hash>
```

### Pull Request Workflow

#### Size Guidelines

Before creating a PR, check the change size:

```bash
# View stats (lines added/removed)
git diff --stat DEV...
# or
git log --oneline --stat DEV...

# View full diff with line counts
git diff --shortstat DEV...
```

**Recommended PR sizes:**
- **Small**: < 100 lines changed (ideal for quick review)
- **Medium**: 100-400 lines changed (standard)
- **Large**: 400-1000 lines changed (requires careful planning)
- **Very Large**: > 1000 lines changed (consider splitting)

If your PR is **Large** or **Very Large**, split it into multiple smaller, logical PRs.

Include these stats in the PR description:

```markdown
### Change Size

- **Lines added**: X
- **Lines removed**: Y
- **Total changed**: Z

*(Generated with `git diff --shortstat DEV...`)*
```

---

### 1. Prepare Changes

```bash
# 1. Update DEV
git checkout DEV
git pull origin DEV

# 2. Create feature branch
git checkout -b feature/new-feature

# 3. Make changes and commits
git add .
git commit -m "feat(module): clear description"
```

### 2. Pre-Push Checklist (Mandatory)

**⚠️ CRITICAL: Before pushing, verify:**

```bash
# 1. Build/compile the project (no errors)
# Example for different languages:

# Node.js/TypeScript
npm run build
# or
yarn build

# Go
go build ./...

# Java/Maven
mvn clean compile

# Python
python -m py_compile $(git ls-files '*.py')

# Java/Gradle
./gradlew build -x test  # compile only

# Rust
cargo check
```

```bash
# 2. Run linter (no warnings)
npm run lint
# or
yarn lint
# or
golangci-lint run
# or
flake8 .
```

```bash
# 3. Run tests (all must pass)
npm test
# or
yarn test
# or
go test ./...
# or
pytest
# or
./gradlew test
# or
cargo test
```

```bash
# 4. Optional: Run type checker
npm run type-check
# or
tsc --noEmit
# or
mypy .
```

**All above commands must exit with code 0 (success) before pushing.**

### 3. Push and PR

```bash
# Push branch
git push -u origin feature/new-feature

# Then on GitHub/GitLab:
# - Create PR from feature/new-feature → DEV
# - Fill template:
#   ### Description
#   [What changes and why]
#
#   ### Type of change
#   - [ ] New feature
#   - [ ] Bug fix
#   - [ ] Breaking change
#
#   ### Checklist
#   - [ ] Tests added/updated
#   - [ ] Docs updated
#   - [ ] Self-review completed
#   - [ ] Pre-Push Checklist completed (build, lint, tests)
```

### 4. Update PR

```bash
# Make more changes
git add .
git commit -m "fix: fix edge case"
git push origin feature/new-feature

# Or modify last commit before push
git commit --amend -m "better message"
git push -f origin feature/new-feature  # force push (¡careful!)
```

### 5. Review and Merge

```bash
# Once PR approved:
# Option A (GitHub UI): Click "Merge pull request"
# Option B (CLI):
gh pr checkout <number>    # if using gh CLI
git checkout DEV
git pull origin DEV
git branch -d feature/new-feature  # delete local branch
git push origin --delete feature/new-feature  # delete remote
```

## Commit Templates for Common Cases

### New feature
```
feat(<module>): <concise description>

Detailed explanation optional.

- detail 1
- detail 2

Closes #<issue>
Refs #<issue>
```

### Hotfix
```
fix(<module>): <description>

Description of the problem caused.

Closes #<issue>
```

### Release
```
chore(release): version <semver>

- Update CHANGELOG
- Bump version

Automated release commit.
```

## Best Practices

### Commits

- **One conceptual change per commit**: Don't mix features and fixes
- **Imperative Messages**: "add" not "added", "fix" not "fixed"
- **Line < 72 characters**, body < 100
- **Use emojis optionally** for visual context:
  - ✨ `feat`: new feature
  - 🐛 `fix`: bug fix
  - 📝 `docs`: documentation
  - ♻️ `refactor`: refactoring
  - ⚡ `perf`: performance
  - ✅ `test`: tests

### Pull Requests

- **Size**: A PR should be <= 400 lines of changes for effective review
- **Clear description**: Explain the "what" and "why", not just the "how"
- **Self-review**: Review your own PR before requesting review
- **Request appropriate reviewers**: Who knows the code area

### Branches

- **Descriptive names**: `feature/login-oauth`, `fix/api-timeout`, `docs/update-api`
- **Common prefixes**:
  - `feature/` - new features
  - `fix/` - bug fixes
  - `hotfix/` - urgent production fixes
  - `docs/` - documentation
  - `chore/` - miscellaneous tasks

## Common Problem Solutions

### Malformed Commits

```bash
# Change last commit message
git commit --amend -m "new message"

# Rewrite multiple commits (interactive)
git rebase -i HEAD~3
# Replace "pick" with "reword" for messages
# or "squash" to combine commits
```

### Merge Conflicts

```bash
# 1. Stop at conflict
git status  # see conflicted files (<<<<<<<)

# 2. Edit files manually
# Remove markers and choose version

# 3. Mark as resolved
git add <files>

# 4. Continue
git merge --continue
# Or if rebase:
git rebase --continue
```

### Dangerous force push

```bash
# NEVER force push to shared branches (DEV, MAIN)
# Only on personal feature branches

# If unavoidable, coordinate with team:
git push -f origin feature/my-branch

# Prefer:
git push --force-with-lease  # safer, fails if someone else pushed
```

### Recover deleted commits

```bash
# View unreachable commits
git reflog

# Recover specific commit
git checkout -f new-branch <commit-hash>
```

## Quick Cheatsheet

```bash
# Status
git status -sb

# Quick commit
git add . && git commit -m "feat: "

# Last 5 commits
git log --oneline -5

# Staged diff
git diff --staged

# Change last commit
git commit --amend -m "new message"

# Discard file changes
git checkout --

# Push new branch
git push -u origin feature/name

# Clean merged branches
git branch --merged | grep -v "DEV\|main\|master" | xargs git branch -d

# Update all remote branches
git fetch --all --prune
```

## References

- [Conventional Commits](https://www.conventionalcommits.org/)
- [Git Documentation](https://git-scm.com/doc)
- [GitHub Flow](https://docs.github.com/en/get-started/quickstart/github-flow)
- [Atlassian Git Tutorials](https://www.atlassian.com/git/tutorials)
