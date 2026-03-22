using AutoMapper;
using FCG.Catalog.Application.Services;
using FCG.Catalog.Domain.Events;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Models.Library;
using FCG.Catalog.Infra.Repository;
using Moq;
using System.Net;

namespace FCG.Catalog.Tests;

public class GameLibraryTests
{
    private readonly Mock<IGameLibraryRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GameLibraryService _sut;

    public GameLibraryTests()
    {
        _repositoryMock = new Mock<IGameLibraryRepository>();
        _mapperMock = new Mock<IMapper>();

        _sut = new GameLibraryService(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task AddGames_ShouldReturnNoContentAndSkipPersistence_WhenGamesCollectionIsEmpty()
    {
        var response = await _sut.AddGames(10, []);

        Assert.True(response.IsSuccess);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        _repositoryMock.Verify(r => r.GetByUserId(It.IsAny<int>()), Times.Never);
        _repositoryMock.Verify(r => r.Create(It.IsAny<GameLibrary>()), Times.Never);
        _repositoryMock.Verify(r => r.Update(It.IsAny<GameLibrary>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddGames_ShouldCreateLibraryAndPersist_WhenLibraryDoesNotExist()
    {
        const int userId = 10;
        var games = BuildOrderGames();

        _repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync((GameLibrary?)null);

        var response = await _sut.AddGames(userId, games);

        Assert.True(response.IsSuccess);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        _repositoryMock.Verify(r => r.Create(It.Is<GameLibrary>(l => l.UserId == userId && l.Games.Count == games.Count)), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<GameLibrary>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddGames_ShouldUpdateExistingLibraryAndPersist_WhenLibraryExists()
    {
        const int userId = 10;
        var existingLibrary = GameLibrary.Create(userId);
        var games = BuildOrderGames();

        _repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync(existingLibrary);

        var response = await _sut.AddGames(userId, games);

        Assert.True(response.IsSuccess);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(games.Count, existingLibrary.Games.Count);
        _repositoryMock.Verify(r => r.Create(It.IsAny<GameLibrary>()), Times.Never);
        _repositoryMock.Verify(r => r.Update(existingLibrary), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddGames_ShouldKeepLibraryUnchanged_WhenAllGamesAlreadyExist()
    {
        const int userId = 10;
        var duplicateGameId = Guid.NewGuid();
        var existingLibrary = GameLibrary.Create(userId);
        existingLibrary.AddGames([
            new OrderItemSnapshot(duplicateGameId, "GTA", "PC", "Rockstar", "Desc", 100)
        ]);

        IReadOnlyCollection<OrderItemSnapshot> duplicatedGames =
        [
            new(duplicateGameId, "Another Name", "PC", "Rockstar", "Another Desc", 999)
        ];

        _repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync(existingLibrary);

        var response = await _sut.AddGames(userId, duplicatedGames);

        Assert.True(response.IsSuccess);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Single(existingLibrary.Games);
        Assert.Contains(existingLibrary.Games, g => g.GameId == duplicateGameId && g.Name == "GTA");
        _repositoryMock.Verify(r => r.Update(existingLibrary), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddGames_ShouldIgnoreDuplicatedGames_WhenGameAlreadyExistsInLibrary()
    {
        const int userId = 10;
        var existingLibrary = GameLibrary.Create(userId);
        var duplicateGameId = Guid.NewGuid();
        existingLibrary.AddGames([
            new OrderItemSnapshot(duplicateGameId, "Old Name", "PC", "Publisher", "Desc", 99)
        ]);

        var games =
            new List<OrderItemSnapshot>
            {
                new(duplicateGameId, "New Name", "PC", "Publisher", "Desc", 150),
                new(Guid.NewGuid(), "Fresh Game", "PC", "Publisher", "Desc", 80)
            };

        _repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync(existingLibrary);

        var response = await _sut.AddGames(userId, games);

        Assert.True(response.IsSuccess);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(2, existingLibrary.Games.Count);
        Assert.Contains(existingLibrary.Games, g => g.GameId == duplicateGameId && g.Name == "Old Name");
        _repositoryMock.Verify(r => r.Update(existingLibrary), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetGamesByUserId_ShouldReturnOkWithEmptyCollection_WhenLibraryDoesNotExist()
    {
        const int userId = 77;
        _repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync((GameLibrary?)null);

        var response = await _sut.GetGamesByUserId(userId);

        Assert.True(response.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.ResultValue);
        Assert.Empty(response.ResultValue!);
        _mapperMock.Verify(m => m.Map<IReadOnlyCollection<GameLibraryGameResponseDto>>(It.IsAny<IReadOnlyCollection<GameLibraryItem>>()), Times.Never);
    }

    [Fact]
    public async Task GetGamesByUserId_ShouldReturnMappedGames_WhenLibraryExists()
    {
        const int userId = 77;
        var library = GameLibrary.Create(userId);
        var gameId = Guid.NewGuid();
        library.AddGames([
            new OrderItemSnapshot(gameId, "GTA", "PC", "Rockstar", "Desc", 100)
        ]);

        IReadOnlyCollection<GameLibraryGameResponseDto> mappedGames =
        [
            new GameLibraryGameResponseDto(gameId, "GTA", "PC", "Rockstar", "Desc", 100)
        ];

        _repositoryMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync(library);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<GameLibraryGameResponseDto>>(library.Games)).Returns(mappedGames);

        var response = await _sut.GetGamesByUserId(userId);

        Assert.True(response.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.ResultValue);
        Assert.Single(response.ResultValue!);
        Assert.Equal(gameId, response.ResultValue.First().GameId);
        _mapperMock.Verify(m => m.Map<IReadOnlyCollection<GameLibraryGameResponseDto>>(library.Games), Times.Once);
    }

    private static IReadOnlyCollection<OrderItemSnapshot> BuildOrderGames()
        =>
        [
            new(Guid.NewGuid(), "GTA", "PC", "Rockstar", "Desc", 100),
            new(Guid.NewGuid(), "RDR2", "PC", "Rockstar", "Desc", 200)
        ];
}
