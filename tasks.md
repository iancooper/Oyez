# Oyez — Tasks

These tasks build the design in `design.md`. They are ordered so that each one builds on the ones before it.

## How to work through them

1. **Write the test first.** Each task names a test that proves it is done. Write that test and watch it fail, then write the code to make it pass, then tidy up.
2. **One task at a time.** Tick a task (`[x]`) only when its test passes and the full suite is green.
3. **Commit at the end of each task.** Name the task ID in the commit message.
4. **Keep `PROMPT.md` current.** Update its "Current state" whenever a task is done, so a new session can carry on.

## Kinds of test

| Kind | What it is | Where it lives |
|---|---|---|
| **Unit** | xUnit tests of pure code, or of handlers with in-memory fakes | `tests/Oyez.Core.Tests` |
| **Adapter** | xUnit tests of an adapter against `FakeHttpMessageHandler`, a temporary folder, or a fake process runner | `tests/Oyez.Adapters.Tests` |
| **E2E** | The CLI run in-process against a temporary repository, with fake HTTP handlers registered through DI | `tests/Oyez.Cli.Tests` |
| **Review** | A checklist in the task, checked by reading the work. `actionlint` runs too, where it applies. | — |
| **Manual** | Steps run against real accounts or GitHub, with the expected result written down | — |

Requirement and acceptance-criteria IDs refer to `requirements.md`. § numbers refer to sections of `design.md`.

---

## Phase 0 — Foundation

### [ ] T-0 Solution skeleton, with Brighter and Darker wired up
- **Covers:** NFR-2, NFR-3, §2, §4.10, §10
- **Test (Unit):** `CompositionSpikeTests`
  - Register Brighter and Darker as in §4.10.
  - Send a test command through `CommandProcessor.SendAsync` and check that its handler set a `Result`.
  - Execute a test query through `QueryProcessor.ExecuteAsync` and check the value returned.
- **Work:**
  - Create `Oyez.slnx`, with `src/Oyez.Core`, `src/Oyez.Adapters` and `src/Oyez.Cli`, and the three test projects.
  - Add `global.json`, pinning SDK 10.0.4xx.
  - Add `Directory.Build.props`: `net10.0`, nullable enabled, warnings as errors, implicit usings.
  - Add `Directory.Packages.props` (central package management), with exact versions as checked on NuGet: Brighter 10.8.x, Darker 4.1.x, System.CommandLine 2.0.x, xunit.v3, and Microsoft.Extensions.TimeProvider.Testing.
  - Add `.gitignore` and `.editorconfig`.
- **Done when:** `dotnet build` and `dotnet test` pass from a clean clone. Remove the spike's test command and query once T-17 replaces them.

### [ ] T-1 CI workflow and Dependabot
- **Covers:** NFR-5, §8 (`ci.yml`)
- **Test (Review, then Manual):**
  - **Review checklist:**
    - `ci.yml` runs on `pull_request` and on `push` to `main`.
    - It declares `permissions: contents: read`.
    - Every action is pinned to a commit SHA, with the version in a comment.
    - The workflow sets up .NET from `global.json`, caches NuGet, and runs `dotnet build` and `dotnet test`.
    - `actionlint` runs as a step.
  - **Manual:** push a branch and open a PR. The CI check goes green.
- **Work:** add `.github/workflows/ci.yml` and `.github/dependabot.yml`, covering the `github-actions` and `nuget` ecosystems.
- **Done when:** both parts of the test pass.

---

## Phase 1 — Core domain (pure code)

### [ ] T-2 Parse an announcement
- **Covers:** FR-1.1–1.4, §3, §4.1
- **Test (Unit):** `AnnouncementParserTests`
  - **AC-1:** a file with a title and two segments, plus a Markdown body, parses into the title, both segments in order (numbered 1 and 2), and the body. Channels default to all four.
  - `publish` with `Z`, and `publish` with an offset, both give the correct UTC instant.
  - Header keys are case-insensitive. Blank lines and `#` comment lines in the header are ignored.
  - Whitespace around each segment is trimmed, and newlines inside a segment are kept.
  - CRLF line endings are normalised to `\n`, and text is NFC-normalised.
  - `channels: x, writeas` limits the targets to those two.
