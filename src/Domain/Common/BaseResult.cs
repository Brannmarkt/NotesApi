using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common;
public class BaseResult<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string Error { get; }

    protected BaseResult(bool isSuccess, T? value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static BaseResult<T> Success(T value) => new(true, value, string.Empty);
    public static BaseResult<T> Failure(string error) => new(false, default, error);
}
