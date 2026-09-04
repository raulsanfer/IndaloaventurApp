namespace IndaloaventurApp.SharedUI.Models.Common;

/// <summary>
/// Represents the outcome of a frontend service operation without throwing for expected failures.
/// </summary>
/// <typeparam name="T">Type returned when the operation succeeds.</typeparam>
public sealed class ServiceResult<T>
{
    private ServiceResult(bool isSuccess, T? value, ServiceError? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    /// <summary>
    /// Gets whether the operation completed successfully.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the value returned by a successful operation.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Gets the stable error returned by a failed operation.
    /// </summary>
    public ServiceError? Error { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static ServiceResult<T> Success(T value) => new(true, value, null);

    /// <summary>
    /// Creates a failed result with a stable service error.
    /// </summary>
    public static ServiceResult<T> Failure(ServiceError error) => new(false, default, error);
}
