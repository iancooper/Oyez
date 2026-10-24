# Oyez — Intent

## Problem

We announce new releases of [Brighter](https://github.com/BrighterCommand) on X (Twitter), Mastodon and LinkedIn, at most about once a week. We currently use Buffer for this, and it has three problems:

1. **Cost.** Buffer charges per channel: about $5 per channel per month on Essentials and $10 on Team. For three channels that is roughly $180–$360 a year, which is a lot for a few posts a month.
2. **No Write.as.** We also want to post to our Write.as blog, and Buffer does not support Write.as.
3. **Awkward workflow.** Each platform has a different character limit. We want to write once, against the strictest limit, and have the tool build the longer posts for the other platforms. Buffer makes us edit each platform's version separately.

## Goal

Build our own small tool, **Oyez**, in C#, that:

- takes one announcement, written as a sequence of short **segments** that each fit X's 280-character limit;
- merges those segments into the largest posts each platform allows, so X gets a thread, Mastodon a shorter thread, LinkedIn a single post and Write.as a blog post;
- publishes to X, Mastodon, LinkedIn and Write.as;
- costs well under Buffer to run, ideally a few dollars a year.

Scheduling would be useful but is not essential.

## Findings from the feasibility study (October 2026)

We don't need a paid subscription on any platform to post through its API. The only charge is X's per-post fee.

| Platform | API access | Cost | Notes |
|---|---|---|---|
| X | Pay-per-use. The free tier has ended, and the Basic and Pro tiers no longer take new sign-ups. | $0.015 per post; $0.20 per post that contains a URL. No minimum spend. | You buy credits in advance in the Developer Console. Posting needs a user-context token: OAuth 1.0a, or OAuth 2.0 with PKCE. |
| Mastodon | Free. Create an app on your instance and you get a token straight away. | $0 | The default limit is 500 characters, but some instances set a different one. |
| LinkedIn | Free, self-serve, through the "Share on LinkedIn" product (`w_member_social` scope). | $0 | The app must be linked to a LinkedIn Page. Self-serve access posts only to a **personal profile**; posting to a company page needs the Community Management API, which LinkedIn reviews. Self-serve tokens are believed to last about 60 days with no refresh, so someone must sign in again periodically. Check this against LinkedIn's current documentation. |
| Write.as | Free with our existing account. Log in through the API to get a token, then post to a collection (blog). | $0 (we already subscribe) | Posts are Markdown. There is no length limit worth worrying about. |

**Estimated running cost.** A release announcement contains a link. In a thread, only the post that contains the URL pays the $0.20 rate. Four announcements a month comes to about $1 a month, or about $12 a year. Hosting is free if we run Oyez locally or from GitHub Actions.

**Buffer's free plan** allows 3 channels with 10 queued posts each. That might cover our current three platforms at no cost, but it still wouldn't support Write.as or the write-once workflow.

## How the write-once model works

The author writes an announcement as an ordered list of segments. Each segment fits within 280 characters, counted the way X counts them (a URL counts as 23 characters, whatever its real length).

For each platform, Oyez combines adjacent segments greedily, adding the next segment until it would push the post over that platform's limit:

| Platform | Limit | Result |
|---|---|---|
| X | 280 | One post per segment, published as a thread |
| Mastodon | Set per instance: 2,263 on hachyderm.io (the default is 500) | Fewer, longer posts, published as a thread if they don't fit in one |
| LinkedIn | about 3,000 | Usually a single post |
| Write.as | effectively none | One Markdown blog post with a title, which can include extra content such as full release notes |

An announcement may carry extra content for a single platform, such as a title or a longer body for Write.as.

## Scope

**In scope**

- A command-line tool, written in C# and .NET.
- An announcement file format (Markdown or YAML) that holds the segments, links and any extra content for each platform.
- Character counting for each platform, including the URL rules on X and Mastodon.
- Publishing to X, Mastodon, LinkedIn (personal profile) and Write.as.
- A dry-run mode that shows what would be posted to each platform without posting it.
- Secrets supplied through environment variables or a local secret store, so the tool runs both locally and in GitHub Actions.

**Nice to have**

- Scheduling: a GitHub Actions cron workflow that publishes announcements whose date has passed.
- Drafting an announcement from a GitHub release in a BrighterCommand repository.
- Triggering a post automatically when a release is published.

**Out of scope (for now)**

- Analytics, engagement tracking and replying to comments.
- Images and other media attachments.
- Platforms other than these four.
- A web UI.
- Posting to a LinkedIn company page. We'll revisit this if LinkedIn grants Community Management API access.

## Success criteria

- One announcement file produces correct posts on all four platforms, with no editing for each platform.
- Running costs stay under $25 a year.
- A release can be announced in under five minutes, from writing the file to the posts going live.
- We can cancel Buffer.

## Decisions

These answer the questions we had open after the feasibility study.

1. **LinkedIn:** post to the personal profile only, using self-serve access. We won't apply for company page access.
2. **Mastodon:** post as `@icooper@hachyderm.io`. The instance allows **2,263 characters** per post, so Mastodon will usually get a single post rather than a thread.
3. **X:** post as `@ICooper`. The account doesn't have X Premium, so the 280-character limit applies.
4. **Write.as:** post to the blog at <https://ian-cooper.writeas.com/> (collection alias `ian-cooper`).
5. **Announcement format:** segments are plain text. The Write.as version is Markdown.
6. **Where it runs:** in GitHub Actions from the start. Secrets live in GitHub Actions secrets, and the tool also runs locally for previews and debugging.

With these limits, a typical announcement becomes a thread on X and a single post on each of Mastodon and LinkedIn, plus a Markdown blog post on Write.as.
