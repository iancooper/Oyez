using System.Text.Json;
using Oyez.Core.Channels;

namespace Oyez.Adapters.Tests.LinkedIn;

public class LinkedInPublishTests
{
    [Fact]
    public async Task When_publishing_to_linkedin_should_post_to_the_members_profile_with_the_api_version()
    {
        //Arrange
        using var host = new OyezAdaptersHost(LinkedInTestData.Configure, LinkedInTestData.FixTime);
        host.Http.RespondWith(_ => LinkedInTestData.CreatedWithId("urn:li:share:7251"));

        //Act
        await host.PublisherFor(ChannelId.LinkedIn).CreateAsync(LinkedInTestData.Request(), CancellationToken.None);

        //Assert
        var sent = Assert.Single(host.Http.Requests);
        Assert.Equal(HttpMethod.Post, sent.Method);
        Assert.Equal("https://api.linkedin.com/rest/posts", sent.Uri.ToString());
        Assert.Equal("Bearer linkedin-token", sent.Headers["Authorization"]);
        Assert.Equal("202609", sent.Headers["LinkedIn-Version"]);
        Assert.Equal("2.0.0", sent.Headers["X-Restli-Protocol-Version"]);
        using var body = JsonDocument.Parse(sent.Body);
        Assert.Equal("urn:li:person:8675309", body.RootElement.GetProperty("author").GetString());
        Assert.Equal("Brighter 10.1 is out. It adds batching.", body.RootElement.GetProperty("commentary").GetString());
        Assert.Equal("PUBLIC", body.RootElement.GetProperty("visibility").GetString());
        Assert.Equal("MAIN_FEED", body.RootElement.GetProperty("distribution").GetProperty("feedDistribution").GetString());
        Assert.Equal("PUBLISHED", body.RootElement.GetProperty("lifecycleState").GetString());
        Assert.False(body.RootElement.GetProperty("isReshareDisabledByAuthor").GetBoolean());
    }
}
