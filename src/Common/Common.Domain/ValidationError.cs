namespace Common.Domain;

public record ValidationError(IDictionary<string, string[]> Errors)
    : Error("General.Validation", "One or more validation errors occurred", ErrorType.Validation);