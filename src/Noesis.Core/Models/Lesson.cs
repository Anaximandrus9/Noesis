using System.Collections.Generic;
namespace Noesis.Core.Models;

public record Lesson(
    Guid Id,
    Guid ProjectId,
    string Content,
    DateTimeOffset CreatedAt,
    ConfidenceLevel Confidence,
    MemoryType Type,
    string? Source,
    List<string> ObservedIn,
    List<string>? RelatedDecisions
    ) : MemoryEntry(Id, ProjectId, Content, CreatedAt, Confidence, Type, Source);