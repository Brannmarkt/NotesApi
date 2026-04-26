using Application.Common;
using Application.DTOs;
using Application.Helpers;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;
public class NoteService : INoteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateNoteDto> _validator;

    public NoteService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateNoteDto> validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<ServiceResult<IEnumerable<NoteDto>>> GetAllAsync(NotesQueryOptions options)
    {
        var query = _unitOfWork.Notes.GetAllQueryable();

        if (!string.IsNullOrEmpty(options.SortBy))
        {
            var sortBy = options.SortBy.Trim().ToLower();

            query = sortBy switch
            {
                "title" => options.SortDescending
                    ? query.OrderByDescending(n => n.Title)
                    : query.OrderBy(n => n.Title),

                "date" or "creationdate" => options.SortDescending
                    ? query.OrderByDescending(n => n.CreationDate)
                    : query.OrderBy(n => n.CreationDate),

                _ => query.OrderBy(n => n.Id)
            };
        }
        else
        {
            query = query.OrderByDescending(n => n.CreationDate);
        }

        var pagedData = await query
            .Skip((options.PageNumber - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync();

        var dtos = _mapper.Map<IEnumerable<NoteDto>>(pagedData);
        return new ServiceResult<IEnumerable<NoteDto>>(ResultStatus.Success, dtos);
    }

    public async Task<ServiceResult<NoteDto>> GetByIdAsync(int id)
    {
        var note = await _unitOfWork.Notes.GetByIdAsync(id);
        if (note == null) return new ServiceResult<NoteDto>(ResultStatus.NotFound, null);

        return new ServiceResult<NoteDto>(ResultStatus.Success, _mapper.Map<NoteDto>(note));
    }

    public async Task<ServiceResult<NoteDto>> CreateAsync(CreateNoteDto dto)
    {
        // 1. Явно викликаємо валідацію
        var validationResult = await _validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            // 2. Викидаємо помилку, яку зловить Middleware
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }

        var entity = _mapper.Map<NoteEntity>(dto);
        await _unitOfWork.Notes.AddAsync(entity);
        await _unitOfWork.CompleteAsync();

        return new ServiceResult<NoteDto>(ResultStatus.Success, _mapper.Map<NoteDto>(entity));

        /*if (string.IsNullOrWhiteSpace(dto.Title))
            return new ServiceResult<NoteDto>(ResultStatus.InvalidData, null, "Title is required");

        var entity = _mapper.Map<NoteEntity>(dto);
        await _unitOfWork.Notes.AddAsync(entity);
        await _unitOfWork.CompleteAsync();

        return new ServiceResult<NoteDto>(ResultStatus.Success, _mapper.Map<NoteDto>(entity));*/
    }

    public async Task<ServiceResult<bool>> UpdateAsync(int id, CreateNoteDto dto)
    {
        var note = await _unitOfWork.Notes.GetByIdAsync(id);
        if (note == null) return new ServiceResult<bool>(ResultStatus.NotFound, false);

        _mapper.Map(dto, note);
        _unitOfWork.Notes.Update(note);
        await _unitOfWork.CompleteAsync();

        return new ServiceResult<bool>(ResultStatus.Success, true);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var note = await _unitOfWork.Notes.GetByIdAsync(id);
        if (note == null) return new ServiceResult<bool>(ResultStatus.NotFound, false);

        _unitOfWork.Notes.Delete(note);
        await _unitOfWork.CompleteAsync();

        return new ServiceResult<bool>(ResultStatus.Success, true);
    }
}
