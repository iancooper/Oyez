using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.LinkedIn;

public class LinkedInTokenExpiredTests
{
    [Fact]
    public async Task When_the_linkedin_token_has_expired_should_return_rejected_without_calling_linkedin()
    {
        //Arrange
        var expiresAt = LinkedInTestData.NOW.AddDays(-1);

        //Act
        var outcome = await LinkedInTestData.PostWithTokenExpiringAtAsync(expiresAt);

        //Assert
        var rejected = Assert.IsType<PostResult.Rejected>(outcome.Result);
        Assert.Equal("LinkedIn token expired on 2026-10-30; run oyez auth linkedin", rejected.Reason);
        Assert.Equal(0, outcome.RequestCount);
    }
}
