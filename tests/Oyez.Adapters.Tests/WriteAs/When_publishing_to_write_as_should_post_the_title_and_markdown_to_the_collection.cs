using System.Net;
using System.Text.Json;
using Oyez.Core.Channels;

namespace Oyez.Adapters.Tests.WriteAs;

public class PublishToWriteAsTests
{
    [Fact]
    public async Task When_publishing_to_write_as_should_post_the_title_and_markdown_to_the_collection()
    {
        //Arrange
        using var host = new OyezAdaptersHost(WriteAsTestData.Configure);
        host.Http.RespondWith(HttpStatusCode.Created, """{"code":201,"data":{"id":"abc123","slug":"brighter-10-1"}}""");
        var request = WriteAsTestData.Article();

        //Act
        await host.PublisherFor(ChannelId.WriteAs).CreateAsync(request, CancellationToken.None);

        //Assert
        var sent = Assert.Single(host.Http.Requests);
        Assert.Equal(HttpMethod.Post, sent.Method);
        Assert.Equal("https://write.as/api/collections/ian-cooper/posts", sent.Uri.ToString());
        Assert.Equal("Token writeas-secret", sent.Headers["Authorization"]);
        using var body = JsonDocument.Parse(sent.Body);
        Assert.Equal("Brighter 10.1", body.RootElement.GetProperty("title").GetString());
        Assert.Equal("Brighter 10.1 is out.\n\nSee what changed.", body.RootElement.GetProperty("body").GetString());
    }
}