- **Done when:** the tests pass.

### [ ] T-3 Reject a malformed announcement
- **Covers:** FR-1.3, FR-5.2, NFR-7, AC-2
- **Test (Unit):** `AnnouncementParserErrorTests`. Each of these cases gives a `ValidationError` naming the announcement and the problem:
  - a missing title;
  - a `publish` time with no time zone;
  - a `publish` value that can't be parsed;
  - an unknown header key;
  - an unknown channel name;
  - an empty segment (such as `---` followed directly by `---`);
  - no segments at all;
  - a folder name that doesn't match `^[a-z0-9][a-z0-9-]*$`.

  Several problems in one file are all reported, not just the first.
- **Done when:** the tests pass.

### [ ] T-4 Find URLs
- **Covers:** FR-2.1, FR-2.2, §4.2
- **Test (Unit):** `UrlMatcherTests`
  - **Matches:** `https://github.com/BrighterCommand/Brighter/releases/tag/10.2.0`, `http://example.com`, and a URL followed by trailing punctuation (the `.` or `)` is excluded from the match).
  - **Bare domains, with X rules on:** matches `brighter.io`, `asp.net` and `www.example.com/path`.
  - **Not matched:** `Paramore.Brighter`, `v10.2.0`, `e.g.` and `file.cs`.
  - **With Mastodon rules:** bare domains aren't matched.
- **Work:** embed the IANA TLD list as a resource. Record the date of the list in a comment.
- **Done when:** the tests pass.

### [ ] T-5 Count characters for X
- **Covers:** FR-2.1, AC-3, AC-4
- **Test (Unit):** `XTextCounterTests`
  - **AC-3:** the URL plus 250 ASCII characters counts as 273.
  - **AC-4:** one emoji plus 10 ASCII characters counts as 12.
  - A family emoji made with ZWJ joins counts as 2.
  - A flag emoji counts as 2.
  - 10 CJK characters count as 20.
  - `é` written as a decomposed sequence counts as 1, after NFC.
  - Curly quotes (U+201C and U+201D) count as 1 each.
  - `asp.net` counts as 23.
- **Done when:** the tests pass.

### [ ] T-6 Count characters for Mastodon
- **Covers:** FR-2.2, AC-5
- **Test (Unit):** `MastodonTextCounterTests`
  - **AC-5:** a URL counts as 23, and `@someone@example.social` counts as 8.
  - An emoji counts as 1.
  - A bare domain counts at its literal length.
  - A local `@user` mention counts at its literal length.
- **Done when:** the tests pass.

### [ ] T-7 Prepare and count text for LinkedIn
- **Covers:** FR-2.3, FR-4.3.3, §4.5, AC-10
- **Test (Unit):** `LinkedInPreparationTests`
  - **AC-10:** `Brighter (v10) #dotnet` becomes `Brighter \(v10\) {hashtag|\#|dotnet}`. With `convertHashtags` off, it becomes `Brighter \(v10\) \#dotnet`.
  - Every reserved character is escaped: `\ | { } @ [ ] ( ) < > # * _ ~`.
  - The count is taken after preparation, in UTF-16 code units.
- **Done when:** the tests pass.

### [ ] T-8 Channel profiles and limits from configuration
- **Covers:** FR-2.4, NFR-9, §4.3, §7
- **Test (Unit):** `ChannelProfileTests`
  - Binding the `oyez.json` sample from §7 gives profiles with these limits: X 280, Mastodon 2,263, LinkedIn 3,000, Write.as none.
  - Each profile has the policy shown in the table in §4.3.
  - A channel with `enabled: false` gets no profile.
- **Done when:** the tests pass.

