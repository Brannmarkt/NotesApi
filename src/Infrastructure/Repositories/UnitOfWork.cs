using Application.Interfaces;
using Application.Interfaces.Notes;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private INoteRepository? _notes;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public INoteRepository Notes =>
        _notes ??= new NoteRepository(_context);

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
        await _context.SaveChangesAsync(cancellationToken);
}
