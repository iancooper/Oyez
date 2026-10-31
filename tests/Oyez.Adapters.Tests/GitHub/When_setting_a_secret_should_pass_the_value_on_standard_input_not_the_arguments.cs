using Microsoft.Extensions.DependencyInjection;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.Tests.GitHub;

public class SetGitHubSecretTests
{
    [Fact]
    public async Task When_setting_a_secret_should_pass_the_value_on_standard_input_not_the_arguments()
    {
        //Arrange
        var runner = new RecordingProcessRunner(new ProcessResult(0, "✓ Set secret X_ACCESS_TOKEN"));
        using var host = new OyezAdaptersHost(_ => { }, services => services.AddSingleton<IAmAProcessRunner>(runner));

        //Act
        await host.Service<IAmAGitHubSecretStore>().SetSecretAsync("X_ACCESS_TOKEN", "s3cr3t-token-value", CancellationToken.None);

        //Assert
        Assert.Equal("gh", runner.FileName);
        Assert.Equal(["secret", "set", "X_ACCESS_TOKEN"], runner.Arguments);
        Assert.Equal("s3cr3t-token-value", runner.StandardInput);
    }
}
