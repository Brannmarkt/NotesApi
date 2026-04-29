using Application.Interfaces.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;
public interface IUnitOfWork : IDisposable
{
    INoteRepository Notes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
