# Kurumi Project Report

Generated: 2026-02-03
Location: C:\Users\Usui\Desktop\DevOPS\Kurumi
Scope: Full repository index and code analysis

## Summary
Kurumi (nhitomi) is a Discord bot built on .NET Core that indexes and serves doujin metadata from multiple sources (notably nhentai and hitomi). It uses EF Core with MySQL for persistence, a custom command parsing system, and an interactivity layer that drives embeds and reaction-based navigation.

## Quick Stats
- Total files (excluding .git): 141
- Non-build files (excluding obj): 108
- Approx repository size: 2.2 MB
- Source lines (excluding obj and binaries):
  - .cs: 8925 lines across 89 files
  - .json: 335 lines across 6 files
  - .md: 165 lines across 2 files
  - .csproj: 98 lines across 3 files
  - .sln: 61 lines across 1 file

## Repository Structure
- Solution: `nhitomi.sln` with three projects:
  - `nhitomi` (app / Discord bot)
  - `nhitomi.Core` (core library, data model, clients, utilities)
  - `nhitomi.Core.UnitTests` (client tests)
- Notable top-level files:
  - `README.md`, `nhitomi_README.md` (usage, commands, setup)
  - `Dockerfile` (build and runtime image pipeline)
  - `appveyor.yml` (CI build config)
  - `renovate.json` (dependency update config)
  - `LICENSE` (MIT)
- Images: `Kurumi-sama.png`, `nhitomi.png`
- .github contains empty directories: `.github\appmod` and `.github\appcat`

## Entry Points and Runtime Flow
- `nhitomi\Program.cs`
  - Builds a generic host, configures services and config, migrates DB in Development, then connects to Discord and runs.
- `nhitomi\Startup.cs`
  - Loads configuration from `appsettings.json`, `appsettings.{ENV}.json`, and environment variables.
  - Registers logging, EF Core DbContext (MySQL via Pomelo), Discord services, interactivity, HTTP client, JSON serializer, and a forced GC background service.

## Configuration
- `nhitomi\appsettings.json`
  - Logging levels, Discord settings (prefix, cache size, status messages, guild IDs), feed toggle.
- `nhitomi\appsettings.Development.json`
  - Overrides log levels and command prefix.
- Expected but missing in repo:
  - `appsecrets.json` (Discord token) per README instructions.
  - Connection string for `nhitomi` (used by `GetConnectionString("nhitomi")`) is not present in default appsettings.
- Environment:
  - `ENVIRONMENT` variable selects environment (defaults to Development).

## Core Library (nhitomi.Core)
### Data Model (EF Core)
- `Doujin` (core content entity) with tags, source metadata, page count, and denormalized tag search field.
- `Tag`, `TagRef` (many-to-many between tags and doujins).
- `Collection`, `CollectionRef` (user collections of doujins).
- `Guild` (per-Discord-server settings).
- `FeedChannel`, `FeedChannelTag` (feed channels and whitelist tags).
- `nhitomiDbContext` provides high-level query APIs and includes a MySQL full-text search strategy.

### Doujin Clients
- `IDoujinClient` defines fetch, enumeration, and page URL population.
- `Clients\Hitomi\HitomiClient.cs`
  - Parses HTML, loads gallery metadata, and processes image metadata from JS.
- `Clients\nhentai\nhentaiClient.cs`
  - Calls JSON API, builds page URLs from media id and extensions.
- `Clients\ClientTester.cs` and `ClientTestCase` provide verification against known fixtures.

### Utilities
- `Extensions.cs` provides a large set of helpers (HTTP cloning, JSON serialization, string distance, chunking, etc.).
- `HashUtility.cs` exposes thread-static SHA256 usage.
- `HttpClientWrapper.cs` wraps HttpClient for streaming responses.
- `nhitomiSerializerSettings.cs` standardizes JSON settings.
- `Models\*` contains an alternate/legacy model set (not referenced in DbContext).

## Discord Layer (nhitomi)
### Command and Event Pipeline
- `DiscordService` extends `DiscordSocketClient` and manages readiness and connection.
- `MessageHandlerService` wires Discord message events to message handlers.
- `ReactionHandlerService` wires reaction events to reaction handlers.
- `CommandExecutor` loads command metadata via reflection and handles command invocation.
- `Discord\Parsing` contains custom attributes and parsing logic (`CommandAttribute`, `ModuleAttribute`, `BindingAttribute`, `CommandInfo`).

### Modules (Commands)
- `DoujinModule` (get, search, download, read, list by source).
- `CollectionModule` and `CollectionListModule` (user collections, add/remove/sort/list).
- `OptionModule` (language, admin-gated).
- `FeedModule` (feed tag whitelist management, admin-gated).
- `HelpModule` (help and debug embeds).

### Interactivity
- `InteractiveManager` manages interactive messages and reaction triggers.
- Message types (embeds and lists) live in `nhitomi\Interactivity\*`.
- Reaction triggers (download, read, list paging, delete, favorite) live under `Interactivity\Triggers`.
- `GalleryUrlDetector` auto-detects gallery links in messages and renders interactive responses.

