# Oyez — Requirements

These requirements come from `intent.md`. Each one has an ID so that `design.md` and `tasks.md` can refer to it.

**Priority** uses MoSCoW:
- **Must** — needed before we can cancel Buffer.
- **Should** — wanted in the first release, but we can ship without it.
- **Could** — later.

## Glossary

| Term | Meaning |
|---|---|
| **Announcement** | One thing to say, such as "Brighter 10.2 is released". It's stored as a file in this repository. |
| **Segment** | One plain-text block of an announcement. Each segment fits X's 280-character limit when counted by X's rules. |
| **Channel** | One account on one platform. There are four: X `@ICooper`, Mastodon `@icooper@hachyderm.io`, LinkedIn (personal profile) and Write.as `ian-cooper`. |
| **Rendering** | The list of posts produced for one channel from an announcement. |
| **Publication record** | Oyez's record of what it has already posted for an announcement, on each channel. |

---

## Functional requirements

### FR-1 Announcement format

| ID | Priority | Requirement |
|---|---|---|
| FR-1.1 | Must | An announcement is a text file in the repository's `announcements/` folder. |
| FR-1.2 | Must | The announcement body is an ordered list of plain-text segments, separated by a delimiter that's easy to type. |
| FR-1.3 | Must | An announcement has a small metadata header containing at least: a title, an optional publish time (UTC), and an optional list of channels to target. If no channels are listed, the announcement goes to all four. |
| FR-1.4 | Must | An announcement can include a Markdown body for Write.as. |
| FR-1.5 | Should | If no Markdown body is given, the Write.as post is built from the segments, joined by blank lines. |
| FR-1.6 | Could | An announcement can override the text for any single channel. |

### FR-2 Character counting

| ID | Priority | Requirement |
|---|---|---|
| FR-2.1 | Must | Oyez counts characters for X the way X does. Every URL counts as 23 characters. Characters in X's standard ranges (most Latin text and punctuation) count as 1, and all other characters, including emoji and CJK, count as 2. Text is counted after Unicode NFC normalisation. |
| FR-2.2 | Must | Oyez counts characters for Mastodon the way Mastodon does. Every URL counts as 23 characters. A mention such as `@user@domain` counts only its `@user` part. Every other character, including emoji, counts as 1. |
| FR-2.3 | Must | Oyez counts characters for LinkedIn as plain characters, after escaping any reserved characters (see FR-4.3.3). |
| FR-2.4 | Must | Each channel's limit is set in configuration: X 280, Mastodon 2,263, LinkedIn 3,000. Write.as has no limit. |

### FR-3 Rendering (merging segments)

| ID | Priority | Requirement |
|---|---|---|
| FR-3.1 | Must | For each channel, Oyez merges adjacent segments greedily, in order. It starts a new post when adding the next segment, plus its separator, would exceed the channel's limit. |
| FR-3.2 | Must | Merged segments are joined with a blank line, and the separator counts towards the limit. |
| FR-3.3 | Must | Merging never splits a segment and never changes the order of segments. |
| FR-3.4 | Must | X follows the same merging rule as the other channels. Because each segment is written close to 280 characters, X normally gets one post per segment. If two short adjacent segments fit together, they are merged, which saves a post. |
| FR-3.5 | Must | Rendering is deterministic: the same announcement and configuration always produce the same posts. |

### FR-4 Publishing

**FR-4.1 X**

| ID | Priority | Requirement |
|---|---|---|
| FR-4.1.1 | Must | Oyez posts the rendering as a thread. The first post stands alone, and each later post replies to the post before it. |
| FR-4.1.2 | Must | Oyez authenticates as `@ICooper` with a user-context credential that GitHub Actions can use without anyone present. |

**FR-4.2 Mastodon**

| ID | Priority | Requirement |
|---|---|---|
| FR-4.2.1 | Must | Oyez posts the rendering to hachyderm.io as `@icooper` with public visibility. If the rendering has more than one post, it posts them as a thread. |
| FR-4.2.2 | Must | Oyez authenticates with an application access token. |

**FR-4.3 LinkedIn**

