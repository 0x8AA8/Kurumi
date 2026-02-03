NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.

# TASK-017 - File Inventory Automation

## Source
- CLAUDE-REPORT section 17 (File Inventory)

## Objective
- Automate file inventory generation and keep it consistent across reports.

## Tasks
1. Create a script that lists non-build files and categorizes them by type.
2. Save inventory output into AI reports on demand.
3. Ensure inventory excludes `obj` and `bin` directories.

## Implementation Notes
- A PowerShell-based inventory script fits the existing Windows environment.

## Code Snippet
```powershell
$root = Get-Location
Get-ChildItem -Recurse -File |
  Where-Object { $_.FullName -notmatch '\\obj\\' -and $_.FullName -notmatch '\\bin\\' } |
  ForEach-Object { $_.FullName.Substring($root.Path.Length + 1) } |
  Sort-Object
```

## Acceptance Criteria
- Inventory output is reproducible and excludes build artifacts.
- Output is stored in the AI folder when generated.
