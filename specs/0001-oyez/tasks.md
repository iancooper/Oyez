# Oyez — Tasks

These tasks build the design in [ADR 0001](../../docs/adr/0001-oyez-design.md). They are ordered so that each one builds on the ones before it.

## How to work through them

Follow the workflow and conventions in `CLAUDE.md`, which follow Brighter's agent instructions.

1. **Write the tests first.** Each task lists the tests that prove it is done, as `When_…_should_…` names. Write them and **stop for the user's approval**. Then watch them fail, write the least code that makes them pass, and refactor.
2. **Test only through the module boundary.** That means Core's commands and queries, the adapters' port implementations, or the command line. No test names an internal type.
3. **One task at a time.** Tick a task (`[x]`) only when its tests pass and the full suite is green.
4. **Commit in two steps.** First a `feat:` or `test:` commit for the behaviour, then a `docs:` commit that ticks the task here and updates `PROMPT.md`. Structural changes go in their own `refactor:` commits.
5. **Refactor at the end of each phase.** Look for code smells before opening the phase's PR.

## Kinds of test

| Kind | What it is | Where it lives |
|---|---|---|
| **Core** | xUnit tests that send commands and run queries through the real Brighter and Darker processors, with `InMemory…` ports and `FakeTimeProvider` | `tests/Oyez.Core.Tests` |
| **Adapter** | xUnit tests of a port implementation against `FakeHttpMessageHandler`, a temporary folder, or a fake process runner | `tests/Oyez.Adapters.Tests` |
| **E2E** | The CLI run in-process against a temporary repository, with fake HTTP handlers registered through DI | `tests/Oyez.Cli.Tests` |
| **Review** | A checklist in the task, checked by reading the work. `actionlint` runs too, where it applies. | — |
| **Manual** | Steps run against real accounts or GitHub, with the expected result written down | — |

Requirement and acceptance-criteria IDs refer to `requirements.md`. § numbers refer to sections of ADR 0001.

---

## Phase 0 — Foundation

### [x] T-0 Solution skeleton, with Brighter and Darker wired up
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
  - *As built:* tests run on Microsoft Testing Platform (`global.json` → `test.runner`), because xunit.v3 4.x no longer supports VSTest on the .NET 10 SDK. So `Microsoft.NET.Test.Sdk` isn't needed. The two test projects that have no tests yet ignore exit code 8 (zero tests ran) until T-19 and T-28.
  - Add `.gitignore` and `.editorconfig`.
- **Done when:** `dotnet build` and `dotnet test` pass from a clean clone. Remove the spike's test command and query once T-17 replaces them.

### [x] T-1 CI workflow and Dependabot
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

## Phase 1 — Core domain, seen through the preview and validate queries

Parsing, counting, rendering and validation are internal to `Oyez.Core`. Every task in this phase observes them through the Darker queries `PreviewAnnouncement` and `ValidateAnnouncements`, run against an `InMemoryAnnouncementStore`.

### [x] T-2 Preview an announcement
- **Covers:** FR-1.1–1.4, §3, §4.1, §4.7, §4.10, AC-1
- **Builds:**
  - the public types `AnnouncementId`, `ChannelId`, `AnnouncementDocument`, `Rendering`, `Post` and `Result<T>`;
  - the port `IAmAnAnnouncementStore`, and `InMemoryAnnouncementStore`;
  - the query `PreviewAnnouncement`, which returns `Result<Preview>`. `Preview` holds the title, the publish time and one `Rendering` for each targeted channel. Until T-9, each segment is one post for X, Mastodon and LinkedIn, and Write.as gets the Markdown body.
  - Internally, the parser.
- **Tests (Core):**
  - `When_previewing_an_announcement_should_show_its_title_segments_and_markdown_for_every_channel` (AC-1)
  - `When_an_announcement_has_a_publish_time_should_preview_it_in_utc`: `Z` and offsets
  - `When_header_keys_vary_in_case_or_comments_are_present_should_still_read_the_header`
  - `When_a_header_value_contains_a_colon_should_keep_the_whole_value`
  - `When_a_segment_has_surrounding_whitespace_should_trim_it_but_keep_its_inner_newlines`
  - `When_an_announcement_has_crlf_line_endings_should_normalise_them`
  - `When_an_announcement_is_not_nfc_normalised_should_normalise_it`
  - `When_an_announcement_names_its_channels_should_preview_only_those`
  - `When_a_separator_line_has_trailing_whitespace_should_still_separate_segments`
