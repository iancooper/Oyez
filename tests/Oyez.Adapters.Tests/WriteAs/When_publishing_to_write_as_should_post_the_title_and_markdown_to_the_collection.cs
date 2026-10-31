#region Licence

/* The MIT License (MIT)
Copyright © 2026 Ian Cooper

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

#endregion

using System.Net;
using System.Text.Json;
using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.WriteAs;

public class WriteAsPublisherTests
{
    [Fact]
    public async Task When_publishing_to_write_as_should_post_the_title_and_markdown_to_the_collection()
    {
        //Arrange
        using var host = new OyezAdaptersHost(options =>
        {
            options.WriteAs.AccessToken = "writeas-secret";
            options.WriteAs.Collection = "ian-cooper";
            options.WriteAs.ApiBase = "https://write.as/api";
            options.WriteAs.BlogUrl = "https://ian-cooper.writeas.com";
        });
        host.Http.RespondWith(HttpStatusCode.Created, """{"code":201,"data":{"id":"abc123","slug":"brighter-10-1"}}""");
        var post = new Post(0, "Brighter 10.1 is out.\n\nSee what changed.", 38);
        var request = new PostRequest(
            new AnnouncementId("brighter-10-1"),
            new Rendering(ChannelId.WriteAs, "Brighter 10.1", null, [post]),
            0,
            null,
            "sha256:0123456789ab");

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
