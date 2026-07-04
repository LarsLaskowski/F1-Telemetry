---
name: publish-pr
description: Creates a branch, commits the current changes, pushes the branch, opens a pull request, and switches back to main. Use this when asked to publish local changes as a pull request.
---

Use this skill when the user wants the current local changes published to GitHub as a pull request.

All user-facing output you create — branch name, commit message, PR title and body — must be written in **English**, regardless of the language the user wrote in. Never mention Claude, Anthropic, "Claude Code", Copilot, or any other AI/assistant tooling in the PR title or body, and do not add any `Co-Authored-By` trailer, "Generated with" footer, session link, or other note attributing the work to an AI.

## Workflow

1. Inspect the repository state first with non-interactive Git commands:
   - confirm the current branch
   - review `git status --short --branch` and `git diff --stat`
   - confirm the `origin` remote exists
2. If there are no relevant local changes to publish, stop and say so plainly.
3. Choose or confirm a branch name based on the change. If the user already provided one, use it. Otherwise derive a short kebab-case branch name from the work, prefixed by type where it fits (`fix/`, `feat/`, `chore/`, `docs/`, `build/`).
4. Create and switch to the branch from the current base branch (`main` unless the user says otherwise).
5. Stage only the files intended for this change. Do not include unrelated modifications.
6. Create a non-interactive Git commit with a concise message that matches the repo history: subject line max 80 characters, no trailing period, not in the first person, body concise (3-5 sentences) when needed. Do not add any `Co-Authored-By` trailer or session link.
7. Validate before pushing:
   - Backend changes: `dotnet build F1Server.slnx`, `reihitsu-format --check <changed-path>` for every changed `.cs` file, and `dotnet test F1Server.Tests`.
   - Frontend changes (`F1ServerApp`): `cd F1ServerApp && npm run build`, and `npm test` if the change affects testable logic.
   - If validation fails, fix the cause before pushing.
8. Push the branch to `origin` with upstream tracking (`git push -u origin <branch>`).
9. Open the pull request (use `gh pr create` if the GitHub CLI is available, otherwise the equivalent GitHub MCP tool, e.g. `create_pull_request`):
   - base branch: `main`, unless the user explicitly requests a different base
   - title: concise English summary of the change
   - body: short English summary of what changed, following the repository's PR template if one exists
   - Do not add any attribution, "Generated with" footer, session link, or other note referencing an AI/assistant in the PR title or body.
10. After the pull request is created, switch back to the `main` branch.
11. Report the branch name and pull request URL clearly.

## Rules

- Prefer non-interactive commands only.
- Do not amend existing commits unless the user explicitly asks.
- Do not include unrelated modified files in the commit.
- If push or PR creation fails, stop and report the failure clearly instead of continuing as if it succeeded.
- If switching back to `main` would discard or conflict with uncommitted work, stop and explain the blocker.
- Invoking this skill is the explicit user approval for the commit, push, and PR-creation steps it performs — no further confirmation is needed for those specific actions.
