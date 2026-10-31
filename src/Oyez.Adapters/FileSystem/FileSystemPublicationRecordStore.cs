#region Licence

/* The MIT License (MIT)
Copyright © 2026 Ian Cooper

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

#endregion

using System.Text.Json;
using System.Text.Json.Serialization;
using Oyez.Core.Announcements;
using Oyez.Core.Publishing;

namespace Oyez.Adapters.FileSystem;

internal sealed class FileSystemPublicationRecordStore(string root) : IAmAPublicationRecordStore
{
    private const string RECORD_FILE = "published.json";

    private static readonly JsonSerializerOptions JSON_OPTIONS = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public PublicationRecord Load(AnnouncementId id)
    {
        var path = Path.Combine(root, id.Value, RECORD_FILE);
        return File.Exists(path)
            ? JsonSerializer.Deserialize<PublicationRecordFile>(File.ReadAllText(path), JSON_OPTIONS)!.ToRecord()
            : PublicationRecord.Empty;
    }

    public void Save(AnnouncementId id, PublicationRecord record)
    {
        var folder = Path.Combine(root, id.Value);
        Directory.CreateDirectory(folder);
        var temporaryPath = Path.Combine(folder, $"{RECORD_FILE}.{Guid.NewGuid():N}.tmp");
        File.WriteAllText(temporaryPath, JsonSerializer.Serialize(PublicationRecordFile.From(record), JSON_OPTIONS));
        File.Move(temporaryPath, Path.Combine(folder, RECORD_FILE), overwrite: true);
    }
}
