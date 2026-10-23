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

`CLAUDE.md` holds the working conventions, which follow Brighter's agent instructions. In short:
- Test first, with an approval gate: write the tests, then stop for the user's approval before implementing.
- Tests are named `When_…_should_…`, one per file, and go only through each module's public boundary.
- Modules are narrow and deep. Parsing, counting, rendering and validation are internal to `Oyez.Core`, and are tested through its Darker queries and Brighter commands.
- Commits come in two steps: `feat:` or `test:` for the behaviour, then `docs:` to tick the task. Structural changes go in their own `refactor:` commits.
- Look for code smells, and refactor, at the end of each phase.
- One branch and PR per phase. Ask before pushing, merging or deleting branches.
- Pace: stop after each task for the user's review until they're happy with the approach, then work a phase at a time. **Current pace: task by task.**

Layout: planning documents at the repo root, code in `src/`, tests in `tests/`, user documentation in `README.md` and `docs/`.

## Current state

- Step 1 is done. The user's decisions are recorded in `intent.md` under "Decisions".
- Step 2 is done. The user approved `requirements.md` and was keen on FR-7.4/7.5 (drafting announcements from BrighterCommand releases); the design covers it as phase 2.
- Step 3 is done. The user approved `design.md`, including Brighter for commands and Darker for queries (§4.10). Package names and versions were checked on NuGet on 2026-10-22 (§10); Darker's DI registration lives in `Paramore.Darker.AspNetCore`. The design also gained an `oyez check` command for testing credentials without posting.
- Step 4 is done. The user approved `tasks.md` (T-0 to T-40, in 8 phases).
- Step 5 has started. Phase 0 (T-0, T-1) is done and merged to `main` (PR #1). An early `README.md` was added afterwards in its own PR. T-38 updates it once Oyez is built.
  - T-0: solution skeleton. `AddOyezApplication()` in `src/Oyez.Core/Application/ServiceCollectionExtensions.cs` registers Brighter and Darker. A spike command and query (`Spike.cs`) prove the wiring and are removed in T-17. Tests run on Microsoft Testing Platform.
  - T-1: `ci.yml` (build, test, actionlint, SHA-pinned actions) and `dependabot.yml`.
- `README.md` and an MIT `LICENSE` are in PR #2 (branch `add-readme`), which isn't merged yet.
- Phase 1 is in progress on branch `phase-1-core-domain`, which was branched from `add-readme`. The user asked for Brighter's conventions, so:
  - `CLAUDE.md` was added;
  - the design gained module boundaries and `IAmA…` port names;
  - phases 1–3 of `tasks.md` were restructured so tests go through the module boundaries.

  A first attempt at T-2, which tested the parser directly, was discarded without being pushed.
- T-2 is done: the `PreviewAnnouncement` query, with an internal parser and renderer. Darker only scans public handlers, so internal handlers are registered explicitly in `AddOyezApplication`. Tests share a never-disposed `NullLoggerFactory`, because Darker captures a logger factory in static state.
- The GitHub repo `iancooper/Oyez` was made public on 2026-10-19 (it was still empty), so Actions minutes are free.

## Next action

A code-smell review of the code so far is running, at the user's request, using Brighter's `/refactor:code-smells` command. Act on its findings in `refactor:` commits, then write T-3's tests and stop for approval.
