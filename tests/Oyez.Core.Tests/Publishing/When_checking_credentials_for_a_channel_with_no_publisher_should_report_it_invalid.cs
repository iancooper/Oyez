using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class MissingPublisherCredentialsTests() : OyezApplicationTest(channelsWithoutPublisher: [ChannelId.Mastodon])
{
    [Fact]
    public async Task When_checking_credentials_for_a_channel_with_no_publisher_should_report_it_invalid()
    {
        //Arrange
        var query = new CheckCredentials();

        //Act
        var report = await Host.QueryProcessor.ExecuteAsync(query, TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal([ChannelId.X, ChannelId.Mastodon, ChannelId.LinkedIn, ChannelId.WriteAs], report.Channels.Select(c => c.Channel));
        var missing = report.Channels.Single(c => c.Channel == ChannelId.Mastodon).Status;
        Assert.False(missing.IsValid);
        Assert.Equal("No publisher is configured for Mastodon.", missing.Message);
        Assert.True(report.Channels.Single(c => c.Channel == ChannelId.X).Status.IsValid);
    }
}
