using Application.Common.Exceptions;
using Application.Interfaces;
using Application.Notes.DTOs;
using Application.Notes.Queries.GetNoteById;
using AutoMapper;
using Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Notes;
public class GetNoteByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetNoteByIdQueryHandler _handler;

    public GetNoteByIdQueryHandlerTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();

        _handler = new GetNoteByIdQueryHandler(_mockUow.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFoundException_WhenNoteDoesNotExist()
    {
        // Arrange
        var query = new GetNoteByIdQuery(999); // ID, якого немає

        // Налаштовуємо БД так, щоб вона повернула null
        _mockUow.Setup(u => u.Notes.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((NoteEntity?)null);

        // Act
        // Оскільки ми чекаємо помилку, ми зберігаємо саму дію у змінну Func
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
                 .WithMessage("*не знайдена*"); // Перевіряємо частину тексту помилки
    }

    [Fact]
    public async Task Handle_Should_ReturnNoteDto_WhenNoteExists()
    {
        // Arrange
        var noteId = 1;
        var query = new GetNoteByIdQuery(noteId);

        // Створюємо фейкові дані, які "нібито" лежать у базі
        var noteEntity = new NoteEntity { Id = noteId, Title = "Тестова", Text = "Текст" };
        var noteDto = new NoteDto { Id = noteId, Title = "Тестова", Text = "Текст" };

        // 1. Вчимо мок-БД: коли тебе попросять ID 1, поверни noteEntity
        _mockUow.Setup(u => u.Notes.GetByIdAsync(noteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(noteEntity);

        // 2. Вчимо мок-мапер: коли тобі дадуть noteEntity, перетвори її на noteDto
        _mockMapper.Setup(m => m.Map<NoteDto>(noteEntity))
                   .Returns(noteDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(noteId);
        result.Title.Should().Be("Тестова");

        // Перевіряємо, чи хендлер дійсно звертався до бази та до мапера
        _mockUow.Verify(u => u.Notes.GetByIdAsync(noteId, It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(m => m.Map<NoteDto>(noteEntity), Times.Once);
    }
}
