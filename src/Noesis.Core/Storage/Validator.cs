using Noesis.Core.Models;

namespace Noesis.Core.Storage;

public class Validator
{
    public static ValidationResult Validate(MemoryEntry entry)
    {
        List<string> errors = new List<string>();
        
        //rule 1
        if(string.IsNullOrWhiteSpace(entry.Content))
            errors.Add("No content available.");
        
        //rule 2
        if(entry.Content?.Length > 2000)
            errors.Add("Content is too long.");
        
        //rule 3
        if(entry.ProjectId == default(Guid))
            errors.Add("No project id available.");
        
        //rule 4
        if(entry.CreatedAt > DateTimeOffset.UtcNow)
            errors.Add("Ambiguous creation date.");
        
        if(!(Enum.IsDefined(typeof(ConfidenceLevel), entry.Confidence)))
            errors.Add("Invalid confidence level.");
        
        return new ValidationResult(errors.Count == 0, errors);
    }
}
/* 

content is not empty,
content is under 2000 characters,
ProjectId is set,
CreatedAt is not in the future,
Confidence is a valid enum value.

*/