using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Exceptions;
public class ValidationException : Exception
{
    // Словник: Ключ — назва поля, Значення — масив помилок для цього поля
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("Виникли одна або кілька помилок валідації.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors)
        : this()
    {
        Errors = errors;
    }
}
