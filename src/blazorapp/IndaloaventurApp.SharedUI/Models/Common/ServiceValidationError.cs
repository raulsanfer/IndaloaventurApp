namespace IndaloaventurApp.SharedUI.Models.Common;

/// <summary>
/// Describes a validation error associated with an input property.
/// </summary>
/// <param name="PropertyName">The API property that failed validation.</param>
/// <param name="ErrorMessage">The safe, user-facing validation message.</param>
public sealed record ServiceValidationError(string PropertyName, string ErrorMessage);
