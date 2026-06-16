namespace Noesis.Core.Storage;

public record ValidationResult(
    bool IsSuccess,
    List<string> Errors
    );