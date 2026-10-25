using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class NeverPublishedStatusTests : OyezApplicationTest
{
    [Fact]
    public async Task When_an_announcement_has_never_been_published_should_report_no_channels_published()
    {
        //Arrange
        var query = new GetPublicationStatus(Id);

        //Act
        var status = await Host.QueryProcessor.ExecuteAsync(query, TestContext.Current.CancellationToken);

        //Assert
        Assert.Empty(status.Channels);
    }
}
