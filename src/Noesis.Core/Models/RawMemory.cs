namespace Noesis.Core.Models;

public record RawMemory(
    Guid Id,
    Guid ProjectId,
    string Content,
    DateTimeOffset CreatedAt,
    ConfidenceLevel Confidence,
    MemoryType Type,
    string? Source,
    string ToolName,
    int SessionDurationSeconds
) : MemoryEntry(Id, ProjectId, Content, CreatedAt, Confidence, Type, Source);