using Application.Common.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Notes;
using Application.Notes.Commands.UpdateNote;
using Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Notes;
public class UpdateNoteCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<INoteRepository> _mockNotesRepo;
    private readonly UpdateNoteCommandHandler _handler;

    public UpdateNoteCommandHandlerTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockNotesRepo = new Mock<INoteRepository>();

        _mockUow.Setup(u => u.Notes).Returns(_mockNotesRepo.Object);

        _handler = new UpdateNoteCommandHandler(_mockUow.Object);
    }

    [Fact]
    public async Task Handle_Should_UpdateNote_WhenNoteExists()
    {
        // Arrange
        var command = new UpdateNoteCommand
        {
            Id = 1,
            Title = "Оновлений заголовок",
            Text = "Оновлений текст"
        };

        var existingNote = new NoteEntity { Id = 1, Title = "Старе", Text = "Старе" };

        _mockNotesRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(existingNote);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingNote.Title.Should().Be(command.Title);
        existingNote.Text.Should().Be(command.Text);

        _mockNotesRepo.Verify(r => r.Update(existingNote), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFoundException_WhenNoteDoesNotExist()
    {
        // Arrange
        var command = new UpdateNoteCommand { Id = 999, Title = "Тест", Text = "Тест" };

        // База повертає null (нотатку не знайдено)
        _mockNotesRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((NoteEntity?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();

        // Перевіряємо, що методи Update та SaveChanges НІКОЛИ не викликалися, бо сталась помилка
        _mockNotesRepo.Verify(r => r.Update(It.IsAny<NoteEntity>()), Times.Never);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
