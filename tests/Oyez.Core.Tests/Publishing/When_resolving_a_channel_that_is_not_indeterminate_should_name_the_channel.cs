using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Core.Tests.Publishing;

public class ResolveChannelNameTests : OyezApplicationTest
{
    [Fact]
    public async Task When_resolving_a_channel_that_is_not_indeterminate_should_name_the_channel()
    {
        //Arrange
        Host.Announcements.Add(PublishingTestData.Announcement(Id, "linkedin", ["Release"]));
        var resolve = new ResolveChannel(Id, ChannelId.LinkedIn, new PostRef("urn-9", null));

        //Act
        await Host.CommandProcessor.SendAsync(resolve, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        var error = Assert.Single(resolve.Result!.Errors);
        Assert.Contains("LinkedIn", error.Message, StringComparison.Ordinal);
    }
}