- **Done when:** the tests pass.

### [x] T-3 Report a malformed announcement
- **Covers:** FR-1.3, FR-5.2, NFR-7, AC-2
- **Builds:** the query `ValidateAnnouncements`, which takes the IDs to check (or none, meaning all), and returns `ValidationError`s. `PreviewAnnouncement` returns the same errors as a failed `Result`.
- **Tests (Core):** each case gives a `ValidationError` naming the announcement and the problem.
  - `When_an_announcement_has_no_title_should_report_it` (AC-2)
  - `When_a_publish_time_has_no_time_zone_should_report_it` (AC-2)
  - `When_a_publish_time_cannot_be_parsed_should_report_it` (AC-2)
  - `When_a_header_key_is_unknown_should_report_it`
  - `When_a_header_line_has_no_colon_should_report_it`
  - `When_a_channel_is_unknown_should_report_it`
  - `When_a_segment_is_empty_should_report_its_number`
  - `When_an_announcement_has_no_segments_should_report_it`
  - `When_an_announcement_id_is_not_a_valid_folder_name_should_report_it`
  - `When_an_announcement_has_several_problems_should_report_them_all`
  - `When_validating_without_ids_should_check_every_announcement`
  - `When_previewing_an_announcement_that_does_not_exist_should_report_it`
- **Code-smell findings to resolve here:**
  - An unknown channel currently becomes `default(ChannelId)` and is silently dropped. It must become a `ValidationError`.
  - The preview handler uses `!` on `Load` and reads `.Value` without checking the `Result`. Return failures instead.
  - A header line with no colon throws, and a missing title becomes an empty string. Extract an internal `AnnouncementHeader` that owns the known keys and their parsing.
- **Done when:** the tests pass.

### [x] T-4 Count links for X and Mastodon
- **Covers:** FR-2.1, FR-2.2, §4.2
- **Refactor first, in its own `refactor:` commit:** replace the renderer's `channel == WriteAs` conditional with the internal `IAmAChannelProfile` strategy and `RenderingPolicy` (§4.3), and make the renderer take its profiles through DI. This was found in the code-smell review, and it's what lets counting, preparation, limits and merging attach to profiles rather than add branches.
- **Builds:** each post's `Count` in the preview, with links counted as 23. Internally, the URL matcher and the embedded IANA TLD list, with the list's date in a comment.
- **Tests (Core):** observed as X and Mastodon post counts in the preview.
  - `When_a_post_contains_a_url_should_count_it_as_23_characters`: `https://…` and `http://…`, on both X and Mastodon
  - `When_a_url_is_followed_by_punctuation_should_not_count_the_punctuation_as_part_of_the_link`
  - `When_a_post_contains_a_bare_domain_should_count_it_as_23_characters_for_x`: `brighter.io`, `asp.net`, `www.example.com/path`
  - `When_a_post_contains_dotted_names_that_are_not_domains_should_count_them_literally_for_x`: `Paramore.Brighter`, `v10.2.0`, `e.g.`, `file.cs`
  - `When_a_post_contains_a_bare_domain_should_count_it_literally_for_mastodon`
- **Done when:** the tests pass.

### [x] T-5 Count characters for X
- **Covers:** FR-2.1, AC-3, AC-4
- **Tests (Core):** observed as X post counts in the preview.
  - `When_a_segment_has_a_release_url_and_250_ascii_characters_should_count_273_for_x` (AC-3)
  - `When_a_segment_has_an_emoji_and_10_ascii_characters_should_count_12_for_x` (AC-4)
  - `When_a_segment_has_a_zwj_or_flag_emoji_should_count_each_as_2_for_x`
  - `When_a_segment_has_cjk_characters_should_count_each_as_2_for_x`
  - `When_a_segment_has_curly_quotes_should_count_each_as_1_for_x`
- **Done when:** the tests pass.

### [x] T-6 Count characters for Mastodon
- **Covers:** FR-2.2, AC-5
- **Tests (Core):** observed as Mastodon post counts in the preview.
  - `When_a_post_mentions_a_remote_account_should_count_only_the_username_for_mastodon` (AC-5): `@someone@example.social` counts as 8
  - `When_a_post_has_an_emoji_should_count_it_as_1_for_mastodon`
  - `When_a_post_mentions_a_local_account_should_count_it_literally_for_mastodon`
