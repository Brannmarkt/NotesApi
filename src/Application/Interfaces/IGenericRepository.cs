using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync(int pageNumber, int pageSize);
    IQueryable<T> GetAllQueryable();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);

    // Додатково для Middle: пошук за фільтром
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
}
