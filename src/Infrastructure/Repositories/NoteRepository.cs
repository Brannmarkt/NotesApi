using Application.Interfaces;
using Application.Interfaces.Notes;
using Domain.Entities;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;
public class NoteRepository : GenericRepository<NoteEntity>, INoteRepository
{
    public NoteRepository(AppDbContext context) : base(context)
    {

    }
}
