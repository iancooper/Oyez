using Microsoft.Extensions.Logging;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.LinkedIn;

public class LinkedInTokenExpiresInFourteenDaysTests
{
    [Fact]
    public async Task When_the_linkedin_token_expires_in_exactly_14_days_should_warn_and_still_post()
    {
        //Arrange
        var expiresAt = LinkedInTestData.NOW.AddDays(14);
        var logs = new CollectedLogs();
        using var host = new OyezAdaptersHost(
            LinkedInTestData.ConfigureExpiringAt(expiresAt),
            services =>
            {
                LinkedInTestData.FixTime(services);
                logs.CaptureIn(services);
            });
        host.Http.RespondWith(_ => LinkedInTestData.CreatedWithId("urn:li:share:7251"));

        //Act
        var result = await host.PublisherFor(ChannelId.LinkedIn).CreateAsync(PostRequests.Build(ChannelId.LinkedIn, 3000, null, ["Brighter 10.1 is out. It adds batching."]), CancellationToken.None);

        //Assert
        Assert.IsType<PostResult.Posted>(result);
        var warning = Assert.Single(logs.Entries, entry => entry.Level == LogLevel.Warning);
        Assert.Contains("2026-11-14", warning.Message);
    }
}
