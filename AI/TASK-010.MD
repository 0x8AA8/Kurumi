NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.

# TASK-010 - Interactive Message System Robustness

## Source
- CLAUDE-REPORT section 10 (Interactive Message System)

## Objective
- Ensure interactive messages, reactions, and cleanup behavior are stable under load.

## Tasks
1. Validate interactive message registration and expiration behavior.
2. Ensure reaction handling handles missing permissions and deleted messages safely.
3. Add tests for trigger routing and stateless triggers.

## Implementation Notes
- Interactive lifecycle is managed in `nhitomi/Interactivity/InteractiveManager.cs`.

## Code Snippet
```csharp
if (message is IInteractiveMessage interactiveMessage)
    if (forceStateful || interactiveMessage.Triggers.Values.Any(t => !t.CanRunStateless))
        InteractiveMessages[id] = interactiveMessage;

_ = Task.Run(async () =>
{
    try
    {
        await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
    }
    catch (TaskCanceledException) { }
    finally
    {
        InteractiveMessages.TryRemove(id, out _);
    }
}, cancellationToken);
```

## Acceptance Criteria
- Interactive messages expire on schedule and do not leak memory.
- Reaction handling does not crash on missing state.
