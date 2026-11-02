using Microsoft.Extensions.Logging;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.LinkedIn;

public class LinkedInTokenExpiresInFourteenDaysTests
{
    [Fact]
    public async Task When_the_linkedin_token_expires_in_exactly_14_days_should_warn_and_still_post()
    {
        //Arrange
        var expiresAt = LinkedInTestData.NOW.AddDays(14);

        //Act
        var outcome = await LinkedInTestData.PostWithTokenExpiringAtAsync(expiresAt);

        //Assert
        Assert.IsType<PostResult.Posted>(outcome.Result);
        var warning = Assert.Single(outcome.Logs.Entries, entry => entry.Level == LogLevel.Warning);
        Assert.Contains("2026-11-14", warning.Message);
    }
}
