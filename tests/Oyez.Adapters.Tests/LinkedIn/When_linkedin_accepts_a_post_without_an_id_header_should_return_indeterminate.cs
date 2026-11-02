using System.Net;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.LinkedIn;

public class LinkedInAcceptsPostWithoutIdTests
{
    [Fact]
    public async Task When_linkedin_accepts_a_post_without_an_id_header_should_return_indeterminate()
    {
        //Arrange
        using var host = new OyezAdaptersHost(LinkedInTestData.Configure, LinkedInTestData.FixTime);
        host.Http.RespondWith(_ => new HttpResponseMessage(HttpStatusCode.Created));

        //Act
        var result = await host.PublisherFor(ChannelId.LinkedIn).CreateAsync(PostRequests.Build(ChannelId.LinkedIn, 3000, null, ["Brighter 10.1 is out. It adds batching."]), CancellationToken.None);

        //Assert
        var indeterminate = Assert.IsType<PostResult.Indeterminate>(result);
        Assert.Equal("LinkedIn accepted the post but returned no id, so the post may have been made", indeterminate.Reason);
    }
}
