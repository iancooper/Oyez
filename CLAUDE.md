# Oyez — instructions for agents

Read `PROMPT.md` first. It holds the plan, the current state and the next action. The planning documents are `intent.md`, `requirements.md`, `design.md` and `tasks.md`.

Oyez follows Brighter's agent instructions: <https://github.com/BrighterCommand/Brighter/tree/master/.agent_instructions>. Read `code_style.md`, `design_principles.md`, `testing.md` and the "Comments in Code" and "Licensing" sections of `documentation.md`. This file summarises them as they apply to Oyez, and records where Oyez differs. Where this file and Brighter's instructions disagree, this file wins.

## Workflow

- **Test first, with an approval gate.** For each behaviour:
  1. Write the failing test.
  2. **Stop and ask the user to approve the test** before running it or writing any production code. The exception is when `PROMPT.md` sets the review gear to **review-after**: then the user reviews the phase as a batch at its end, and this pause is the only thing dropped.
  3. Once approved, watch it fail for the right reason, then write the least code that makes it pass.
  4. Refactor.
- **A test that passes first time** hasn't shown it can fail. Don't weaken it. Instead, apply a named mutation to *production* code, see the test fail on its assertion, revert the mutation, and say in the commit message which mutation you used.
- **Commits.**
  - Behaviour goes in a `feat:` or `test:` commit.
  - Ticking the task in `tasks.md` and updating `PROMPT.md` goes in a separate `docs:` commit.
  - Structural changes (`refactor:`) never share a commit with behavioural ones, and come first ("Tidy First").
  - Run the full test suite before every commit.
- **Look for code smells after each phase**, and refactor before opening the phase's PR.
- **Pace.** Stop after each task for review until the user approves the approach, then work a phase at a time. `PROMPT.md` records the current pace.
- **Ask before pushing, merging or deleting branches.**

## Design

- **Narrow and deep modules.** An assembly is a module. It should have a small public surface that hides substantial behaviour.
  - `Oyez.Core`'s boundary is:
    - the Brighter commands and events, and the Darker queries, with their results;
    - the ports (`IAmA…` interfaces) that adapters implement, with the types in their signatures;
    - the `InMemory…` implementations of those ports;
    - configuration types;
    - `AddOyezApplication`.
  - Parsing, counting, rendering, validation and the publishing workflow are **internal**.
  - `Oyez.Adapters`' boundary is the port implementations and their registration. HTTP handlers, signing and similar details are internal.
- **Visibility.** A type is `public` only if something outside its assembly uses it. Never use `InternalsVisibleTo`.
  - If a test needs to inject randomness, time or I/O, make that seam public and record why. `TimeProvider` is the clock.
- **Responsibility-driven design.** Every object has one role (single role principle). Name its collaborators.
- **Interfaces.**
  - Name an interface for a role `IAmA…` or `IAmAn…`, for example `IAmAChannelPublisher`.
  - Internal types get an interface only where there is real optionality, such as a strategy.
- **Avoid primitive obsession.** Use a record or struct for a value with domain meaning. Primitives are fine at serialisation boundaries.
- **Keep methods small.** No more than one level of indentation in a method. Use explaining variables and extracted methods for concepts.
- **Don't add speculative code** or new types without a need.

## Code style

- Follow Microsoft's C# naming conventions, except:
  - constants are `ALL_CAPS_WITH_UNDERSCORES`;
  - Microsoft's Framework Design Guidelines are *not* followed.
- Prefer primary constructors, expression-bodied members and `readonly` fields.
- Nullable reference types are on. Make a type nullable to show that it's optional.
- Use one type per file, unless one type is clearly a detail of another.
- Suffix async methods with `Async`.
- **Comments.**
  - Public and protected members of public types get `///` XML documentation, which is required. Use `<summary>`, `<param>`, `<returns>` and `<value>` for how to use the API, and `<remarks>` for notes.
  - Internal and private code gets no XML documentation. At most, put one `//` comment immediately before a method's signature, when the method is complex or guards against a trap.
  - No comments inside method bodies. Extract a method, or use an explaining variable, instead.
  - No references to issues, PRs or specs, and no change history. Delete commented-out code.
- **Licence header.** Every file in `src/` starts with the MIT licence in a `#region Licence` block, as below.

  ```csharp
  #region Licence

  /* The MIT License (MIT)
  Copyright © 2026 Ian Cooper

  … (full MIT text, as in LICENSE) …
  */

  #endregion
  ```

## Tests

- Use xUnit v3. Tests run on Microsoft Testing Platform, so use `dotnet test`.
- **Test only through a module's public boundary.** Never test internal types, and never widen the public surface just to test something.
  - Core behaviour is tested by sending Brighter commands and running Darker queries through the real processors, built by `AddOyezApplication`.
  - Use `InMemory…` port implementations in place of I/O.
- **Naming.**
  - Test methods are named `When_[condition]_should_[expected_behaviour]`.
  - Put one test per file, and name the file after the test method.
  - A test class is named `[Behaviour]Tests`, never `When_…`.
- **Structure.**
  - Mark the sections with `//Arrange`, `//Act` and `//Assert`.
  - Use evident data: make obvious the data that affects the outcome. Use a test data builder to hide noise.
- **No mocks for isolating a class.** Test doubles are only for I/O, the clock or randomness. Prefer `InMemory…` implementations to fakes, and fakes to mocks.
- **Test the production path.** Build objects the way production builds them, through DI.

## Build

```sh
dotnet build Oyez.slnx
dotnet test
```
