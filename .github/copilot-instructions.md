# Copilot Instructions

## Principles

- **KISS**: keep solutions simple and easy to understand.
- **YAGNI**: don't add functionality, config, or abstraction until it's needed.
- Migrations and refactors should be minimal and scoped to the task — avoid
  unrelated cleanup in the same change.

## Migrations checklist
*(Applies to schema changes, data transformations, or breaking changes to
message/API contracts.)*

- State intent and data impact in the PR description.
- Prefer backward-compatible changes; use feature flags for phased rollouts.
- Include a rollback plan and smoke-test instructions.

## PR expectations

- Small, single-responsibility PRs with clear commit messages and a linked
  issue.
- Include tests and update docs when behavior changes.
- Call out performance, security, or API compatibility impacts.

## Copilot prompt template

> Change: `<goal>`. Scope: `<files/dirs>`. Constraints: `<KISS/YAGNI, compat, tests>`.

Example: *Change: add retry on 5xx to the forwarder client. Scope:
`src/client/`. Constraints: KISS, no new config options, keep existing tests
passing.*

## Ownership

Reviewed by `@org/team`. Enforced via `.github/CODEOWNERS`.