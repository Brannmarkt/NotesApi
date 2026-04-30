using Application.Common.Pagination;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Mappings;
public static class MappingExtensions
{
    public static Task<PaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> queryable, int pageNumber, int pageSize) where T : class
    {
        return PaginatedList<T>.CreateAsync(queryable.AsNoTracking(), pageNumber, pageSize);
    }
}
