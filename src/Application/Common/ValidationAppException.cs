using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common;
public class ValidationAppException(IDictionary<string, string[]> errors) : Exception("One or more validation failures have occurred.")
{
    public IDictionary<string, string[]> Errors { get; } = errors;
}
