NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.

# TASK-005 - Architecture and DI Verification

## Source
- CLAUDE-REPORT section 5 (Architecture and Design Patterns)

## Objective
- Validate dependency injection registrations and document architecture boundaries.

## Tasks
1. Confirm all hosted services are registered exactly once and are resolvable.
2. Document layered responsibilities (Discord layer, services, business, data).
3. Add a DI validation test that builds the host and resolves critical services.

## Implementation Notes
- Service registration lives in `nhitomi/Startup.cs`.

## Code Snippet
```csharp
services
   .AddSingleton<DiscordService>()
   .AddSingleton<CommandExecutor>()
   .AddSingleton<InteractiveManager>()
   .AddHostedInjectableService<MessageHandlerService>()
   .AddHostedInjectableService<ReactionHandlerService>();
```

## Acceptance Criteria
- The host can be constructed with no missing registrations.
- Architecture documentation is consistent with the current codebase.