| ID | Priority | Requirement |
|---|---|---|
| FR-4.3.1 | Must | Oyez posts the rendering to the member's personal profile with public visibility, using the Posts API and the `w_member_social` scope. |
| FR-4.3.2 | Must | If the rendering has more than one post, Oyez fails validation, because LinkedIn doesn't support threads. |
| FR-4.3.3 | Must | Oyez escapes the characters that LinkedIn's commentary format reserves, such as `( ) [ ] { } < > @ # \ * _ ~ |`, so the post isn't truncated or mangled. |
| FR-4.3.4 | Must | Oyez provides a local command that runs the LinkedIn OAuth sign-in and stores the new access token as a GitHub Actions secret. |
| FR-4.3.5 | Should | Oyez warns when the LinkedIn token is within 14 days of expiry, and fails with a clear message once it has expired. |

**FR-4.4 Write.as**

| ID | Priority | Requirement |
|---|---|---|
| FR-4.4.1 | Must | Oyez publishes one post, with the announcement's title and Markdown body, to the `ian-cooper` collection. |
| FR-4.4.2 | Must | Oyez authenticates with a Write.as access token. It also provides a local command that obtains a token by logging in. |

**FR-4.5 All channels**

| ID | Priority | Requirement |
|---|---|---|
| FR-4.5.1 | Must | Oyez publishes to each channel independently. A failure on one channel doesn't stop the others. |
| FR-4.5.2 | Must | The exit code is non-zero if any channel fails, and the output names the failed channels. |

### FR-5 Preview and validation

| ID | Priority | Requirement |
|---|---|---|
| FR-5.1 | Must | A `preview` command (dry run) shows each channel's rendering, with character counts and limits, without posting anything. |
| FR-5.2 | Must | A `validate` command checks one announcement or all of them, and fails if: the format is wrong; any segment is over X's limit; a LinkedIn rendering has more than one post; or the Write.as title or body is missing (where no fallback applies). |
| FR-5.3 | Should | The preview shows the estimated X cost: $0.015 per post, or $0.20 if the post contains a URL. |
| FR-5.4 | Should | When a pull request adds or changes an announcement, GitHub Actions runs validation and posts the preview as a PR comment. |

### FR-6 Idempotency and partial failure

| ID | Priority | Requirement |
|---|---|---|
| FR-6.1 | Must | Oyez keeps a publication record for each announcement in the repository. The record lists, for each channel, the IDs or URLs of the posts already made. |
| FR-6.2 | Must | Oyez never republishes a post that the publication record shows as done. A rerun after a partial failure carries on from where it stopped, including midway through a thread. |
| FR-6.3 | Must | Oyez updates the publication record after every successful post, so a crash loses at most the post that was in progress. |
| FR-6.4 | Must | After a publish run, the GitHub Actions workflow commits the updated publication record back to the repository. |
| FR-6.5 | Must | If Oyez can't tell whether a post was created, it marks that channel **indeterminate** in the publication record. Later runs skip an indeterminate channel, with a warning, so it is never retried automatically. |
| FR-6.6 | Must | A `resolve` command clears an indeterminate channel, after someone has checked the platform. The person says whether the post was made, and gives its ID if it was. The next run then carries on from the correct point. |

### FR-7 Scheduling and triggers (GitHub Actions)

| ID | Priority | Requirement |
|---|---|---|
| FR-7.1 | Must | A `publish` command publishes every announcement that is due and not yet fully published. An announcement is due when its publish time has passed, or when it has no publish time. |
| FR-7.2 | Must | A GitHub Actions workflow runs `publish` when changes are pushed to `main`, on a cron schedule (hourly), and when started manually (`workflow_dispatch`). |
| FR-7.3 | Must | Publishing runs never overlap; the workflow uses a concurrency group. |
| FR-7.4 | Could | Oyez can draft an announcement from a GitHub release in a BrighterCommand repository. |
| FR-7.5 | Could | Publishing a release in a BrighterCommand repository triggers Oyez through `repository_dispatch` to draft an announcement in a PR. |

### FR-8 Configuration and secrets

| ID | Priority | Requirement |
|---|---|---|
| FR-8.1 | Must | Settings that aren't secret, such as limits, the instance URL, the collection alias, and which channels are enabled, live in a configuration file in the repository. |
| FR-8.2 | Must | Secrets come only from environment variables. Locally, they can also come from .NET user secrets. Secrets never appear in the repository or in log output. |

---

## Non-functional requirements

