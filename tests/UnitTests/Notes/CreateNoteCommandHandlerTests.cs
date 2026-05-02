using Application.Interfaces;
using Application.Interfaces.Notes;
using Application.Notes.Commands.CreateNote;
using Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Notes;

public class CreateNoteCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    // 1. Додаємо окремий фейк для репозиторію
    private readonly Mock<INoteRepository> _mockNotesRepo;
    private readonly CreateNoteCommandHandler _handler;

    public CreateNoteCommandHandlerTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockNotesRepo = new Mock<INoteRepository>();

        // 2. Вчимо UnitOfWork: коли хендлер просить ".Notes", віддай йому фейковий репозиторій
        _mockUow.Setup(u => u.Notes).Returns(_mockNotesRepo.Object);

        _handler = new CreateNoteCommandHandler(_mockUow.Object);
    }

    [Fact]
    public async Task Handle_Should_AddNote_And_ReturnId()
    {
        // Arrange
        var command = new CreateNoteCommand
        {
            Title = "Test Title",
            Text = "Test Text"
        };

        // 3. Імітуємо поведінку БД: перехоплюємо сутність при додаванні і даємо їй Id = 5
        _mockNotesRepo.Setup(r => r.AddAsync(It.IsAny<NoteEntity>(), It.IsAny<CancellationToken>()))
            .Callback((NoteEntity entity, CancellationToken ct) =>
            {
                entity.Id = 5; // У реальності це робить PostgreSQL, а в тесті — ми самі
            });

        _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(5); // Перевіряємо, чи повернувся наш призначений Id

        // Перевіряємо, чи викликався метод на нашому _mockNotesRepo
        _mockNotesRepo.Verify(r => r.AddAsync(It.IsAny<NoteEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
