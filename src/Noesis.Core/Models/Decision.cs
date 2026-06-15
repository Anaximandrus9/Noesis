using System.Collections.Generic;
namespace Noesis.Core.Models;

public enum Status
{
    Deprecated,
    Superseded,
    Active
}

public record Decision(
    Guid Id,
    Guid ProjectId,
    string Content,
    DateTimeOffset CreatedAt,
    ConfidenceLevel Confidence,
    MemoryType Type,
    string? Source,
    string Reason,
    List<string> Alternatives,
    Status Status
    ) : MemoryEntry(Id, ProjectId, Content, CreatedAt, Confidence, Type, Source);