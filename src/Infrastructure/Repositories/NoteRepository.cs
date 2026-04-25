using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;
public class NoteRepository : GenericRepository<NoteEntity>, INoteRepository
{
    public NoteRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<NoteEntity>> GetArchivedNotesAsync()
    {
        return await _dbSet.Where(n => n.IsArchived).ToListAsync();
    }
}
