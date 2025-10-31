using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.MongoDB;

namespace AlphaAgentWebApi.Stores;
internal sealed class VectorChatMessageStore : ChatMessageStore
{
    private readonly MongoVectorStore _mongoVectorStore;
    public VectorChatMessageStore(
        MongoVectorStore mongoVectorStore,
        JsonElement serializedStoreState,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        this._mongoVectorStore = mongoVectorStore ?? throw new ArgumentNullException(nameof(mongoVectorStore));
        if (serializedStoreState.ValueKind is JsonValueKind.String)
        {
            this.ThreadDbKey = serializedStoreState.Deserialize<string>();
        }
    }

    public string? ThreadDbKey { get; private set; }

    public override async Task AddMessagesAsync(
        IEnumerable<ChatMessage> messages,
        CancellationToken cancellationToken)
    {
        this.ThreadDbKey ??= Guid.NewGuid().ToString("N");
        var collection = this._mongoVectorStore.GetCollection<string, ChatHistoryItem>("chat_history");
        await collection.EnsureCollectionExistsAsync(cancellationToken);
        await collection.UpsertAsync(messages.Select(x => new ChatHistoryItem()
        {
            Key = this.ThreadDbKey + x.MessageId,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            ThreadId = this.ThreadDbKey,
            SerializedMessage = JsonSerializer.Serialize(x),
            MessageText = x.Text
        }), cancellationToken);
    }

    public override async Task<IEnumerable<ChatMessage>> GetMessagesAsync(
        CancellationToken cancellationToken)
    {
        var collection = this._mongoVectorStore.GetCollection<string, ChatHistoryItem>("chat_history");
        await collection.EnsureCollectionExistsAsync(cancellationToken);
        var records = collection
            .GetAsync(
                x => x.ThreadId == this.ThreadDbKey, 10,
                new() { OrderBy = x => x.Descending(y => y.Timestamp) },
                cancellationToken);

        List<ChatMessage> messages = [];
        await foreach (var record in records)
        {
            messages.Add(JsonSerializer.Deserialize<ChatMessage>(record.SerializedMessage!)!);
        }

        messages.Reverse();
        return messages;
    }

    public override JsonElement Serialize(JsonSerializerOptions? jsonSerializerOptions = null) =>
        // We have to serialize the thread id, so that on deserialization you can retrieve the messages using the same thread id.
        JsonSerializer.SerializeToElement(this.ThreadDbKey);
}