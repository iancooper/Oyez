using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.LinkedIn;

public class LinkedInTokenExpiresNowTests
{
    [Fact]
    public async Task When_the_linkedin_token_expires_exactly_now_should_return_rejected_without_calling_linkedin()
    {
        //Arrange
        var expiresAt = LinkedInTestData.NOW;

        //Act
        var outcome = await LinkedInTestData.PostWithTokenExpiringAtAsync(expiresAt);

        //Assert
        Assert.IsType<PostResult.Rejected>(outcome.Result);
        Assert.Equal(0, outcome.RequestCount);
    }
}
