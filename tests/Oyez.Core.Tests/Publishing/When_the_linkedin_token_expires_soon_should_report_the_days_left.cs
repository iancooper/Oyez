using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class ExpiringTokenTests : OyezApplicationTest
{
    [Fact]
    public async Task When_the_linkedin_token_expires_soon_should_report_the_days_left()
    {
        //Arrange
        var expiresAt = Host.Clock.GetUtcNow().AddDays(10).AddHours(3);
        Host.PublisherFor(ChannelId.LinkedIn).ReportCredentials(new CredentialStatus(true, "Token is valid.", expiresAt));

        //Act
        var report = await Host.QueryProcessor.ExecuteAsync(new CheckCredentials(), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal(10, report.Channels.Single(c => c.Channel == ChannelId.LinkedIn).DaysLeft);
    }
}
