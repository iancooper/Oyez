using System.Net;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.Mastodon;

public class MastodonRejectsPostWithoutJsonTests
{
    [Fact]
    public async Task When_mastodon_rejects_a_post_with_a_body_that_is_not_json_should_still_return_rejected()
    {
        //Arrange
        using var host = new OyezAdaptersHost(MastodonTestData.Configure);
        host.Http.RespondWith(HttpStatusCode.Unauthorized, "<html>Unauthorized</html>");
        PostResult? result = null;
        Exception? thrown = null;

        //Act
        try
        {
            result = await host.PublisherFor(ChannelId.Mastodon).CreateAsync(PostRequests.Thread(ChannelId.Mastodon, 500), CancellationToken.None);
        }
        catch (Exception exception)
        {
            thrown = exception;
        }

        //Assert
        Assert.Null(thrown);
        var rejected = Assert.IsType<PostResult.Rejected>(result);
        Assert.Contains("401", rejected.Reason);
    }
}
