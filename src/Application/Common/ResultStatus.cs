using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common;
public enum ResultStatus
{
    Success,
    NotFound,
    Error,
    InvalidData
}

public record ServiceResult<T>(ResultStatus Status, T? Data, string? Message = null);