### [ ] T-9 Render each channel
- **Covers:** FR-3.1–3.5, FR-1.5, §4.4
- **Test (Unit):** `RendererTests`
  - **AC-7:** three segments of 200 characters with a limit of 450 give two posts. The first post is 402 characters.
  - **AC-8:** four segments of about 250 characters give X 4 posts, Mastodon 1, LinkedIn 1 and Write.as 1.
  - Two very short segments merge into one X post (the refined FR-3.4).
  - The separator counts towards the limit: at the limit exactly, merge; one over the limit, don't.
  - The order of segments is kept, and a segment is never split.
  - Rendering twice gives equal output and the same `Hash` (FR-3.5).
  - Write.as uses `writeas.md` when it's present, and otherwise the segments joined by blank lines.
- **Done when:** the tests pass.

### [ ] T-10 Validate an announcement
- **Covers:** FR-5.2, FR-4.3.2, §4.6
- **Test (Unit):** `ValidatorTests`
  - **AC-6:** a segment that counts as 281 for X fails, naming segment N. This applies even when `channels` leaves X out.
  - **AC-9:** segments totalling more than 3,000 characters fail with a LinkedIn overflow, unless LinkedIn isn't targeted.
  - Write.as fails if its title or body is empty.
  - A valid announcement gives no errors.
- **Done when:** the tests pass.

### [ ] T-11 Estimate the X cost
- **Covers:** FR-5.3
- **Test (Unit):** `XCostEstimatorTests`. With the prices from configuration, a 3-post thread where only post 1 contains a URL costs $0.20 + 2 × $0.015 = $0.23. A bare domain counts as a URL.
- **Done when:** the tests pass.

---

## Phase 2 — Application layer (handlers and the publishing workflow)

### [ ] T-12 Publication record model
- **Covers:** FR-6.1, §4.8
- **Test (Unit):** `PublicationRecordTests`
  - Round-trip the JSON from §4.8, checking that serialising and deserialising gives the same record.
  - The JSON is camelCase and indented.
  - Every state is supported: `partial`, `complete`, `indeterminate` and `failed`.
  - Loading a record that doesn't exist gives an empty record.
  - An `InMemoryPublicationRecordStore` is available for the tests that follow.
- **Done when:** the tests pass.

### [ ] T-13 Publish due announcements
- **Covers:** FR-7.1, FR-6.2, FR-6.3, §4.9
- **Test (Unit):** `PublishServiceTests` (part 1), using fake publishers, in-memory stores and a `FakeTimeProvider`
  - **AC-17:** of three announcements (one dated in the past, one in the future, one undated), only the past and undated ones are published.
  - **AC-16:** an announcement whose record shows every channel complete produces no publisher calls.
  - A 3-post X thread is published with correct reply chaining, and the record is saved after *each* post. A spy store counts the saves.
  - Only targeted channels that are enabled are published.
- **Done when:** the tests pass.

### [ ] T-14 Resume, and keep channels independent
- **Covers:** FR-4.5.1, FR-6.2, AC-14 (Core level), AC-15
- **Test (Unit):** `PublishServiceTests` (part 2)
  - **AC-15:** with post 1 recorded as done, only posts 2 and 3 are sent, and post 2 replies to post 1's recorded ID.
  - **AC-14 (Core):** LinkedIn returns `Rejected` and the others succeed. The report shows three channels published and LinkedIn failed, and the record shows LinkedIn as `failed`.
  - A `failed` channel is retried on the next run.
  - A `partial` channel whose rendering hash no longer matches fails, with a message telling you to resolve it, and makes no publisher calls.
  - A channel with a missing secret is reported as `Rejected("missing secret …")`, and the other channels carry on.
- **Done when:** the tests pass.

### [ ] T-15 The indeterminate state
- **Covers:** FR-6.5, NFR-6, AC-23 (first part)
- **Test (Unit):** `PublishServiceTests` (part 3)
  - A publisher returning `Indeterminate` sets the channel's state to `indeterminate`, with the reason, and stops that channel.
  - On the next run, the channel is skipped with a warning and gets no publisher calls.
- **Done when:** the tests pass.

### [ ] T-16 The `PublishDueAnnouncements` command and its events
- **Covers:** §4.10, FR-4.5.2, NFR-10
- **Test (Unit):** `PublishDueAnnouncementsHandlerTests`
  - Sending the command through a real `CommandProcessor` (built with the in-memory fakes) puts a `PublishReport` in `Result`.
  - The `PostPublished` and `ChannelPublishFailed` events reach their handlers.
  - `JobSummaryBuilder` produces the expected Markdown summary: a table of channels and their states, with links to new posts.
  - The record is saved *before* each event is published. A store and an event handler that record the order of calls check this.
