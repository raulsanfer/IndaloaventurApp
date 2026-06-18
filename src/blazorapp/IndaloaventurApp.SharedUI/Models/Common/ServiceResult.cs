namespace IndaloaventurApp.SharedUI.Models.Common;

public sealed class ServiceResult<T>
{
    private ServiceResult(bool isSuccess, T? value, ServiceError? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public bool IsSuccess { get; }

    public T? Value { get; }

    public ServiceError? Error { get; }

    public static ServiceResult<T> Success(T value) => new(true, value, null);

    public static ServiceResult<T> Failure(ServiceError error) => new(false, default, error);
}
