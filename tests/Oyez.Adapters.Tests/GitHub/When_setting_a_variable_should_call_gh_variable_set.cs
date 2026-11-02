using Microsoft.Extensions.DependencyInjection;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.GitHub;

public class SetGitHubVariableTests
{
    [Fact]
    public async Task When_setting_a_variable_should_call_gh_variable_set()
    {
        //Arrange
        var runner = new RecordingProcessRunner(new ProcessResult(0, "✓ Created variable MASTODON_BASE_URL"));
        using var host = new OyezAdaptersHost(_ => { }, services => services.AddSingleton<IAmAProcessRunner>(runner));

        //Act
        await host.Service<IAmAGitHubSecretStore>().SetVariableAsync("MASTODON_BASE_URL", "https://hachyderm.io", CancellationToken.None);

        //Assert
        Assert.Equal("gh", runner.FileName);
        Assert.Equal(["variable", "set", "MASTODON_BASE_URL"], runner.Arguments);
        Assert.Equal("https://hachyderm.io", runner.StandardInput);
    }
}
