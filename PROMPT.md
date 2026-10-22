# PROMPT.md — Oyez working state

This file holds the plan and current state of the work, so a new session can pick up where the last one stopped. Read it first, then read the documents it points to.

## What Oyez is

Oyez is a small C# tool that replaces Buffer for posting Brighter release announcements to X, Mastodon, LinkedIn and Write.as. You write an announcement once, as segments that each fit the smallest limit (X), and Oyez merges them into larger posts for the platforms with higher limits. See `intent.md` for the full background.

## Plan

We work step by step. Each step ends with the user reviewing the output before we start the next step.

| # | Step | Output | Status |
|---|------|--------|--------|
| 1 | Intent: capture the problem, goals and findings from the initial discussion | `intent.md` | Done |
| 2 | Requirements: functional, non-functional and acceptance criteria, derived from the intent | `requirements.md` | Done |
| 3 | Design: a C# design that meets the requirements | `design.md` | Done |
| 4 | Tasks: an ordered, test-first task list; each task names the test that proves it is done | `tasks.md` | Done |
| 5 | Code: implement the tasks in order, test first | `src/`, `tests/` | In progress |
| 6 | Documentation: user documentation for setup and use | `README.md`, `docs/` | Not started |

## Conventions

- **Test first.** Every task in `tasks.md` names the test that proves it is done, and we write that test before the code. What counts as a test depends on the task: a unit test, an integration test against a fake HTTP server, a manual check against a real account, or a review checklist for documents.
- **One task at a time.** Mark a task done in `tasks.md` only when its test passes. Commit at the end of each task, naming the task ID. Update this file when a step or task changes status.
- **Layout.** Planning documents live at the repo root, code in `src/`, tests in `tests/`, and user documentation in `README.md` and `docs/`.

## Current state

- Step 1 is done. The user's decisions are recorded in `intent.md` under "Decisions".
- Step 2 is done. The user approved `requirements.md` and was keen on FR-7.4/7.5 (drafting announcements from BrighterCommand releases); the design covers it as phase 2.
- Step 3 is done. The user approved `design.md`, including Brighter for commands and Darker for queries (§4.10). Package names and versions were checked on NuGet on 2026-10-22 (§10); Darker's DI registration lives in `Paramore.Darker.AspNetCore`. The design also gained an `oyez check` command for testing credentials without posting.
- Step 4 is done. The user approved `tasks.md` (T-0 to T-40, in 8 phases).
- Step 5 has started. Current task: T-0.
- The GitHub repo `iancooper/Oyez` was made public on 2026-10-19 (it was still empty), so Actions minutes are free.

## Next action

Do T-0 in `tasks.md`. Work one task at a time, test first, and commit at the end of each task.
