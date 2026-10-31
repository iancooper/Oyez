using Microsoft.Extensions.Logging;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.LinkedIn;

public class LinkedInTokenExpiringSoonTests
{
    [Fact]
    public async Task When_the_linkedin_token_expires_soon_should_warn_and_still_post()
    {
        //Arrange
        var logs = new CollectedLogs();
        var expiresInTenDays = LinkedInTestData.NOW.AddDays(10);
        using var host = new OyezAdaptersHost(
            LinkedInTestData.ConfigureExpiringAt(expiresInTenDays),
            services =>
            {
                LinkedInTestData.FixTime(services);
                logs.CaptureIn(services);
            });
        host.Http.RespondWith(_ => LinkedInTestData.CreatedWithId("urn:li:share:7251"));

        //Act
        var result = await host.PublisherFor(ChannelId.LinkedIn).CreateAsync(LinkedInTestData.Request(), CancellationToken.None);

        //Assert
        Assert.IsType<PostResult.Posted>(result);
        var warning = Assert.Single(logs.Entries, entry => entry.Level == LogLevel.Warning);
        Assert.Contains("2026-11-10", warning.Message);
    }
}
