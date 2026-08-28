# VictusControl AI Agent Rules

VictusControl is a future open-source C#/.NET Windows utility for the HP Victus Gaming Laptop 16-s0035nt / 7Z5Z2EA. These rules apply to Codex, ChatGPT, Cursor, GitHub Copilot-style agents, and future AI coding assistants.

## Project Boundaries

- Work inside the VictusControl product repository only.
- Treat sibling `reference/` or `references/` folders as read-only research material.
- Do not copy an entire external repository into VictusControl.
- Do not create application code, solution files, or hardware logic unless the user asks for implementation.
- Keep changes small, reviewable, and directly tied to the current request.

## Read Order

Before making any code change, read:

1. `AGENTS.md`
2. `AI_CONTEXT.md`
3. `SESSION_STATE.md`
4. `TOKEN_STRATEGY.md`
5. `CONTEXT_RECIPES.md` for the task type
6. `REFERENCE_POLICY.md` only when reference repositories are relevant

## Allowed Actions

- Search the local repository.
- Read selected files.
- Make scoped edits requested by the user.
- Add or update focused tests with behavior changes.
- Run relevant build/test commands when an application exists.
- Update `SESSION_STATE.md` at the end of meaningful work.

## Forbidden Actions

- Do not scan or send the whole repository by default.
- Do not modify reference repositories.
- Do not stage, commit, push, or create branches unless explicitly asked.
- Do not introduce unrelated refactors.
- Do not add hardware access, drivers, elevated operations, or vendor binaries without explicit direction.
- Do not include secrets, machine-specific paths, logs, or generated output in prompts.

## Search First

Use local search before broad reading. Prefer `rg` or symbol search. Read exact files only after locating likely definitions, call sites, tests, or configuration.

## Context Discipline

- Default context should fit in 8k-20k tokens.
- Routine tasks should stay under 12 files.
- Large investigations should stay under 25 files and require a reason.
- Use `git diff` for current changes instead of resending full files when possible.
- Use Repomix only for selected files/directories, never as a lazy whole-repo dump.

## GitHub Desktop Workflow

Assume the maintainer uses GitHub Desktop for Git operations. AI assistants should leave commits to the maintainer unless explicitly asked. When asked to prepare a commit, keep it atomic and summarize changed files clearly.

## Build And Test Expectations

When application code exists, run the narrowest useful verification:

- build for project or solution changes
- focused tests for logic changes
- broader tests for shared contracts or safety-sensitive behavior

If no application exists yet, do not run application builds.

## File Editing Rules

- Preserve user changes.
- Read files before editing them.
- Keep edits minimal.
- Avoid boilerplate unless requested.
- Prefer ASCII unless the file already uses another character set.
- Add comments only where they clarify non-obvious intent.

## Reference Repository Rules

Reference repositories are excluded from default context. Inspect them only when the user asks or when a task cannot be answered from VictusControl. Record upstream commit SHAs when reference behavior informs a decision.

## Hardware Safety Rules

Hardware control changes require extra caution. Use capability detection, safe defaults, explicit bounds, restore-to-auto behavior, and verification. Never assume one HP model behaves like another.

## Session State

Update `SESSION_STATE.md` after meaningful work. Keep it short. It is a handoff note, not a diary.