- **Done when:** the tests pass.

### [x] T-7 Prepare text for LinkedIn
- **Covers:** FR-2.3, FR-4.3.3, §4.5, AC-10
- **Tests (Core):** observed as the LinkedIn rendering's text and count in the preview.
  - `When_a_post_has_reserved_characters_should_escape_them_for_linkedin` (AC-10): every character in `\ | { } @ [ ] ( ) < > # * _ ~`
  - `When_a_post_has_a_hashtag_should_use_the_linkedin_hashtag_template` (AC-10)
  - `When_hashtag_conversion_is_off_should_escape_the_hash_for_linkedin`
  - `When_counting_for_linkedin_should_count_the_prepared_text`
- **Done when:** the tests pass.

### [x] T-8 Limits and enabled channels from configuration
- **Covers:** FR-2.4, FR-8.1, NFR-9, §4.3, §7
- **Builds:** public configuration types, bound in the CLI (T-28) from `oyez.json` and passed to `AddOyezApplication`. Each rendering in the preview carries its channel's limit.
- **Tests (Core):**
  - `When_previewing_should_show_each_channels_limit_from_configuration`: X 280, Mastodon 2,263, LinkedIn 3,000, Write.as none
  - `When_a_channel_is_disabled_should_not_preview_it`
- **Done when:** the tests pass.

### [x] T-9 Merge segments into posts
- **Covers:** FR-3.1–3.5, FR-1.5, §4.4
- **Tests (Core):** observed as the renderings in the preview.
  - `When_segments_do_not_fit_together_should_start_a_new_post` (AC-7): three segments of 200 characters with a limit of 450 give posts of 402 and 200 characters
  - `When_rendering_four_long_segments_should_give_each_channel_its_expected_post_count` (AC-8): X 4, Mastodon 1, LinkedIn 1, Write.as 1
  - `When_two_short_segments_fit_together_should_merge_them_for_x` (FR-3.4)
  - `When_a_merge_lands_exactly_on_the_limit_should_merge`
  - `When_a_merge_is_one_over_the_limit_should_not_merge`
  - `When_rendering_twice_should_give_identical_renderings_and_hashes` (FR-3.5)
  - `When_there_is_no_markdown_body_should_join_the_segments_for_write_as` (FR-1.5)
- **Done when:** the tests pass.

### [x] T-10 Validate against the channels' limits
- **Covers:** FR-5.2, FR-4.3.2, §4.6
- **Tests (Core):** through `ValidateAnnouncements`.
  - `When_a_segment_is_over_xs_limit_should_report_its_number` (AC-6)
  - `When_x_is_not_targeted_should_still_check_segments_against_xs_limit` (AC-6)
  - `When_the_linkedin_rendering_needs_more_than_one_post_should_report_it` (AC-9)
  - `When_linkedin_is_not_targeted_should_not_check_its_length`
  - `When_the_write_as_body_is_empty_should_report_it`
  - `When_an_announcement_is_valid_should_report_no_errors`
- **Done when:** the tests pass.

### [x] T-11 Estimate the X cost
- **Covers:** FR-5.3
- **Tests (Core):** through the preview.
  - `When_previewing_a_thread_should_estimate_its_x_cost`: with prices from configuration, a 3-post thread where only post 1 has a URL costs $0.20 + 2 × $0.015 = $0.23
  - `When_a_post_has_only_a_bare_domain_should_price_it_as_a_post_with_a_url`
- **Done when:** the tests pass.


### [x] T-11a An announcement that goes nowhere
- **Covers:** FR-1.3, FR-5.2, FR-8.1
- **Found by:** the Phase 1 code-smell review.
- **Tests (Core):** through `ValidateAnnouncements`.
  - `When_every_targeted_channel_is_disabled_should_report_it`: `channels: linkedin` with LinkedIn disabled in configuration
  - `When_the_channels_header_is_empty_should_report_it`: `channels:` with no value
- **Done when:** the tests pass.

