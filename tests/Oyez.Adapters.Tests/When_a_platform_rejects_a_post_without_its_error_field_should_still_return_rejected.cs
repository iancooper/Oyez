using System.Net;
using Oyez.Adapters.Tests.Mastodon;
using Oyez.Adapters.Tests.WriteAs;
using Oyez.Adapters.Tests.X;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests;

public class RejectsPostWithoutErrorFieldTests
{
    [Theory]
    [InlineData("x", 403)]
    [InlineData("mastodon", 422)]
    [InlineData("writeas", 400)]
    public async Task When_a_platform_rejects_a_post_without_its_error_field_should_still_return_rejected(string channelName, int statusCode)
    {
        //Arrange
        ChannelId.TryParse(channelName, out var channel);
        using var host = new OyezAdaptersHost(options =>
        {
            XTestData.Configure(options);
            MastodonTestData.Configure(options);
            WriteAsTestData.Configure(options);
        });
        host.Http.RespondWith((HttpStatusCode)statusCode, """{"title":"Rejected","status":0}""");
        var request = PostRequests.Build(channel, 280, "Brighter 10.1", ["Brighter 10.1 is out."]);
        PostResult? result = null;
        Exception? thrown = null;

        //Act
        try
        {
            result = await host.PublisherFor(channel).CreateAsync(request, CancellationToken.None);
        }
        catch (Exception exception)
        {
            thrown = exception;
        }

        //Assert
        Assert.Null(thrown);
        var rejected = Assert.IsType<PostResult.Rejected>(result);
        Assert.Contains(statusCode.ToString(), rejected.Reason);
    }
}
