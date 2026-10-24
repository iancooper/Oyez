# Build and Development Commands

Oyez targets .NET 10. `global.json` pins the SDK and selects Microsoft Testing Platform as the test runner.

## Building the Solution

```bash
# Build entire solution
dotnet build Oyez.slnx

# Build specific project
dotnet build src/Oyez.Core/Oyez.Core.csproj

# Build in Release mode
dotnet build Oyez.slnx -c Release
```

Warnings are errors, and projects under `src/` generate XML documentation, so a public member without `///` docs fails the build.

## Running Tests

```bash
# Run all tests
dotnet test

# Run tests for specific project
dotnet test --project tests/Oyez.Core.Tests/

# Run tests matching a name
dotnet test --project tests/Oyez.Core.Tests/ --filter "FullyQualifiedName~When_previewing"
```

No external infrastructure is needed: tests use `InMemory…` port implementations, fake HTTP handlers and `FakeTimeProvider` in place of I/O and the clock.
