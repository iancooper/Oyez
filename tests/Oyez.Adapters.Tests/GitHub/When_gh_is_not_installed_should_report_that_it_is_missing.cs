using Microsoft.Extensions.DependencyInjection;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.GitHub;

[Collection(ProcessEnvironmentCollection.NAME)]
public class MissingGitHubCliTests
{
    [Fact]
    public async Task When_gh_is_not_installed_should_report_that_it_is_missing()
    {
        //Arrange
        using var noGhOnPath = new EmptySearchPath();
        using var host = new OyezAdaptersHost(_ => { });
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        //Act
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            host.Service<IAmAGitHubSecretStore>().SetSecretAsync("X_ACCESS_TOKEN", "s3cr3t-token-value", timeout.Token));

        //Assert
        Assert.Contains("gh", error.Message);
        Assert.Contains("not installed", error.Message);
        Assert.DoesNotContain("s3cr3t-token-value", error.Message);
    }
}
