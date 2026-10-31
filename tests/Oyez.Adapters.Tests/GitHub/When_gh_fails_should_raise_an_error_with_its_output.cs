using Microsoft.Extensions.DependencyInjection;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.GitHub;

public class GitHubFailureTests
{
    [Fact]
    public async Task When_gh_fails_should_raise_an_error_with_its_output()
    {
        //Arrange
        var runner = new RecordingProcessRunner(new ProcessResult(1, "HTTP 403: Resource not accessible by personal access token"));
        using var host = new OyezAdaptersHost(_ => { }, services => services.AddSingleton<IAmAProcessRunner>(runner));

        //Act
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            host.Service<IAmAGitHubSecretStore>().SetSecretAsync("X_ACCESS_TOKEN", "s3cr3t-token-value", CancellationToken.None));

        //Assert
        Assert.Contains("HTTP 403: Resource not accessible by personal access token", error.Message);
        Assert.DoesNotContain("s3cr3t-token-value", error.Message);
    }
}
