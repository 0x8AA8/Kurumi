NOTE FOR CLAUDE CODE: Do not make assumptions, mock data, or placeholders. Always implement production-ready code.

# TASK-009 - Doujin Source Client Reliability

## Source
- CLAUDE-REPORT section 9 (Doujin Source Clients)

## Objective
- Improve source client resilience, error handling, and data normalization.

## Tasks
1. Add HTTP timeouts, retry logic, and consistent user-agent configuration.
2. Normalize tag values and metadata casing consistently across sources.
3. Expand integration coverage for source client edge cases without mock data.

## Implementation Notes
- Clients are implemented in `nhitomi.Core/Clients/nhentai` and `nhitomi.Core/Clients/Hitomi`.

## Code Snippet
```csharp
using (var response = await _http.SendAsync(
    new HttpRequestMessage
    {
        Method     = HttpMethod.Get,
        RequestUri = new Uri(nhentai.Gallery(intId))
    },
    cancellationToken))
{
    if (!response.IsSuccessStatusCode)
        return null;

    using (var textReader = new StringReader(await response.Content.ReadAsStringAsync()))
    using (var jsonReader = new JsonTextReader(textReader))
        data = _serializer.Deserialize<nhentai.DoujinData>(jsonReader);
}
```

## Acceptance Criteria
- Client failures are handled gracefully with meaningful logs.
- Metadata outputs are consistent across sources.