## Localization
- Embedded JSON resources in `Globalization\Languages\` (en, id, ko).
- `Localization.cs` loads and selects dictionaries; English is the default fallback.

## Tests
- `nhitomi.Core.UnitTests` contains NUnit tests for the source clients.
- Tests rely on live data from external sites (integration style) and fixtures in `Clients\Hitomi\Hitomi*.cs` and `Clients\nhentai\nhentai*.cs`.

## CI / Build / Packaging
- `appveyor.yml` builds with Visual Studio 2017, tests disabled.
- `Dockerfile` uses deprecated `microsoft/dotnet` images and publishes `nhitomi` to a runtime image.
- `renovate.json` extends base configuration for dependency updates.

## Notable Observations and Risks
- Target frameworks are netcoreapp2.2 and netstandard2.0, which are end-of-life.
- Dockerfile uses old `microsoft/dotnet` image names; modern tags are `mcr.microsoft.com/dotnet/*`.
- `ForcedGarbageCollector` runs GC every 10 seconds and is explicitly labeled as problematic in code comments.
- `nhitomi.Core\Models` appears to be a legacy model layer separate from the EF Core entities in root.
- Tests are integration-style and depend on external services; they are not deterministic and may fail if upstream data changes.
- Build artifacts (`obj` and cache files) are present in the repo.

## File Index (non-obj files)
```
.gitignore
AI\CLAUDE-REPORT.MD
AI\REPORT.MD
appveyor.yml
Dockerfile
Kurumi-sama.png
LICENSE
nhitomi_README.md
nhitomi.Core.UnitTests\Clients\ClientTests.cs
nhitomi.Core.UnitTests\nhitomi.Core.UnitTests.csproj
nhitomi.Core.UnitTests\TestUtils.cs
nhitomi.Core\AtomicCounter.cs
nhitomi.Core\Clients\ClientTestCase.cs
nhitomi.Core\Clients\ClientTester.cs
nhitomi.Core\Clients\Hitomi\Hitomi1422964.cs
nhitomi.Core\Clients\Hitomi\Hitomi1425272.cs
nhitomi.Core\Clients\Hitomi\Hitomi1425386.cs
nhitomi.Core\Clients\Hitomi\HitomiClient.cs
nhitomi.Core\Clients\nhentai\nhentai250000.cs
nhitomi.Core\Clients\nhentai\nhentai273650.cs
nhitomi.Core\Clients\nhentai\nhentai82843.cs
nhitomi.Core\Clients\nhentai\nhentaiClient.cs
nhitomi.Core\Collection.cs
nhitomi.Core\CollectionSort.cs
nhitomi.Core\Doujin.cs
nhitomi.Core\DoujinInfo.cs
nhitomi.Core\Extensions.cs
nhitomi.Core\FeedChannel.cs
nhitomi.Core\Guild.cs
nhitomi.Core\HashUtility.cs
nhitomi.Core\HttpClientWrapper.cs
nhitomi.Core\IDoujinClient.cs
nhitomi.Core\IgnoredAttribute.cs
nhitomi.Core\IHttpClient.cs
nhitomi.Core\Migrations\20190610042252_MergeMigrations.cs
nhitomi.Core\Migrations\20190610042252_MergeMigrations.Designer.cs
nhitomi.Core\Migrations\nhitomiDbContextModelSnapshot.cs
nhitomi.Core\Models\Doujin.cs
nhitomi.Core\Models\DoujinCollection.cs
nhitomi.Core\Models\DoujinMeta.cs
nhitomi.Core\Models\DoujinSource.cs
nhitomi.Core\Models\ModelBase.cs
nhitomi.Core\nhitomi.Core.csproj
nhitomi.Core\nhitomiDbContext.cs
nhitomi.Core\nhitomiSerializerSettings.cs
nhitomi.Core\Tag.cs
nhitomi.png
nhitomi.sln
nhitomi\AppSettings.cs
nhitomi\appsettings.Development.json
nhitomi\appsettings.json
nhitomi\AsyncEnumerableBrowser.cs
nhitomi\DependencyUtility.cs
nhitomi\Discord\CommandExecutor.cs
nhitomi\Discord\DiscordErrorReporter.cs
nhitomi\Discord\DiscordService.cs
nhitomi\Discord\FeedChannelUpdateService.cs
nhitomi\Discord\GalleryUrlDetector.cs
nhitomi\Discord\GuildSettingsSyncService.cs
nhitomi\Discord\GuildWelcomeMessageService.cs
nhitomi\Discord\LogHandlerService.cs
nhitomi\Discord\MessageHandlerService.cs
nhitomi\Discord\Parsing\BindingAttribute.cs
nhitomi\Discord\Parsing\CommandAttribute.cs
nhitomi\Discord\Parsing\CommandInfo.cs
nhitomi\Discord\Parsing\ModuleAttribute.cs
nhitomi\Discord\Parsing\OptionAttribute.cs
nhitomi\Discord\ReactionHandlerService.cs
nhitomi\Discord\StatusUpdateService.cs
nhitomi\ForcedGarbageCollector.cs
nhitomi\GalleryUtility.cs
nhitomi\Globalization\Languages\en.json
nhitomi\Globalization\Languages\id.json
nhitomi\Globalization\Languages\ko.json
nhitomi\Globalization\Localization.cs
nhitomi\Globalization\LocalizationDictionary.cs
nhitomi\Interactivity\CollectionListMessage.cs
nhitomi\Interactivity\CollectionMessage.cs
nhitomi\Interactivity\CommandHelpMessage.cs
nhitomi\Interactivity\DoujinListFromQueryMessage.cs
nhitomi\Interactivity\DoujinListFromSourceMessage.cs
nhitomi\Interactivity\DoujinListMessage.cs
nhitomi\Interactivity\DoujinMessage.cs
nhitomi\Interactivity\DoujinReadMessage.cs
nhitomi\Interactivity\DownloadMessage.cs
nhitomi\Interactivity\EmbedMessage.cs
nhitomi\Interactivity\ErrorMessage.cs
nhitomi\Interactivity\HelpMessage.cs
nhitomi\Interactivity\InteractiveManager.cs
nhitomi\Interactivity\InteractiveMessage.cs
nhitomi\Interactivity\ListMessage.cs
nhitomi\Interactivity\Triggers\DeleteTrigger.cs
nhitomi\Interactivity\Triggers\DownloadTrigger.cs
nhitomi\Interactivity\Triggers\FavoriteTrigger.cs
nhitomi\Interactivity\Triggers\ListJumpTrigger.cs
nhitomi\Interactivity\Triggers\ListTrigger.cs
nhitomi\Interactivity\Triggers\ReactionTrigger.cs
nhitomi\Interactivity\Triggers\ReadTrigger.cs
nhitomi\Modules\CollectionModule.cs
nhitomi\Modules\DoujinModule.cs
nhitomi\Modules\FeedModule.cs
nhitomi\Modules\HelpModule.cs
nhitomi\Modules\OptionModule.cs
nhitomi\nhitomi.csproj
nhitomi\Program.cs
nhitomi\Startup.cs
nhitomi\VersionHelper.cs
README.md
renovate.json
```

## Build Artifacts (obj directories)
```
nhitomi.Core.UnitTests\obj\Debug\netcoreapp2.2\nhitomi.Core.UnitTests.AssemblyInfo.cs
nhitomi.Core.UnitTests\obj\Debug\netcoreapp2.2\nhitomi.Core.UnitTests.AssemblyInfoInputs.cache
nhitomi.Core.UnitTests\obj\Debug\netcoreapp2.2\nhitomi.Core.UnitTests.assets.cache
nhitomi.Core.UnitTests\obj\Debug\netcoreapp2.2\nhitomi.Core.UnitTests.csproj.AssemblyReference.cache
nhitomi.Core.UnitTests\obj\Debug\netcoreapp2.2\nhitomi.Core.UnitTests.GeneratedMSBuildEditorConfig.editorconfig
nhitomi.Core.UnitTests\obj\nhitomi.Core.UnitTests.csproj.nuget.dgspec.json
nhitomi.Core.UnitTests\obj\nhitomi.Core.UnitTests.csproj.nuget.g.props
nhitomi.Core.UnitTests\obj\nhitomi.Core.UnitTests.csproj.nuget.g.targets
nhitomi.Core.UnitTests\obj\project.assets.json
nhitomi.Core.UnitTests\obj\project.nuget.cache
nhitomi.Core\obj\Debug\netstandard2.0\nhitomi.Core.AssemblyInfo.cs
nhitomi.Core\obj\Debug\netstandard2.0\nhitomi.Core.AssemblyInfoInputs.cache
nhitomi.Core\obj\Debug\netstandard2.0\nhitomi.Core.assets.cache
nhitomi.Core\obj\Debug\netstandard2.0\nhitomi.Core.csproj.AssemblyReference.cache
nhitomi.Core\obj\Debug\netstandard2.0\nhitomi.Core.GeneratedMSBuildEditorConfig.editorconfig
nhitomi.Core\obj\nhitomi.Core.csproj.nuget.dgspec.json
nhitomi.Core\obj\nhitomi.Core.csproj.nuget.g.props
nhitomi.Core\obj\nhitomi.Core.csproj.nuget.g.targets
nhitomi.Core\obj\project.assets.json
nhitomi.Core\obj\project.nuget.cache
nhitomi\obj\Debug\netcoreapp2.2\nhitomi.AssemblyInfo.cs
nhitomi\obj\Debug\netcoreapp2.2\nhitomi.AssemblyInfoInputs.cache
nhitomi\obj\Debug\netcoreapp2.2\nhitomi.assets.cache
nhitomi\obj\Debug\netcoreapp2.2\nhitomi.csproj.AssemblyReference.cache
nhitomi\obj\Debug\netcoreapp2.2\nhitomi.GeneratedMSBuildEditorConfig.editorconfig
nhitomi\obj\nhitomi.csproj.nuget.dgspec.json
nhitomi\obj\nhitomi.csproj.nuget.g.props
nhitomi\obj\nhitomi.csproj.nuget.g.targets
nhitomi\obj\project.assets.json
nhitomi\obj\project.nuget.cache
```
