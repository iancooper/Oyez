using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.LinkedIn;

public sealed record LinkedInPostOutcome(PostResult Result, CollectedLogs Logs, int RequestCount);
