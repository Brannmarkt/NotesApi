using Application.Common.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Notes;
using Application.Notes.Commands.DeleteNote;
using Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Notes;
public class DeleteNoteCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<INoteRepository> _mockNotesRepo;
    private readonly DeleteNoteCommandHandler _handler;

    public DeleteNoteCommandHandlerTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockNotesRepo = new Mock<INoteRepository>();

        _mockUow.Setup(u => u.Notes).Returns(_mockNotesRepo.Object);

        _handler = new DeleteNoteCommandHandler(_mockUow.Object);
    }

    [Fact]
    public async Task Handle_Should_DeleteNote_WhenNoteExists()
    {
        // Arrange
        var command = new DeleteNoteCommand(1);
        var existingNote = new NoteEntity { Id = 1, Title = "На видалення", Text = "Текст" };

        _mockNotesRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(existingNote);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockNotesRepo.Verify(r => r.Delete(existingNote), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFoundException_WhenNoteDoesNotExist()
    {
        // Arrange
        var command = new DeleteNoteCommand(999);

        _mockNotesRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((NoteEntity?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();

        // Якщо вилетіла помилка, видалення не повинно було статися!
        _mockNotesRepo.Verify(r => r.Delete(It.IsAny<NoteEntity>()), Times.Never);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