- **Done when:** the tests pass.

### [ ] T-17 The `ResolveChannel` command
- **Covers:** FR-6.6, AC-23 (second part)
- **Test (Unit):** `ResolveChannelHandlerTests`
  - **Posted:** resolving with `--posted 123` appends post 123 and sets the state to `partial`, or to `complete` if it was the last post. The next publish run sends only the remaining posts, replying to 123.
  - **Not posted:** resolving with `--not-posted` sets the state to `failed`, and the next run retries.
  - Resolving a channel that isn't `indeterminate` gives an error.
- **Work:** remove the T-0 spike command and query.
- **Done when:** the tests pass.

### [ ] T-18 Darker queries: validate, preview and status
- **Covers:** FR-5.1, FR-5.2, FR-5.3, §4.10
- **Test (Unit):** `QueryHandlerTests`, run through a real `QueryProcessor`
  - `ValidateAnnouncements` returns the errors from every announcement, or from just the ones named.
  - `PreviewAnnouncement` returns the renderings with counts, limits and the X cost, and makes no publisher calls. A strict fake publisher that throws on any call checks this.
  - `GetPublicationStatus` returns each channel's state.
- **Done when:** the tests pass.

---

## Phase 3 — Adapters

### [ ] T-19 Test fake for HTTP, and the retry handler
- **Covers:** NFR-6, §5.1, AC-20 (handler level)
- **Test (Adapter):** `RetryHandlerTests`, using a new `FakeHttpMessageHandler` that records requests and returns scripted responses for each route
  - **Idempotent request:** a 503 followed by a 200 retries and succeeds. Three 503s give up after 3 retries. The backoff is checked with `FakeTimeProvider`.
  - **Non-idempotent create:** a 503 isn't retried. A timeout isn't retried and comes back as a recognisable "outcome unknown" signal.
  - Connection refused is retried for both kinds of request.
  - A 429 with `Retry-After: 5` waits and retries. A 429 with `Retry-After: 120` gives up.
- **Done when:** the tests pass.

### [ ] T-20 Keep secrets out of the logs
- **Covers:** FR-8.2, NFR-5, AC-19 (unit level)
- **Test (Adapter):** `RedactionTests`
  - `RedactingLoggingHandler` logs the method, host, path and status. It never logs `Authorization` or any other header.
  - A 4xx response body containing a registered secret is logged with the secret replaced by `***`.
  - `SecretRedactor` also redacts secrets in logged exception messages.
- **Done when:** the tests pass.

### [ ] T-21 OAuth 1.0a signer
- **Covers:** FR-4.1.2, §5.2
- **Test (Adapter):** `OAuth1SignerTests`
  - Using the inputs from X's "Creating a signature" documentation example, the signature matches the published value. A fixed nonce and timestamp are injected.
  - The JSON body isn't included in the signature base string.
  - The `Authorization` header is formatted and percent-encoded correctly.
- **Done when:** the tests pass.

### [ ] T-22 X publisher
- **Covers:** FR-4.1, AC-11
- **Test (Adapter):** `XPublisherTests`
  - **AC-11:** publishing a 3-post rendering through the fake sends three `POST /2/tweets` requests. The second has `reply.in_reply_to_tweet_id` set to the first post's ID, and the third to the second's.
  - The request carries an OAuth header and the JSON body shown in §5.2.
  - `data.id` maps to `Posted`, with the URL `https://x.com/ICooper/status/{id}`.
  - A 400 or 403 response maps to `Rejected`, with X's error detail.
  - A 503 response or a timeout maps to `Indeterminate` (AC-20).
  - `VerifyAsync` calls `GET /2/users/me`.
- **Done when:** the tests pass.

