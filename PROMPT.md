# PROMPT.md — Oyez working state

This file holds the plan and current state of the work, so a new session can pick up where the last one stopped. Read it first, then read the documents it points to.

## What Oyez is

Oyez is a small C# tool that replaces Buffer for posting Brighter release announcements to X, Mastodon, LinkedIn and Write.as. You write an announcement once, as segments that each fit the smallest limit (X), and Oyez merges them into larger posts for the platforms with higher limits. See [`intent.md`](specs/0001-oyez/intent.md) for the full background.

## Plan

We work step by step. Each step ends with the user reviewing the output before we start the next step.

| # | Step | Output | Status |
|---|------|--------|--------|
| 1 | Intent: capture the problem, goals and findings from the initial discussion | `specs/0001-oyez/intent.md` | Done |
| 2 | Requirements: functional, non-functional and acceptance criteria, derived from the intent | `specs/0001-oyez/requirements.md` | Done |
| 3 | Design: a C# design that meets the requirements | `docs/adr/0001-oyez-design.md` (ADR 0001) | Done |
| 4 | Tasks: an ordered, test-first task list; each task names the test that proves it is done | `specs/0001-oyez/tasks.md` | Done |
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
- Pace: stop after each task for the user's review until they're happy with the approach, then work a phase at a time. **Current pace: a phase at a time** (from 2026-10-24, after T-3).

## Review gear

`specs/0001-oyez/.current-gear` → **review-after**, spec-wide, driver `ralph`. Downshift with `/spec:gear review-before`.

The user approved T-2 and T-3 test by test, then (2026-10-24) asked to move a phase at a time with sub-agents. Run a phase with `/spec:ralph-implement`: the main agent (Opus) picks each task in order and hands its Red→Green→Refactor cycle to a fresh Sonnet sub-agent, which never commits or edits `tasks.md`; the main agent makes the change commit and then the `docs:` tick. At the end of a phase, run `/refactor:smells`, make the fixes as `refactor:` commits, and the user reviews the phase as a batch before its PR. Only the per-test approval pause is removed: RED first, the full suite before every commit, the two-commit shape and every convention in `CLAUDE.md` still apply.

The gear file is untracked (`.gitignore`), so a fresh clone has no gear file and resolves to review-before; recreate it with `/spec:gear review-after` if this section says so.

Layout: Brighter's spec layout — the spec in `specs/0001-oyez/` (`specs/.current-spec`), designs as ADRs in `docs/adr/`, code in `src/`, tests in `tests/`, user documentation in `README.md` and `docs/`. Agent instructions in `.agent_instructions/` and commands in `.claude/commands/`, copied from Brighter.

## Current state

