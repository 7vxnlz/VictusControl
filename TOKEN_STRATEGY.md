# Token Strategy

This policy keeps VictusControl development low-token, reproducible, and compatible with ChatGPT, Codex, GitHub Copilot, Cursor, and future assistants.

## Budgets

- Default context budget: 8k-20k tokens
- Routine maximum: 35k tokens
- Investigation maximum: 60k tokens with justification
- Normal file count limit: 12 files
- Large task file count limit: 25 files with justification
- Reference file limit: 1-5 files per task
- Whole repository rule: forbidden unless the user explicitly approves it for a rare architecture audit

## Context Selection Order

1. Read workflow files.
2. Check current git diff if relevant.
3. Search locally with `rg` or symbol tools.
4. Select exact files.
5. Read only selected files.
6. Use Repomix only after selection.

## Git Diff

Use `git diff` when reviewing current edits, continuing interrupted work, preparing commits, or debugging regressions from recent changes. Prefer diffs over full files when the unchanged context is not needed.

## Repomix

Use Repomix when:

- more than four files must be reviewed together
- context needs to be portable between assistants
- token counts are needed before sharing context
- a task spans multiple modules

Do not use Repomix when:

- one or two files are enough
- symbol search can answer the question
- the pack would include generated files, binaries, logs, or references
- the user only needs a status answer

## Stacklit

Use Stacklit after the repository has meaningful structure. It is best for module-level orientation, ownership discovery, and compact repo maps. Regenerate it after major structural changes. Do not treat it as live source truth.

## Serena MCP

Use Serena MCP when available for symbol definitions, references, call graphs, and large-service navigation. Always read the target files before editing.

## Reference Repositories

Use reference repositories only when the user asks or when a task requires external behavior comparison. Never include reference repositories in default context packs.

## Model Scaling Policy

- Terra Low: routine edits, docs, small tests, formatting, straightforward bug fixes
- GPT-5.5 Low: small feature work, focused investigations, simple refactors
- GPT-5.5 Medium: multi-file changes, architectural tradeoffs, failing build/test diagnosis
- GPT-5.5 High: risky behavior changes, concurrency, safety-sensitive logic, release review
- GPT-5.6 High: major architecture decisions, deep cross-module debugging, high-risk hardware workflows

Use the smallest model that can do the job well. Scale up for uncertainty, risk, or cross-module reasoning, not for habit.
