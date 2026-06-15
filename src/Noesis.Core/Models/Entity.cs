namespace Noesis.Core.Models;

public enum EntityType
{
    Tool,
    Library,
    File,
    Service,
    AiAgent
}

public record Entity(
    Guid Id,
    Guid ProjectId,
    string Content,
    DateTimeOffset CreatedAt,
    ConfidenceLevel Confidence,
    MemoryType Type,
    string? Source,
    string Description,
    string? Location, //file path
    EntityType CurrentEntityType
    ) : MemoryEntry(Id, ProjectId, Content, CreatedAt, Confidence, Type, Source);