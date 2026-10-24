using Oyez.Core.Announcements;
using Oyez.Core.Channels;
using Oyez.Core.Previewing;

namespace Oyez.Core.Tests.Counting;

// Cases are the weightedLength expectations in the WeightedTweetsWithDiscountedEmojiCounterTest section of
// twitter-text's conformance/validate.yml (master, last changed in commit 65e7e00 on 2020-03-31), which
// is the section for the current config with emoji weighted 2. Cases whose text exceeds one post, or that
// begin or end with whitespace, are left out. The last group is not from the conformance file; it
// records Unicode's Extended_Pictographic treatment of (c), (r) and (tm), with and without U+FE0F.
public class XTwitterTextConformanceTests : IDisposable
{
    private readonly OyezApplicationHost _host = new();
    private readonly AnnouncementId _id = new("2026-11-03-brighter-10-3-0");

    [Theory]
    [InlineData("Hi http://test.co", 26)]
    [InlineData("http://test.co", 23)]
    [InlineData("randomurlrandomurlrandomurlrandomurlrandomurlrandomurlrandomurls.com", 68)]
    [InlineData("H🐱☺👨‍👩‍👧‍👦", 7)]
    [InlineData("😷👾😡🔥💩", 10)]
    [InlineData("🙋🏽👨‍🎤", 4)]
    [InlineData("1⃣", 2)]
    [InlineData("Unicode 10.0 emoji: 🤪; 🧕; 🧕🏾; 🏴󠁧󠁢󠁥󠁮󠁧󠁿", 34)]
    [InlineData("Unicode 9.0 emoji: 🤠; 💃; 💃🏾", 29)]
    [InlineData("This is a tweet with general punctuation characters:            ​ ‐ ‑ ‒ – — ― ‖  ‗ ‘ ’ ‚ ‛ “ ” „ ‟ ′ ″ ‴ ‵ ‶ ‷", 110)]
    [InlineData("©", 2)]
    [InlineData("®", 2)]
    [InlineData("™", 2)]
    [InlineData("©️", 2)]
    [InlineData("®️", 2)]
    [InlineData("™️", 2)]
    public async Task When_counting_twitter_text_conformance_cases_should_match_for_x(string text, int expectedWeightedLength)
    {
        //Arrange
        _host.Announcements.Add(new AnnouncementDocument(
            _id,
            $"""
            title: Release
            channels: X
            ---
            {text}
            """,
            WriteAsMarkdown: null));

        //Act
        var result = await _host.QueryProcessor.ExecuteAsync(new PreviewAnnouncement(_id), TestContext.Current.CancellationToken);

        //Assert
        var post = Assert.Single(Assert.Single(result.Value.Renderings).Posts);
        Assert.Equal(expectedWeightedLength, post.Count);
    }

    public void Dispose() => _host.Dispose();
}