### [ ] T-23 Mastodon publisher
- **Covers:** FR-4.2, AC-12
- **Test (Adapter):** `MastodonPublisherTests`
  - **AC-12:** the second post sets `in_reply_to_id` to the first post's ID, and both posts set `visibility=public`.
  - A Bearer token is sent.
  - The `Idempotency-Key` header equals the key from the request, and the request is marked idempotent, so a 503 is retried.
  - `id` and `url` map to `Posted`.
  - `VerifyAsync` calls `verify_credentials`.
- **Done when:** the tests pass.

### [ ] T-24 LinkedIn publisher
- **Covers:** FR-4.3.1, FR-4.3.5, §5.4
- **Test (Adapter):** `LinkedInPublisherTests`
  - The headers include `LinkedIn-Version` from configuration and `X-Restli-Protocol-Version: 2.0.0`.
  - The body matches §5.4, with the author URN from configuration.
  - A 201 response maps to `Posted`, using `x-restli-id` and the feed URL.
  - A token expiring within 14 days logs a warning and still posts.
  - An expired token gives `Rejected("…run oyez auth linkedin")` and makes no HTTP call.
  - A timeout maps to `Indeterminate`.
  - `VerifyAsync` calls `userinfo`.
- **Done when:** the tests pass.

### [ ] T-25 Write.as publisher
- **Covers:** FR-4.4.1, AC-13
- **Test (Adapter):** `WriteAsPublisherTests`
  - **AC-13:** publishing sends one `POST /api/collections/ian-cooper/posts`, with the title and the Markdown body.
  - The `Authorization: Token …` header is set.
  - `data.slug` maps to `Posted`, with the URL `https://ian-cooper.writeas.com/{slug}`.
  - `VerifyAsync` calls `/api/me`.
- **Done when:** the tests pass.

### [ ] T-26 File-system stores
- **Covers:** FR-1.1, FR-6.1, FR-6.3, §5.6
- **Test (Adapter):** `FileSystemStoreTests`, using temporary folders
  - The announcement store lists folders, loads `announcement.txt` and `writeas.md`, and ignores dot-files such as `.drafted.json`.
  - The record store writes and reads `published.json`.
  - A failure while writing (a fake that throws after writing the temporary file) leaves the previous `published.json` intact.
- **Done when:** the tests pass.

### [ ] T-27 GitHub CLI adapter
- **Covers:** FR-4.3.4, FR-4.4.2, §5.7
- **Test (Adapter):** `GhCliTests`, using a fake process runner
  - `SetSecret` runs `gh secret set NAME`, with the value on standard input and *not* in the arguments.
  - `SetVariable` runs `gh variable set NAME --body VALUE`.
  - A non-zero exit from `gh` raises an error that includes `gh`'s error output.
- **Done when:** the tests pass.

---

## Phase 4 — Command-line interface

### [ ] T-28 Composition root and `oyez validate`
- **Covers:** FR-5.2, FR-8, §6, §7
- **Test (E2E):** `ValidateCommandTests`
  - In a temporary repository with `oyez.json` and two announcements (one valid, one with a 281-character segment), `oyez validate` exits with 2 and prints the bad announcement's ID and segment number.
  - With only valid announcements, it exits with 0.
  - A missing or invalid `oyez.json` exits with 3.
  - `validate` needs no secrets.
- **Done when:** the tests pass.

### [ ] T-29 `oyez preview`
- **Covers:** FR-5.1, FR-5.3, AC-18
- **Test (E2E):** `PreviewCommandTests`
  - **AC-18:** the text output lists each channel's posts, with `count/limit`, and the estimated X cost. A `FakeHttpMessageHandler` that fails on any request shows that no HTTP call was made.
  - `--format markdown` produces Markdown suitable for a PR comment. A golden-file test checks it.
  - `--channel x` shows only X.
- **Done when:** the tests pass.

### [ ] T-30 `oyez publish`, end to end
- **Covers:** FR-4.5, FR-6, NFR-10, AC-14, AC-19
- **Test (E2E):** `PublishCommandTests`, with fake HTTP for all four platforms and secrets in environment variables
  - **AC-14:** LinkedIn returns 500 every time. X, Mastodon and Write.as are published, LinkedIn is reported, and the exit code is 1.
  - `published.json` is written for each announcement.
  - With `GITHUB_STEP_SUMMARY` pointing to a temporary file, the summary is written to it.
  - **AC-19:** with verbose logging and a failing request, the captured output contains none of the secret values.
  - `--now` overrides the clock.
