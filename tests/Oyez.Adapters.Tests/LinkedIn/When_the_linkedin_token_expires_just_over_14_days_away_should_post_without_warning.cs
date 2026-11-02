using Microsoft.Extensions.Logging;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.LinkedIn;

public class LinkedInTokenExpiresJustOverFourteenDaysTests
{
    [Fact]
    public async Task When_the_linkedin_token_expires_just_over_14_days_away_should_post_without_warning()
    {
        //Arrange
        var expiresAt = LinkedInTestData.NOW.AddDays(14).AddSeconds(1);

        //Act
        var outcome = await LinkedInTestData.PostWithTokenExpiringAtAsync(expiresAt);

        //Assert
        Assert.IsType<PostResult.Posted>(outcome.Result);
        Assert.DoesNotContain(outcome.Logs.Entries, entry => entry.Level == LogLevel.Warning);
    }
}
