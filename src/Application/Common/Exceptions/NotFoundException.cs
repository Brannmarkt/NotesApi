using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Exceptions;
public class NotFoundException : Exception
{
    public NotFoundException()
        : base()
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Зручний конструктор для Clean Architecture
    public NotFoundException(string name, object key)
        : base($"Сутність \"{name}\" ({key}) не знайдена.")
    {
    }
}