### [x] T-11b Duplicate header keys
- **Covers:** FR-1.1, FR-5.2
- **Found by:** the Phase 1 code-smell review. Today the last value silently wins.
- **Tests (Core):** through `ValidateAnnouncements`.
  - `When_a_header_key_appears_twice_should_report_it`: two `title:` lines, in different case
- **Done when:** the tests pass.

### [x] T-11c Emoji from Unicode's data, checked against twitter-text
- **Covers:** FR-2.1, §4.2
- **Found by:** the Phase 1 code-smell review. `EmojiRanges` is a hand-typed table with no source or version; keycaps such as `1️⃣` count 5 on X instead of 2, and © and ® are unverified.
- **Builds:** the emoji ranges loaded from Unicode's `emoji-data.txt`, embedded like the TLD list, with its version in a comment, in place of the hand-typed table.
- **Tests (Core):** observed as X post counts in the preview.
  - `When_a_segment_has_a_keycap_emoji_should_count_it_as_2_for_x`
  - `When_counting_twitter_text_conformance_cases_should_match_for_x`: a `[Theory]` over cases from twitter-text's `conformance/validate.yml` (weighted tweets), including © and ®
- **Done when:** the tests pass.

### [x] T-11d A post over its channel's limit
- **Covers:** FR-2.4, FR-5.2, §4.6
- **Found by:** the Phase 1 code-smell review. Only X's segment check and LinkedIn's one-post check exist, so a channel configured with a smaller limit can render a post over it with no error.
- **Tests (Core):** through `ValidateAnnouncements`.
  - `When_a_post_is_over_its_channels_limit_should_report_it`: Mastodon's limit configured below a segment's length
  - `When_a_segment_is_over_xs_limit_should_report_it_once`: the segment check and the post check don't both report it
- **Done when:** the tests pass.

---

## Phase 2 — Publishing, through the application layer

