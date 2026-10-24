using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using Temple.Application.Interfaces;
using Temple.Application.Saves;

namespace Temple.Infrastructure.IO;

public sealed class JsonGameSaveStore(string directoryPath) : IGameSaveStore
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        TypeNameHandling = TypeNameHandling.None,
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Error,
        MaxDepth = 32,
        Formatting = Formatting.Indented,
        ContractResolver = new SaveContractResolver()
    };

    public async Task WriteAsync(int slot, GameSave save, CancellationToken cancellationToken = default)
    {
        var path = SlotPath(slot);
        ArgumentNullException.ThrowIfNull(save);
        var json = JsonConvert.SerializeObject(save, Settings);
        cancellationToken.ThrowIfCancellationRequested();
        Directory.CreateDirectory(directoryPath);
        var temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            await using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write,
                FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                await using var writer = new StreamWriter(stream, leaveOpen: true);
                await writer.WriteAsync(json.AsMemory(), cancellationToken);
                await writer.FlushAsync(cancellationToken);
                stream.Flush(flushToDisk: true);
            }
            cancellationToken.ThrowIfCancellationRequested();
            // Temp and destination share a directory/volume. Never delete the old save first.
            File.Move(temporary, path, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporary)) File.Delete(temporary);
        }
    }

    public async Task<GameSave> ReadAsync(int slot, CancellationToken cancellationToken = default)
    {
        var json = await File.ReadAllTextAsync(SlotPath(slot), cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            return JsonConvert.DeserializeObject<GameSave>(json, Settings)
                ?? throw new InvalidDataException("The save file is empty.");
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException("The save file is not valid game data.", ex);
        }
    }

    private string SlotPath(int slot)
    {
        if (slot is < 1 or > 3) throw new ArgumentOutOfRangeException(nameof(slot));
        return Path.Combine(directoryPath, $"slot-{slot}.json");
    }

    private sealed class SaveContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            // Missing v1 fields must not silently turn into zero HP or default quest progress.
            // Null position fields are valid and are written explicitly by this store.
            if (member.DeclaringType == typeof(GameSave) || member.DeclaringType == typeof(PartyMemberSave) ||
                member.DeclaringType == typeof(QuestProgressSave))
                property.Required = Required.AllowNull;
            return property;
        }
    }
}
