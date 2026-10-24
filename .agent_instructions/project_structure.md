# Project Structure

- `Oyez.slnx` - the solution
- `src/Oyez.Core/` - the domain: Brighter commands and events, Darker queries, their handlers, and the ports (`IAmA…`) that adapters implement. Parsing, counting, rendering, validation and the publishing workflow are internal
- `src/Oyez.Adapters/` - the port implementations: a publisher per platform (X, Mastodon, LinkedIn, Write.as), file-system stores and the GitHub CLI
- `src/Oyez.Cli/` - the composition root and command line
- `tests/Oyez.Core.Tests/`, `tests/Oyez.Adapters.Tests/`, `tests/Oyez.Cli.Tests/` - one test project per module
- `specs/` - feature specs (requirements, design, tasks); `docs/adr/` - Oyez's ADRs; `docs/` - user documentation

Each assembly is a narrow, deep module; `CLAUDE.md` lists each one's public boundary, and the Oyez spec's `design.md` has the full module table.

## Testing Framework

- **Test Framework**: xUnit v3 on Microsoft Testing Platform. No mocking library: `InMemory…` port implementations, fakes for HTTP, and `FakeTimeProvider` for the clock
- **Test Patterns**: `When_[condition]_should_[expected_behaviour]`, one test per file
- **Composition**: Core tests build the application through `tests/Oyez.Core.Tests/OyezApplicationHost.cs`, which calls `AddOyezApplication` and shares one never-disposed `NullLoggerFactory`, because Darker captures a logger factory in static state

## Package Management

- Uses Central Package Management via `Directory.Packages.props`
- Single target: .NET 10 (`Directory.Build.props`)