Every task in this phase sends Brighter commands, or runs Darker queries, through the real processors. In-memory publishers stand in for the platforms (they're test doubles for I/O). `InMemoryPublicationRecordStore` and `FakeTimeProvider` stand in for storage and the clock.

### [ ] T-12 Publication status
- **Covers:** FR-6.1, §4.8
- **Builds:**
  - the public `PublicationRecord` and its channel states (`partial`, `complete`, `indeterminate`, `failed`);
  - the port `IAmAPublicationRecordStore`, and `InMemoryPublicationRecordStore`;
  - the query `GetPublicationStatus`.
- **Tests (Core):**
  - `When_an_announcement_has_never_been_published_should_report_no_channels_published`
  - `When_a_publication_record_exists_should_report_each_channels_state_and_posts`
- **Code-smell finding to resolve here (Phase 1 review):** before the first test, hide the test boilerplate. About 40 tests write the whole announcement text inline where the header isn't the point, and every class repeats the host, the `_id` and `Dispose`. Use `AnnouncementText` where the header isn't evident data, and add a small fixture or base for the host and ID, so the Phase 2 tests start clean.
- **Done when:** the tests pass.

### [ ] T-13 Publish due announcements
- **Covers:** FR-7.1, FR-6.2, FR-6.3, FR-4.1.1, §4.9
- **Builds:** the port `IAmAChannelPublisher`, and the command `PublishDueAnnouncements`, which returns a `PublishReport` in its `Result`.
- **Tests (Core):**
  - `When_publishing_should_publish_only_announcements_that_are_due` (AC-17): one past-dated, one future-dated, one undated
  - `When_every_channel_is_complete_should_make_no_publisher_calls` (AC-16)
  - `When_publishing_a_thread_should_reply_to_the_previous_post` (thread chaining)
  - `When_publishing_a_thread_should_save_the_record_after_each_post` (FR-6.3; a spy store counts the saves)
  - `When_a_channel_is_not_targeted_or_not_enabled_should_not_publish_to_it`
- **Done when:** the tests pass.

### [ ] T-14 Resume, and keep channels independent
- **Covers:** FR-4.5.1, FR-6.2, AC-14 (Core level), AC-15
- **Tests (Core):**
  - `When_some_posts_are_recorded_should_publish_only_the_rest` (AC-15)
  - `When_one_channel_is_rejected_should_still_publish_the_others` (AC-14, Core level)
  - `When_a_channel_failed_last_time_should_retry_it`
  - `When_a_partly_published_announcement_was_edited_should_fail_that_channel_without_posting`
  - `When_a_channels_secret_is_missing_should_reject_it_and_publish_the_others`
- **Done when:** the tests pass.

### [ ] T-15 The indeterminate state
- **Covers:** FR-6.5, NFR-6, AC-23 (first part)
- **Tests (Core):**
  - `When_a_post_may_have_been_made_should_mark_the_channel_indeterminate_and_stop_it`
  - `When_a_channel_is_indeterminate_should_skip_it_with_a_warning`
- **Done when:** the tests pass.

### [ ] T-16 Events and the job summary
- **Covers:** §4.10, FR-4.5.2, NFR-10
- **Builds:** the events `PostPublished` and `ChannelPublishFailed`, and the job summary in the `PublishReport`.
- **Tests (Core):**
  - `When_publishing_should_summarise_each_channels_state_with_links_to_new_posts`
  - `When_a_post_is_published_should_save_the_record_before_raising_the_event`: a recording store and a recording event handler check the order
- **Done when:** the tests pass.

### [ ] T-17 Resolve an indeterminate channel
- **Covers:** FR-6.6, AC-23 (second part)
- **Builds:** the command `ResolveChannel`. Removes the T-0 spike command and query, and their tests.
- **Tests (Core):**
  - `When_resolving_a_post_as_made_should_carry_on_from_the_next_post` (AC-23)
  - `When_resolving_the_last_post_as_made_should_mark_the_channel_complete`
  - `When_resolving_a_post_as_not_made_should_retry_it_on_the_next_run`
  - `When_resolving_a_channel_that_is_not_indeterminate_should_report_an_error`
- **Done when:** the tests pass.

### [ ] T-18 Check credentials
- **Covers:** FR-4.3.5, NFR-7, §6 (`check`)
- **Builds:** `IAmAChannelPublisher.VerifyAsync`, and the query `CheckCredentials`.
- **Tests (Core):**
  - `When_checking_credentials_should_report_each_enabled_channels_status`
  - `When_the_linkedin_token_expires_soon_should_report_the_days_left`
- **Done when:** the tests pass.

---

## Phase 3 — Adapters

Every task in this phase tests a port implementation registered by `AddOyezAdapters`. HTTP handlers, OAuth signing and JSON shapes are internal, and are observed through the publishers.

### [ ] T-19 Write.as publisher, and the HTTP test fake
- **Covers:** FR-4.4.1, AC-13, §5.5
- **Builds:** `AddOyezAdapters`, `WriteAsPublisher`, and, in the tests, `FakeHttpMessageHandler`. Removes the "zero tests" exit-code exemption from `Oyez.Adapters.Tests`.
- **Tests (Adapter):**
  - `When_publishing_to_write_as_should_post_the_title_and_markdown_to_the_collection` (AC-13)
  - `When_write_as_accepts_a_post_should_return_its_url`
  - `When_write_as_rejects_a_post_should_return_rejected_with_the_reason`
  - `When_verifying_write_as_credentials_should_call_me`
- **Done when:** the tests pass.

### [ ] T-20 X publisher, with OAuth 1.0a
- **Covers:** FR-4.1, FR-4.1.2, AC-11, §5.2
- **Builds:** `XPublisher`. The clock (`TimeProvider`) and the nonce source are public seams, so the signature can be tested against X's published example.
- **Tests (Adapter):**
  - `When_publishing_a_thread_to_x_should_reply_to_each_previous_post` (AC-11)
  - `When_signing_a_request_for_x_should_match_xs_published_example`
  - `When_signing_a_request_for_x_should_not_sign_the_json_body`
  - `When_x_accepts_a_post_should_return_its_url`
  - `When_x_rejects_a_post_should_return_rejected_with_xs_error_detail`
  - `When_verifying_x_credentials_should_call_users_me`
- **Done when:** the tests pass.

### [ ] T-21 Safe retries for posts that can't be repeated
- **Covers:** NFR-6, AC-20, §5.1
- **Tests (Adapter):** through `XPublisher` and `WriteAsPublisher`.
  - `When_creating_a_post_times_out_should_return_indeterminate_without_retrying` (AC-20)
  - `When_creating_a_post_gets_a_server_error_should_return_indeterminate_without_retrying`
  - `When_the_connection_is_refused_should_retry_the_post`
  - `When_rate_limited_briefly_should_wait_and_retry`
  - `When_rate_limited_for_too_long_should_return_rejected`
  - `When_a_read_gets_a_server_error_should_retry_with_backoff`
- **Done when:** the tests pass.

### [ ] T-22 Mastodon publisher
- **Covers:** FR-4.2, AC-12, §5.3
- **Tests (Adapter):**
  - `When_publishing_a_thread_to_mastodon_should_reply_to_the_previous_post_publicly` (AC-12)
  - `When_publishing_to_mastodon_should_send_the_idempotency_key`
  - `When_creating_a_mastodon_post_gets_a_server_error_should_retry_it`
  - `When_verifying_mastodon_credentials_should_call_verify_credentials`
- **Done when:** the tests pass.

### [ ] T-23 LinkedIn publisher
- **Covers:** FR-4.3.1, FR-4.3.5, §5.4
- **Tests (Adapter):**
  - `When_publishing_to_linkedin_should_post_to_the_members_profile_with_the_api_version`
  - `When_linkedin_accepts_a_post_should_return_its_feed_url`
  - `When_the_linkedin_token_expires_soon_should_warn_and_still_post`
  - `When_the_linkedin_token_has_expired_should_return_rejected_without_calling_linkedin`
  - `When_verifying_linkedin_credentials_should_call_userinfo`
- **Done when:** the tests pass.

### [ ] T-24 Keep secrets out of the logs
- **Covers:** FR-8.2, NFR-5, AC-19 (adapter level)
- **Tests (Adapter):** through any publisher, with logs captured.
  - `When_logging_a_request_should_never_log_headers`
  - `When_logging_an_error_body_should_redact_secrets`
  - `When_logging_an_exception_should_redact_secrets`
- **Done when:** the tests pass.

### [ ] T-25 File-system announcement store
- **Covers:** FR-1.1, §5.6
- **Tests (Adapter):** in temporary folders.
  - `When_listing_announcements_should_return_each_folder_and_ignore_dot_files`
  - `When_loading_an_announcement_should_return_its_text_and_markdown`
  - `When_loading_an_announcement_that_does_not_exist_should_return_nothing`
- **Done when:** the tests pass.

### [ ] T-26 File-system publication record store
- **Covers:** FR-6.1, FR-6.3, §4.8, §5.6
- **Tests (Adapter):** in temporary folders.
  - `When_saving_a_record_should_write_camel_case_indented_json`
  - `When_loading_a_saved_record_should_return_the_same_record`: every state, round-tripped
  - `When_saving_fails_part_way_should_leave_the_previous_record_intact`
- **Done when:** the tests pass.

### [ ] T-27 GitHub secret store (`gh` CLI)
- **Covers:** FR-4.3.4, FR-4.4.2, §5.7
- **Builds:** the port `IAmAGitHubSecretStore` in Core, and `GhCli` in Adapters. The process runner is a public seam, so tests can use a fake.
- **Tests (Adapter):**
  - `When_setting_a_secret_should_pass_the_value_on_standard_input_not_the_arguments`
  - `When_setting_a_variable_should_call_gh_variable_set`
  - `When_gh_fails_should_raise_an_error_with_its_output`
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
- **Code-smell finding to resolve here (Phase 1 review):** when binding `OyezOptions` from `oyez.json`, have each `…Profile` hold its own `ChannelOptions` section rather than each repeating `options.<Channel>.Enabled` and `.Limit` (Preserve Whole Object).
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
    - `README.md`: what Oyez is, a quick start, and links. (An early README was added in phase 0. T-38 updates it to match what was built, and removes the "in development" notice.)
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

**Refactoring, from the code-smell review after T-2:**
- Rewrite the block-splitting loop in `AnnouncementParser` as a pipeline.
- Remove package references that nothing uses yet, such as `Paramore.Darker.QueryLogging` and the unused ones in Adapters and Cli. Add them back when a task needs them.
- Decide whether `Result<T>` is general-purpose. If it is, move `ValidationError` next to it.

**Features:**

- FR-1.6: text overrides for a single channel (`x.txt`, `linkedin.txt` and so on).
- FR-7.5: `repository_dispatch` from the BrighterCommand repositories, if polling turns out to be too slow.
- Link previews on LinkedIn: attach an article to the post. This needs media support, which is out of scope.