- Step 1 is done. The user's decisions are recorded in `intent.md` under "Decisions".
- Step 2 is done. The user approved `requirements.md` and was keen on FR-7.4/7.5 (drafting announcements from BrighterCommand releases); the design covers it as phase 2.
- Step 3 is done. The user approved `design.md`, including Brighter for commands and Darker for queries (§4.10). Package names and versions were checked on NuGet on 2026-10-22 (§10); Darker's DI registration lives in `Paramore.Darker.AspNetCore`. The design also gained an `oyez check` command for testing credentials without posting.
- Step 4 is done. The user approved `tasks.md` (T-0 to T-40, in 8 phases).
- Step 5 has started. Phase 0 (T-0, T-1) is done and merged to `main` (PR #1). An early `README.md` was added afterwards in its own PR. T-38 updates it once Oyez is built.
  - T-0: solution skeleton. `AddOyezApplication()` in `src/Oyez.Core/Application/ServiceCollectionExtensions.cs` registers Brighter and Darker. A spike command and query (`Spike.cs`) prove the wiring and are removed in T-17. Tests run on Microsoft Testing Platform.
  - T-1: `ci.yml` (build, test, actionlint, SHA-pinned actions) and `dependabot.yml`.
- `README.md` and an MIT `LICENSE` came in PR #2 (branch `add-readme`), which is merged.
- Phase 1 is in progress on branch `phase-1-core-domain`, which was branched from `add-readme`. The user asked for Brighter's conventions, so:
  - `CLAUDE.md` was added;
  - the design gained module boundaries and `IAmA…` port names;
  - phases 1–3 of `tasks.md` were restructured so tests go through the module boundaries.

  A first attempt at T-2, which tested the parser directly, was discarded without being pushed.
- T-2 is done: the `PreviewAnnouncement` query, with an internal parser and renderer. Tests share a never-disposed `NullLoggerFactory`, because Darker captures a logger factory in static state.
- After T-2, a code-smell review (a sub-agent following Brighter's `.claude/commands/refactor/smells.md`) found 15 issues. Six were fixed as `refactor:` commits:
  1. licence headers;
  2. `GenerateDocumentationFile` on for `src/` (via `src/Directory.Build.props`), so missing XML docs fail the build;
  3. every Darker query handler in Core, public or internal, is registered by reflection;
  4. header keys are constants;
  5. three test smells;
  6. `ChannelId.TryParse`.

  The rest are recorded in `tasks.md`: under T-3, at the start of T-4 (the `IAmAChannelProfile` strategy refactor), and in the backlog.
- T-3 is done: the `ValidateAnnouncements` query, and `PreviewAnnouncement` returning failures (including for an announcement that doesn't exist). The T-3 code-smell findings are resolved: an internal `AnnouncementHeader` owns the known keys and reports its own errors, unknown channels are errors, and the preview handler no longer uses `!` or an unchecked `.Value`. The user also approved one extra test, `When_previewing_a_malformed_announcement_should_return_its_validation_errors`. Publish times must be strict ISO 8601 (`yyyy-MM-ddTHH:mm[:ss[.fff]]` with `Z` or an offset).
- T-4 to T-11 are done, in the review-after gear, by Sonnet sub-agents. Phase 1's tasks are complete (103 tests). The T-4 refactor (`IAmAChannelProfile`) came first, in its own commit.
- The Phase 1 code-smell review (T-3 to T-11) found 13 code smells and 3 test smells. Ten were fixed as `refactor:` commits. Two are deferred: test boilerplate to T-12, and profiles' options lookup to T-28. The user chose: braces on any multi-line control statement; ALL_CAPS for static readonly lookup tables (both in `CLAUDE.md`); and moving `UsDollars` to `Costing`. `design.md` was brought in line with the code.
- The review also found five behaviour gaps. The user made them tasks T-11a to T-11d, at the end of Phase 1, and all four are done (128 tests). T-11c's sub-agent wrote production code before its tests; RED was recovered by running the tests against the previous code, as its commit records. The gaps were: an announcement naming only disabled channels; an empty `channels:` value; duplicate header keys; the hand-typed emoji table (keycaps miscount on X; © and ® unverified); and no general post-over-its-channel's-limit check.
- Phase 1 is merged to `main` (PR #3).
- Brighter's tooling is being imported on branch `import-brighter-tooling` (the user chose this on 2026-10-24): `.agent_instructions/` and the `spec`, `bugfix`, `refactor`, `tdd`, `adr` and `test-infra` commands, copied unchanged from BrighterCommand/Brighter@be197af in one commit and adapted to Oyez in the next (so a later sync can diff against upstream). `box_provisioning.md`, `generated_tests.md` and `.claude/settings.json` were left out. The planning documents moved into `specs/0001-oyez/`, with all three approval markers, and `design.md` became ADR 0001 (`docs/adr/0001-oyez-design.md`, with `docs/adr/index.md` generated from its frontmatter).
- The GitHub repo `iancooper/Oyez` was made public on 2026-10-19 (it was still empty), so Actions minutes are free.

## Next action

Finish the tooling import, ask before pushing, and open its PR. Then start Phase 2 (T-12) with `/spec:ralph-implement`, in the review-after gear; T-12 begins with the deferred test-boilerplate refactor.

Open points for the user, from the Phase 1 review: error messages name channels by ID (`x`, `linkedin`), so a display name may be wanted; © ® ™ count 2 on X following Unicode's data, unconfirmed by twitter-text's conformance file.
