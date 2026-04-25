using Application.Common;
using Application.DTOs;
using Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;
public interface INoteService
{
    Task<ServiceResult<IEnumerable<NoteDto>>> GetAllAsync(NotesQueryOptions options);
    Task<ServiceResult<NoteDto>> GetByIdAsync(int id);
    Task<ServiceResult<NoteDto>> CreateAsync(CreateNoteDto dto);
    Task<ServiceResult<bool>> UpdateAsync(int id, CreateNoteDto dto);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}
