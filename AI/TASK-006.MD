NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.

# TASK-006 - Database Schema and Migration Integrity

## Source
- CLAUDE-REPORT section 6 (Database Schema)

## Objective
- Validate schema design, indices, and migrations for current usage.

## Tasks
1. Review entity relationships and indices for search and filtering.
2. Validate migrations against the current model snapshot.
3. Add missing constraints or indices required by query patterns.

## Implementation Notes
- Entity definitions and indices are in `nhitomi.Core/*.cs` and `nhitomi.Core/nhitomiDbContext.cs`.
- Migrations are in `nhitomi.Core/Migrations`.

## Code Snippet
```csharp
doujin.HasIndex(d => d.AccessId).IsUnique();

doujin.HasIndex(d => new { d.Source, d.SourceId });
doujin.HasIndex(d => new { d.Source, d.UploadTime });

doujin.HasIndex(d => d.UploadTime);
doujin.HasIndex(d => d.ProcessTime);
```

## Acceptance Criteria
- Database indices match the most common query paths.
- Migrations are consistent with the model snapshot.
