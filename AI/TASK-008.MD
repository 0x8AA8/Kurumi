NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.

# TASK-008 - Command System Validation

## Source
- CLAUDE-REPORT section 8 (Command System)

## Objective
- Strengthen command parsing, binding, and execution reliability.

## Tasks
1. Add tests for binding syntax and option parsing.
2. Validate command discovery order and alias collisions.
3. Add logging for command parse failures with input context.

## Implementation Notes
- Command discovery and parsing are implemented in `nhitomi/Discord/CommandExecutor.cs` and `nhitomi/Discord/Parsing/CommandInfo.cs`.

## Code Snippet
```csharp
_commands.AddRange(
    typeof(Startup)
       .Assembly
       .GetTypes()
       .Where(t => !t.IsAbstract && t.IsClass)
       .SelectMany(t => t.GetMethods())
       .Where(t => t.GetCustomAttribute<CommandAttribute>() != null)
       .OrderBy(t => t.Name)
       .ThenByDescending(t => t.GetParameters().Length)
       .Select(t => new CommandInfo(t)));
```

## Acceptance Criteria
- Commands resolve deterministically and tests cover edge cases.
- Command parsing failures are logged with actionable details.