- **Done when:** the tests pass.

### [ ] T-31 `oyez status`, `resolve`, `new` and `check`
- **Covers:** FR-6.6, NFR-7, NFR-8, §6
- **Test (E2E):** `MaintenanceCommandTests`
  - `status` shows each channel's state.
  - `resolve <id> x --posted 123` updates the record, as in T-17.
  - `new brighter-10-3 --publish 2026-11-01T09:00Z` creates `announcements/2026-…-brighter-10-3/` with a template that passes `validate`, apart from the placeholder text.
  - `check` against the fakes reports each channel as OK or failed. It reports the days until the LinkedIn token expires, and exits with 1 if any channel fails.
- **Done when:** the tests pass.

### [ ] T-32 `oyez auth writeas`
- **Covers:** FR-4.4.2
- **Test (E2E and Manual):**
  - **E2E:** with a fake Write.as login endpoint and a fake `gh`, the command stores the token returned through `SetSecret("OYEZ_WRITEAS_ACCESS_TOKEN")`. A failed login exits with 1 and stores nothing.
  - **Manual:** run it against the real account. `gh secret list` then shows the secret.
- **Done when:** both parts of the test pass.

### [ ] T-33 `oyez auth linkedin`
- **Covers:** FR-4.3.4, AC-22
- **Test (E2E and Manual):**
  - **E2E:** with fakes for the browser launcher, the token endpoint, `userinfo` and `gh`:
    - the authorisation URL contains the client ID, the redirect URI, the scopes `openid profile w_member_social` and a random `state`;
    - a callback with the wrong `state` is rejected;
    - a successful exchange sets the secret and two variables. The expiry is calculated from `expires_in` using `FakeTimeProvider`.
  - **Manual (AC-22):** run it for real. Then `oyez check` reports that LinkedIn is OK, with about 60 days to expiry.
- **Done when:** both parts of the test pass.

---

## Phase 5 — Workflows and going live

### [ ] T-34 `preview.yml`
- **Covers:** FR-5.4, NFR-5, §8
- **Test (Review, then Manual):**
  - **Review checklist:**
    - It's triggered by `pull_request` with `paths: announcements/**`.
    - It declares `permissions: contents: read, pull-requests: write`.
    - Actions are pinned to SHAs.
    - It finds the changed announcements with `git diff`.
    - It runs `validate` and `preview --format markdown`.
    - It posts the comment, or updates the earlier one.
    - `actionlint` passes.
  - **Manual:** open a PR that adds a sample announcement. The PR gets a preview comment. Push a change, and the same comment is updated.
- **Done when:** both parts of the test pass.

### [ ] T-35 `publish.yml`
- **Covers:** FR-6.4, FR-7.2, FR-7.3, NFR-5, NFR-10, AC-21 (first part)
- **Test (Review):** checklist
  - **Triggers:** `push` to `main` on `announcements/**`, `schedule: '17 * * * *'`, and `workflow_dispatch`.
  - **Concurrency:** `concurrency: publish`, with `cancel-in-progress: false`.
  - **Permissions:** `permissions: contents: write`, and nothing else.
  - **Secrets and variables:** each one in §7 is mapped to an environment variable.
  - **Publish step:** runs `oyez publish`.
  - **Commit step:** an `if: always()` step commits `announcements/**/published.json` with `[skip ci]`, then runs `git pull --rebase` and pushes.
  - **Pinning:** actions are pinned to SHAs.
  - **Linting:** `actionlint` passes.
- **Done when:** the checklist passes. The workflow is proven for real in T-37.

