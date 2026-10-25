using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class CheckCredentialsTests() : OyezApplicationTest(options => options.Mastodon.Enabled = false)
{
    [Fact]
    public async Task When_checking_credentials_should_report_each_enabled_channels_status()
    {
        //Arrange
        var rejected = new CredentialStatus(false, "X rejected the token. Run oyez auth x.");
        Host.PublisherFor(ChannelId.X).ReportCredentials(rejected);

        //Act
        var report = await Host.QueryProcessor.ExecuteAsync(new CheckCredentials(), TestContext.Current.CancellationToken);

        //Assert
        Assert.Equal([ChannelId.X, ChannelId.LinkedIn, ChannelId.WriteAs], report.Channels.Select(c => c.Channel));
        Assert.Equal(rejected, report.Channels.Single(c => c.Channel == ChannelId.X).Status);
        Assert.True(report.Channels.Single(c => c.Channel == ChannelId.LinkedIn).Status.IsValid);
        Assert.True(report.Channels.Single(c => c.Channel == ChannelId.WriteAs).Status.IsValid);
    }
}
