using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }


    public async Task AddAsync(T entity) 
        => await _dbSet.AddAsync(entity);

    public void Delete(T entity) 
        => _dbSet.Remove(entity);

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) 
        => await _dbSet.Where(predicate).ToListAsync();

    public async Task<IEnumerable<T>> GetAllAsync(int pageNumber, int pageSize) 
        => await _dbSet.Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    public IQueryable<T> GetAllQueryable() => _dbSet.AsNoTracking(); 

    public async Task<T?> GetByIdAsync(int id) 
        => await _dbSet.FindAsync(id);

    public void Update(T entity) 
        => _dbSet.Update(entity);
}