| ID | Category | Priority | Requirement |
|---|---|---|---|
| NFR-1 | Cost | Must | Running costs, including X API charges, stay under $25 a year at four announcements a month. There are no paid services apart from X credits. |
| NFR-2 | Platform | Must | Oyez is written in C# on .NET 10 (LTS). It's a console application that can also be installed as a `dotnet tool`. It runs on macOS and on Linux GitHub Actions runners. |
| NFR-3 | Dependencies | Should | Oyez uses as few third-party packages as practical. It talks to the platform APIs over `HttpClient` directly, rather than through SDKs for each platform. The exception is BrighterCommand's own libraries: Oyez deliberately uses them, so it also serves as a small working example of them. |
| NFR-4 | Testability | Must | Formatting, counting, rendering and validation are pure functions with unit tests. The code for each platform sits behind an interface and has integration tests against a fake HTTP handler. CI never calls a real platform. |
| NFR-5 | Security | Must | Tokens are never logged; the logging layer redacts them. The GitHub Actions workflow uses minimal `permissions` (`contents: write` only to commit the publication record). Third-party actions are pinned to commit SHAs. |
| NFR-6 | Reliability | Must | Oyez retries transient HTTP errors (5xx, timeouts) with backoff. On a 429 response, it honours `Retry-After` if it's 60 seconds or less, and otherwise fails that channel. It never retries a post-creation request when it can't tell whether the post was created. In that case it stops that channel and reports the problem. |
| NFR-7 | Usability | Must | Error messages name the announcement, the channel and the cause, and suggest a fix where one is known. |
| NFR-8 | Usability | Should | Writing an announcement and publishing it takes under five minutes. |
| NFR-9 | Maintainability | Should | Adding a new channel means implementing one interface and adding configuration, without changing the core. |
| NFR-10 | Observability | Should | Every publish run writes a summary to the GitHub Actions job summary, listing each channel's status and the URLs of new posts. |

---

## Acceptance criteria

Each criterion is written as Given, When, Then, and names the requirements it covers. `tasks.md` will turn these into tests.

**AC-1 Parse an announcement (FR-1.1–1.4)**
- **Given** an announcement file with a title, two segments and a Markdown body
- **when** it is parsed,
- **then** the result contains the title, both segments in order, and the Markdown body, and its channels default to all four.

**AC-2 Reject a malformed announcement (FR-1, FR-5.2)**
- **Given** an announcement with no title, or with an unparseable publish time,
- **when** it is validated,
- **then** validation fails with a message that names the file and the problem.

**AC-3 X URL counting (FR-2.1)**
- **Given** a segment containing `https://github.com/BrighterCommand/Brighter/releases/tag/10.2.0` and 250 other ASCII characters,
- **when** it is counted for X,
- **then** the count is 273 and the segment is within the limit.

**AC-4 X weighted counting (FR-2.1)**
- **Given** a segment containing one emoji and 10 ASCII characters,
- **when** it is counted for X,
- **then** the count is 12.

**AC-5 Mastodon counting (FR-2.2)**
- **Given** text that contains a URL and the mention `@someone@example.social`,
- **when** it is counted for Mastodon,
- **then** the URL counts as 23 characters and the mention counts as 8 (`@someone`).

**AC-6 Over-limit segment (FR-2.1, FR-5.2)**
- **Given** a segment that counts as 281 for X,
- **when** the announcement is validated,
- **then** validation fails and identifies the segment by number.

**AC-7 Greedy merge (FR-3.1–3.3)**
- **Given** three segments of 200 characters each, and a channel limit of 450,
- **when** the announcement is rendered for that channel,
- **then** there are two posts: segments 1 and 2 joined by a blank line (402 characters), then segment 3.

**AC-8 Renderings for each channel (FR-3.4, FR-3.5, FR-2.4)**
- **Given** four segments of about 250 characters each,
- **when** the announcement is rendered,
- **then** X gets 4 posts, Mastodon 1, LinkedIn 1 and Write.as 1. Rendering twice gives identical output.

**AC-9 LinkedIn overflow (FR-4.3.2)**
- **Given** segments that total more than 3,000 characters,
- **when** the announcement is validated,
- **then** validation fails with a message that the LinkedIn rendering doesn't fit in one post.

