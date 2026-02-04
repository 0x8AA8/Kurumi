using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace nhitomi.Core.Clients.Hitomi
{
    public static class Hitomi
    {
        // API domain changed from hitomi.la to gold-usergeneratedcontent.net
        private const string ApiDomain = "gold-usergeneratedcontent.net";

        public static string Gallery(int id) => $"https://hitomi.la/galleries/{id}.html";

        public static string GalleryInfo(int id) => $"https://ltn.{ApiDomain}/galleries/{id}.js";

        static char GetCdn(int id) => (char) ('a' + (id % 10 == 1 ? 0 : id) % 2);

        public static string Image(int id,
                                   string name) => $"https://{GetCdn(id)}a.hitomi.la/galleries/{id}/{name}";

        public const string NozomiIndex = $"https://ltn.{ApiDomain}/index-all.nozomi";
    }

    public sealed class HitomiClient : IDoujinClient
    {
        public string Name => nameof(Hitomi);
        public string Url => "https://hitomi.la/";

        readonly IHttpClient _http;
        readonly JsonSerializer _serializer;
        readonly ILogger<HitomiClient> _logger;

        public HitomiClient(IHttpClient http,
                            JsonSerializer serializer,
                            ILogger<HitomiClient> logger)
        {
            _http       = http;
            _serializer = serializer;
            _logger     = logger;
        }

        // regex to match () and [] in titles
        static readonly Regex _bracketsRegex = new Regex(@"\([^)]*\)|\[[^\]]*\]",
                                                         RegexOptions.Compiled | RegexOptions.Singleline);

        public static string GetGalleryUrl(Doujin doujin) => $"https://hitomi.la/galleries/{doujin.SourceId}.html";

        public async Task<DoujinInfo> GetAsync(string id,
                                               CancellationToken cancellationToken = default)
        {
            if (!int.TryParse(id, out var intId))
                return null;

            // Fetch gallery info from JSON API
            using (var response = await _http.SendAsync(
                new HttpRequestMessage
                {
                    Method     = HttpMethod.Get,
                    RequestUri = new Uri(Hitomi.GalleryInfo(intId))
                },
                cancellationToken))
            {
                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();

                // The response is JavaScript: "var galleryinfo = {...}"
                // We need to strip the prefix to get the JSON
                var jsonStart = content.IndexOf('{');
                if (jsonStart == -1)
                    return null;

                var jsonContent = content.Substring(jsonStart);

                GalleryInfo gallery;
                using (var textReader = new StringReader(jsonContent))
                using (var jsonReader = new JsonTextReader(textReader))
                {
                    gallery = _serializer.Deserialize<GalleryInfo>(jsonReader);
                }

                if (gallery == null)
                    return null;

                // Filter out anime
                if (gallery.Type != null && gallery.Type.Equals("anime", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation($"Skipping '{id}' because it is of type 'anime'.");
                    return null;
                }

                // Parse names
                var prettyName = gallery.Title ?? "";
                var originalName = gallery.JapaneseTitle ?? prettyName;

                // Replace stuff in brackets with nothing
                prettyName = _bracketsRegex.Replace(prettyName, "").Trim();

                if (string.IsNullOrEmpty(prettyName))
                    prettyName = originalName;

                // Parse names with two parts (separated by |)
                var pipeIndex = prettyName.IndexOf('|');
                if (pipeIndex != -1)
                {
                    var temp = prettyName;
                    prettyName = temp.Substring(0, pipeIndex).Trim();
                    originalName = temp.Substring(pipeIndex + 1).Trim();
                }

                // Parse upload time
                DateTime uploadTime = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(gallery.Date))
                {
                    if (DateTime.TryParse(gallery.Date, out var parsed))
                        uploadTime = parsed.ToUniversalTime();
                }

                // Build doujin info
                var doujin = new DoujinInfo
                {
                    PrettyName   = prettyName,
                    OriginalName = originalName,
                    UploadTime   = uploadTime,
                    Source       = this,
                    SourceId     = id,

                    Artist     = gallery.Artists?.FirstOrDefault()?.Name?.ToLowerInvariant(),
                    Group      = gallery.Groups?.FirstOrDefault()?.Name?.ToLowerInvariant(),
                    Language   = gallery.Language?.ToLowerInvariant(),
                    Parody     = ConvertSeries(gallery.Parodys?.FirstOrDefault()?.Name)?.ToLowerInvariant(),
                    Characters = gallery.Characters?.Select(c => c.Name?.ToLowerInvariant()).Where(n => n != null),
                    Tags       = gallery.Tags?.Select(t => ConvertTag(t))?.Where(t => t != null)
                };

                // Parse images
                if (gallery.Files != null && gallery.Files.Length > 0)
                {
                    var extensionsCombined = new string(gallery.Files.Select(i =>
                    {
                        var ext = Path.GetExtension(i.Name);
                        switch (ext)
                        {
                            case "":      return '.';
                            case ".jpg":  return 'j';
                            case ".jpeg": return 'J';
                            case ".png":  return 'p';
                            case ".gif":  return 'g';
                            case ".webp": return 'w';
                            default:      return 'j'; // Default to jpg for unknown
                        }
                    }).ToArray());

                    doujin.PageCount = gallery.Files.Length;
                    doujin.Data = _serializer.Serialize(new InternalDoujinData
                    {
                        ImageNames = gallery.Files.Select(i => Path.GetFileNameWithoutExtension(i.Name)).ToArray(),
                        Extensions = extensionsCombined
                    });
                }

                return doujin;
            }
        }

        // JSON model classes for the Hitomi API response
        sealed class GalleryInfo
        {
            [JsonProperty("title")] public string Title;
            [JsonProperty("japanese_title")] public string JapaneseTitle;
            [JsonProperty("id")] public string Id;
            [JsonProperty("language")] public string Language;
            [JsonProperty("type")] public string Type;
            [JsonProperty("date")] public string Date;
            [JsonProperty("artists")] public ArtistInfo[] Artists;
            [JsonProperty("groups")] public GroupInfo[] Groups;
            [JsonProperty("characters")] public CharacterInfo[] Characters;
            [JsonProperty("parodys")] public ParodyInfo[] Parodys;
            [JsonProperty("tags")] public TagInfo[] Tags;
            [JsonProperty("files")] public FileInfo[] Files;
        }

        sealed class ArtistInfo
        {
            [JsonProperty("artist")] public string Name;
        }

        sealed class GroupInfo
        {
            [JsonProperty("group")] public string Name;
        }

        sealed class CharacterInfo
        {
            [JsonProperty("character")] public string Name;
        }

        sealed class ParodyInfo
        {
            [JsonProperty("parody")] public string Name;
        }

        sealed class TagInfo
        {
            [JsonProperty("tag")] public string Name;
            [JsonProperty("female")] public string Female;
            [JsonProperty("male")] public string Male;
        }

        sealed class FileInfo
        {
            [JsonProperty("name")] public string Name;
            [JsonProperty("width")] public int Width;
            [JsonProperty("height")] public int Height;
            [JsonProperty("hash")] public string Hash;
        }

        sealed class InternalDoujinData
        {
            [JsonProperty("n")] public string[] ImageNames;
            [JsonProperty("e")] public string Extensions;
        }

        static string ConvertSeries(string series) =>
            series == null || series.Equals("original", StringComparison.OrdinalIgnoreCase) ? null : series;

        static string ConvertTag(TagInfo tag)
        {
            if (tag?.Name == null)
                return null;

            // Return just the tag name without gender prefix
            // (matching original behavior)
            return tag.Name.ToLowerInvariant();
        }

        async Task<int[]> ReadNozomiIndicesAsync(CancellationToken cancellationToken = default)
        {
            using (var memory = new MemoryStream())
            {
                using (var response = await _http.SendAsync(
                    new HttpRequestMessage
                    {
                        Method     = HttpMethod.Get,
                        RequestUri = new Uri(Hitomi.NozomiIndex)
                    },
                    cancellationToken))
                {
                    if (!response.IsSuccessStatusCode)
                        return null;

                    using (var stream = await response.Content.ReadAsStreamAsync())
                        await stream.CopyToAsync(memory, 4096, cancellationToken);

                    memory.Position = 0;
                }

                var indices = new int[memory.Length / sizeof(int)];

                using (var reader = new BinaryReader(memory))
                {
                    for (var i = 0; i < indices.Length; i++)
                        indices[i] = reader.ReadInt32Be();
                }

                return indices;
            }
        }

        public async Task<IEnumerable<string>> EnumerateAsync(string startId = null,
                                                              CancellationToken cancellationToken = default)
        {
            var indices = await ReadNozomiIndicesAsync(cancellationToken);

            if (indices == null)
                return null;

            Array.Sort(indices);

            // skip to starting id
            int.TryParse(startId, out var intId);

            var startIndex = 0;

            for (; startIndex < indices.Length; startIndex++)
            {
                if (indices[startIndex] >= intId)
                    break;
            }

            indices = indices.Subarray(startIndex);

            return indices.Select(x => x.ToString());
        }

        public IEnumerable<string> PopulatePages(Doujin doujin)
        {
            if (!int.TryParse(doujin.SourceId, out var intId))
                yield break;

            var data = _serializer.Deserialize<InternalDoujinData>(doujin.Data);

            if (data.ImageNames == null || data.Extensions == null)
                yield break;

            for (var i = 0; i < data.ImageNames.Length; i++)
            {
                var    name = data.ImageNames[i];
                string extension;

                switch (data.Extensions[i])
                {
                    case '.':
                        extension = "";
                        break;
                    case 'p':
                        extension = ".png";
                        break;
                    case 'J':
                        extension = ".jpeg";
                        break;
                    case 'g':
                        extension = ".gif";
                        break;
                    case 'w':
                        extension = ".webp";
                        break;
                    default:
                        extension = ".jpg";
                        break;
                }

                yield return Hitomi.Image(intId, name + extension);
            }
        }

        public void InitializeImageRequest(Doujin doujin,
                                           HttpRequestMessage message) => message.Headers.Referrer =
            new Uri($"https://hitomi.la/reader/{doujin.SourceId}.html");

        public void Dispose() { }
    }
}