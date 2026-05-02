using Application.Interfaces;
using Application.Interfaces.Notes;
using Application.Notes.DTOs;
using Application.Notes.Queries.GetNotes;
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
public class GetNotesListQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<INoteRepository> _mockNotesRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetNotesListQueryHandler _handler;

    public GetNotesListQueryHandlerTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockNotesRepo = new Mock<INoteRepository>();
        _mockMapper = new Mock<IMapper>();

        // Прив'язуємо репозиторій до UnitOfWork
        _mockUow.Setup(u => u.Notes).Returns(_mockNotesRepo.Object);

        _handler = new GetNotesListQueryHandler(_mockUow.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnPaginatedList_WhenCalled()
    {
        // Arrange
        var query = new GetNotesListQuery
        {
            PageNumber = 1,
            PageSize = 10,
            SearchTerm = null // Тестуємо базовий сценарій без пошуку
        };

        // 1. Створюємо фейкові сутності (те, що нібито лежить у базі)
        var entities = new List<NoteEntity>
        {
            new NoteEntity { Id = 1, Title = "Перша", Text = "Текст 1", CreationDate = DateTime.UtcNow },
            new NoteEntity { Id = 2, Title = "Друга", Text = "Текст 2", CreationDate = DateTime.UtcNow.AddMinutes(-5) }
        };

        // 2. Створюємо DTO (те, що має вийти після мапера)
        var dtos = new List<NoteDto>
        {
            new NoteDto { Id = 1, Title = "Перша", Text = "Текст 1" },
            new NoteDto { Id = 2, Title = "Друга", Text = "Текст 2" }
        };

        // 3. Вчимо GetAll повертати IQueryable (щоб Where та OrderBy не впали з помилкою)
        _mockNotesRepo.Setup(r => r.GetAll())
                      .Returns(entities.AsQueryable());

        // 4. Вчимо GetPagedAsync повертати Tuple (наші сутності та їх загальну кількість: 2)
        _mockNotesRepo.Setup(r => r.GetPagedAsync(
                It.IsAny<IQueryable<NoteEntity>>(),
                query.PageNumber,
                query.PageSize,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((entities, entities.Count));

        // 5. Вчимо Mapper перетворювати колекцію сутностей на колекцію DTO
        _mockMapper.Setup(m => m.Map<IReadOnlyCollection<NoteDto>>(entities))
                   .Returns(dtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2); // Має бути 2 нотатки
        result.TotalCount.Should().Be(2);   // Загальна кількість теж 2
        result.PageNumber.Should().Be(1);

        // Перевіряємо, чи хендлер звернувся до всіх потрібних методів
        _mockNotesRepo.Verify(r => r.GetAll(), Times.Once);
        _mockNotesRepo.Verify(r => r.GetPagedAsync(
            It.IsAny<IQueryable<NoteEntity>>(),
            query.PageNumber,
            query.PageSize,
            It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(m => m.Map<IReadOnlyCollection<NoteDto>>(entities), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_FilterNotes_WhenSearchTermIsProvided()
    {
        // Arrange
        var query = new GetNotesListQuery
        {
            PageNumber = 1,
            PageSize = 10,
            SearchTerm = "Секрет" // Шукаємо слово "Секрет"
        };

        // 1. Створюємо 3 сутності, але тільки 2 з них містять слово "Секрет" (в Title або Text)
        var entities = new List<NoteEntity>
        {
            new NoteEntity { Id = 1, Title = "Звичайна нотатка", Text = "Просто текст", CreationDate = DateTime.UtcNow },
            new NoteEntity { Id = 2, Title = "Секретна нотатка", Text = "Важливе", CreationDate = DateTime.UtcNow.AddMinutes(-5) },
            new NoteEntity { Id = 3, Title = "Третя", Text = "Тут є Секрет всередині", CreationDate = DateTime.UtcNow.AddMinutes(-10) }
        };

        // 2. DTO, які ми очікуємо отримати після мапінгу (тільки ті 2, що збіглися)
        var expectedDtos = new List<NoteDto>
        {
            new NoteDto { Id = 2, Title = "Секретна нотатка", Text = "Важливе" },
            new NoteDto { Id = 3, Title = "Третя", Text = "Тут є Секрет всередині" }
        };

        _mockNotesRepo.Setup(r => r.GetAll())
                      .Returns(entities.AsQueryable());

        // 3. РОЗУМНИЙ МОК: Замість того, щоб повертати статичні дані, 
        // ми беремо запит 'q', який передав хендлер, і викликаємо на ньому .ToList().
        // Це змусить LINQ застосувати фільтр .Where(...) до нашого списку entities!
        _mockNotesRepo.Setup(r => r.GetPagedAsync(
                It.IsAny<IQueryable<NoteEntity>>(),
                query.PageNumber,
                query.PageSize,
                It.IsAny<CancellationToken>()))
            .Returns((IQueryable<NoteEntity> q, int page, int size, CancellationToken ct) =>
            {
                var filteredList = q.ToList(); // Виконуємо запит у пам'яті
                return Task.FromResult<(IEnumerable<NoteEntity>, int)>((filteredList, filteredList.Count));
            });

        // 4. Вчимо мапер працювати з будь-яким списком і віддавати наші очікувані DTO
        _mockMapper.Setup(m => m.Map<IReadOnlyCollection<NoteDto>>(It.IsAny<IEnumerable<NoteEntity>>()))
                   .Returns(expectedDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2); // Фільтр мав відкинути нотатку з Id=1
        result.TotalCount.Should().Be(2);

        // Переконуємось, що це саме ті нотатки, які ми шукали
        result.Items.Should().Contain(i => i.Id == 2);
        result.Items.Should().Contain(i => i.Id == 3);
        result.Items.Should().NotContain(i => i.Id == 1); // Звичайної нотатки тут бути не повинно
    }
}
