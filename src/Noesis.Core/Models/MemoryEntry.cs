namespace Noesis.Core.Models;

public record MemoryEntry(
    Guid Id,
    Guid ProjectId,
    string Content,
    DateTimeOffset CreatedAt,
    ConfidenceLevel Confidence,
    MemoryType Type,
    string? Source
    );