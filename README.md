# Oyez

[![CI](https://github.com/iancooper/Oyez/actions/workflows/ci.yml/badge.svg)](https://github.com/iancooper/Oyez/actions/workflows/ci.yml)

*Oyez! Oyez! Oyez!* — the town crier's call for attention.

Oyez announces new releases of [Brighter](https://github.com/BrighterCommand) on X, Mastodon, LinkedIn and Write.as. It replaces Buffer at a fraction of the cost.

> **Status: in development.** The design is complete and the build has started, but Oyez can't post anything yet. Progress is tracked in [`tasks.md`](tasks.md).

## Why

Buffer charges per channel, which is a lot for a few posts a month, and it doesn't support Write.as. Each platform also has a different character limit, so with Buffer we end up editing every post by hand.

With Oyez, you write once and post everywhere:

1. **Write an announcement as short segments.** Each segment fits X's 280-character limit.
2. **Oyez merges the segments into the longest posts each platform allows:**

   | Platform | Limit | Typical result |
   |---|---|---|
   | X (`@ICooper`) | 280 | A thread, with one post per segment |
   | Mastodon (`@icooper@hachyderm.io`) | 2,263 | A single post |
   | LinkedIn (personal profile) | 3,000 | A single post |
   | Write.as ([ian-cooper.writeas.com](https://ian-cooper.writeas.com/)) | none | A Markdown blog post, which can include the full release notes |

3. **Merge a pull request to publish.** GitHub Actions previews each platform's posts on the pull request, then posts when the PR is merged, or later at a scheduled time.

The running cost is X's API fees: about $0.20 per announcement, so roughly $12 a year. Everything else is free.

## How it will work

Each announcement is a folder in `announcements/`:

```
announcements/2026-11-03-brighter-10-3-0/
  announcement.txt   # a header, then plain-text segments separated by ---
  writeas.md         # optional Markdown body for the Write.as post
  published.json     # written by Oyez: what has been posted where
```

`announcement.txt` looks like this:

```
title: Brighter 10.3.0 released
publish: 2026-11-03T09:00Z
---
Brighter 10.3.0 is out! … https://github.com/BrighterCommand/Brighter/releases/tag/10.3.0
---
Also in this release: …
```

And these are the commands it will have:

```
oyez new brighter-10-3-0     # create an announcement from a template
oyez validate                # check the format and every platform's limits
oyez preview <id>            # show each platform's posts, with counts and the X cost
oyez publish                 # post every announcement that is due (run by GitHub Actions)
oyez check                   # check every platform's credentials without posting
```

Oyez never posts the same thing twice. It records each post as it's made, and if a run fails partway through, the next run carries on from where it stopped.

## Building

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```sh
dotnet build
dotnet test
```

## Design

Oyez uses ports and adapters. The application layer is built from [Brighter](https://github.com/BrighterCommand/Brighter) commands and [Darker](https://github.com/BrighterCommand/Darker) queries, so Oyez also works as a small example of both libraries.

These planning documents describe it in full:

| Document | Contents |
|---|---|
| [`intent.md`](intent.md) | The problem, the goals, the costs, and the decisions made |
| [`requirements.md`](requirements.md) | Functional and non-functional requirements, and acceptance criteria |
| [`design.md`](design.md) | The architecture, the announcement format, the platform APIs, and the GitHub Actions workflows |
| [`tasks.md`](tasks.md) | The build plan: test-first tasks, each with the test that proves it |
| [`PROMPT.md`](PROMPT.md) | The current working state, for picking up where work stopped |