**AC-10 LinkedIn escaping (FR-4.3.3)**
- **Given** the text `Brighter (v10) #dotnet`,
- **when** it is prepared for LinkedIn,
- **then** the reserved characters are escaped and the length is counted after escaping.

**AC-11 X thread (FR-4.1.1)**
- **Given** a 3-post X rendering and a fake X API,
- **when** it is published,
- **then** the fake receives three create requests: the second replies to the first post's ID, and the third replies to the second's.

**AC-12 Mastodon thread (FR-4.2.1)**
- **Given** a 2-post Mastodon rendering and a fake Mastodon API,
- **when** it is published,
- **then** the second request sets `in_reply_to_id` to the first post's ID, and both requests set public visibility.

**AC-13 Write.as post (FR-4.4.1)**
- **Given** an announcement with a title and a Markdown body, and a fake Write.as API,
- **when** it is published,
- **then** one request is made to the `ian-cooper` collection endpoint, with that title and body.

**AC-14 Channels are independent (FR-4.5)**
- **Given** fake APIs where LinkedIn returns 500 every time and the others succeed,
- **when** the announcement is published,
- **then** X, Mastodon and Write.as are published, LinkedIn is reported as failed, and the exit code is non-zero.

**AC-15 Resume after partial failure (FR-6.1–6.3)**
- **Given** an X thread of 3 posts where the publication record already shows post 1 as done,
- **when** the announcement is published,
- **then** only posts 2 and 3 are sent, and post 2 replies to post 1's recorded ID.

**AC-16 No duplicates (FR-6.2)**
- **Given** an announcement whose publication record shows every channel as done,
- **when** `publish` runs,
- **then** no requests are made to any platform.

**AC-17 Due check (FR-7.1)**
- **Given** three announcements, one with a publish time in the past, one in the future and one with no time,
- **when** `publish` runs at a fixed clock time,
- **then** only the past-dated one and the undated one are published.

**AC-18 Preview (FR-5.1, FR-5.3)**
- **Given** an announcement,
- **when** `preview` runs,
- **then** each channel's posts are printed with their counts and limits, along with the estimated X cost, and no HTTP requests are made.

**AC-19 Secrets aren't logged (FR-8.2, NFR-5)**
- **Given** a publish run with verbose logging, where a request fails,
- **when** the log output is captured,
- **then** it contains no token values.

**AC-20 Ambiguous failure isn't retried (NFR-6)**
- **Given** a fake X API that times out on a create request,
- **when** the announcement is published,
- **then** that request isn't retried, the X channel is reported as needing a manual check, and the other channels carry on.

**AC-21 Workflow (FR-7.2, FR-7.3, FR-6.4, NFR-5)** — checked by review and a manual run
- **Given** the workflow file,
- **when** it is reviewed,
- **then** it has the `push`, `schedule` and `workflow_dispatch` triggers, a concurrency group, minimal permissions, actions pinned to SHAs, and a step that commits the publication record.
- **Also, given** a real announcement merged to `main`,
- **when** the workflow runs,
- **then** posts appear on all four channels and the publication record is committed back.

**AC-22 LinkedIn token refresh (FR-4.3.4, FR-4.3.5)** — checked manually
- **Given** an expired or nearly expired LinkedIn token,
- **when** `oyez auth linkedin` is run locally,
- **then** a browser sign-in completes, and the GitHub secret is updated through `gh secret set`.
- **And when** a later publish run checks expiry,
- **then** it no longer warns.

**AC-23 Indeterminate and resolve (FR-6.5, FR-6.6)**
- **Given** a publication record where the X channel is indeterminate after post 2 of a 3-post thread,
- **when** `publish` runs,
- **then** no X requests are made, and the output warns that X needs resolving.
- **And given** that `resolve` then records post 2 as posted, with its ID,
- **when** `publish` runs again,
- **then** only post 3 is sent, as a reply to post 2.

---

## Traceability to the intent

| Intent success criterion | Covered by |
|---|---|
| One file produces correct posts on all four channels | FR-1, FR-3, FR-4; AC-1, AC-7, AC-8, AC-11–AC-13 |
| Running cost under $25 a year | NFR-1, FR-5.3 |
| Announce in under five minutes | NFR-8, FR-7.1–7.2 |
| Buffer can be cancelled | All Must requirements, and AC-21 run for real |
