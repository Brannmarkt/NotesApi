using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers;
public class NotesQueryOptions : QueryOptions
{
    public string? SortBy { get; set; }
}