### [ ] T-36 Set up accounts and credentials
- **Covers:** intent decisions 2–4, FR-4.x
- **Test (Manual):** `oyez check`, run locally with user secrets *and* through `workflow_dispatch` (a `check` job in `publish.yml`, or a small `check.yml`), reports all four channels OK.
- **Steps:**
  - **X:**
    - Create a developer account and app for `@ICooper`, with read and write permissions.
    - Generate the access token and secret, and buy the minimum amount of credits.
    - `GET /2/users/me` costs about $0.01 per check.
  - **Mastodon:** create an application on hachyderm.io with the `write:statuses` and `read:accounts` scopes.
  - **LinkedIn:**
    - Create an app linked to a LinkedIn Page, and add the "Sign In with OpenID Connect" and "Share on LinkedIn" products.
    - Register `http://localhost:8765/callback` as a redirect URL, then run T-33.
  - **Write.as:** run T-32.
  - **GitHub:** set the secrets with `gh secret set`.
- **Done when:** both `check` runs report every channel OK.

### [ ] T-37 Go live with the first real announcement
- **Covers:** AC-21 (second part), the intent's success criteria
- **Test (Manual):**
  1. Write the next Brighter release announcement in a PR, and check the preview comment.
  2. Merge it. Posts appear on X (as a thread), Mastodon, LinkedIn and Write.as.
  3. `published.json` is committed back by the workflow.
  4. The job summary lists the four URLs.
  5. A second manual run of `publish.yml` posts nothing.
  6. Record the time taken, from creating the file to merging. The target is under five minutes (NFR-8).
- **Done when:** all of these steps check out. Then cancel Buffer.

---

## Phase 6 — Documentation

### [ ] T-38 User documentation
- **Covers:** NFR-7, NFR-8, PROMPT step 6
- **Test (Review, then Manual):**
  - **Review checklist:** these documents exist and are accurate:
    - `README.md`: what Oyez is, a quick start, and links.
    - `docs/writing-announcements.md`: the format, the counting rules, previews, scheduling, and per-channel targeting.
    - `docs/setup.md`: setting up each platform and GitHub's secrets and variables, following T-36.
    - `docs/operations.md`: resolving an indeterminate channel, renewing the LinkedIn token, bumping `LinkedIn-Version`, updating X pricing, and the TLD list.
    - `docs/architecture.md`: a short tour of the ports and adapters and of the Brighter and Darker handlers, for readers using Oyez as a Brighter example.
  - **Manual:** in a fresh clone, follow `README.md` and `docs/writing-announcements.md` to create an announcement and preview it, without referring to any other source.
- **Done when:** both parts of the test pass.

---

## Phase 7 — Drafting from releases (phase 2 features)

### [ ] T-39 `oyez draft`
- **Covers:** FR-7.4, §4.10 (`DraftFromRelease`), §8
- **Test (Unit, then E2E):**
  - **Unit:** given a fixture of a GitHub release JSON response for `BrighterCommand/Brighter` 10.2.0, the handler creates an announcement folder:
    - its first segment has the release name and URL, and fits within 280 characters;
    - placeholder segments follow;
    - `writeas.md` holds the release notes, with a heading.
  - **E2E:** the command works against a fake GitHub API.
- **Done when:** the tests pass.

### [ ] T-40 `draft.yml`: poll releases and open PRs
- **Covers:** FR-7.4, FR-7.5 (handled by polling), §8
- **Test (Unit, Review, then Manual):**
  - **Unit:** release discovery against `announcements/.drafted.json`.
    - A new release is returned.
    - A release that's already drafted is skipped.
    - Prereleases are skipped, unless configuration includes them.
  - **Review:** the workflow checklist:
    - a daily `schedule` and `workflow_dispatch`;
    - `contents: write` and `pull-requests: write` permissions;
    - pinned actions;
    - the preview is included in the PR body, because a PR opened with `GITHUB_TOKEN` won't trigger `preview.yml`.
  - **Manual:** run `workflow_dispatch` for a real release. A PR appears with the draft and its preview.
- **Done when:** all three parts of the test pass.

---

## Backlog (Could, not scheduled)

- FR-1.6: text overrides for a single channel (`x.txt`, `linkedin.txt` and so on).
- FR-7.5: `repository_dispatch` from the BrighterCommand repositories, if polling turns out to be too slow.
- Link previews on LinkedIn: attach an article to the post. This needs media support, which is out of scope.
