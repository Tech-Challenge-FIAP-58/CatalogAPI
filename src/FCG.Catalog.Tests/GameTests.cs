using FCG.Catalog.Application.Services;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Infra.Repository;
using Moq;
using System.Net;

namespace FCG.Catalog.Tests
{
	public class GameTests
	{
		private readonly Mock<IGameRepository> _repositoryMock;
		private readonly GameService _sut;

		public GameTests()
		{
			_repositoryMock = new Mock<IGameRepository>();
			_sut = new GameService(_repositoryMock.Object);
		}

		[Fact]
		public async Task CreateGameTest()
		{
			// Arrange
			var dto = new GameRegisterDto
			{
				Name = "EA FC 26",
				Platform = "Playstation 5",
				PublisherName = "Electronic Arts",
				Description = "The next evolution of football.",
				Price = 299.90
			};
			_repositoryMock.Setup(r => r.Create(dto)).ReturnsAsync(1);

			// Act
			var response = await _sut.Create(dto);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
			Assert.Equal(1, response.ResultValue);
			Assert.Equal("Jogo registrado com sucesso.", response.Message);
		}

		[Fact]
		public async Task RemoveGameTest()
		{
			// Arrange
			var gameId = 1;
			_repositoryMock.Setup(r => r.Remove(gameId)).ReturnsAsync(true);

			// Act
			var response = await _sut.Remove(gameId);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		}

		[Fact]
		public async Task GetAllGamesTest()
		{
			// Arrange
			var games = new List<GameResponseDto>
			{
				new(1, "Game 1", "PC", "Publisher 1", "Description 1", 59.99, DateTime.Now),
				new(2, "Game 2", "Console", "Publisher 2", "Description 2", 69.99, DateTime.Now)
			};
			_repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(games);

			// Act
			var response = await _sut.GetAll();

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal(2, response.ResultValue!.Count());
		}

		[Fact]
		public async Task GetGameByIdTest()
		{
			// Arrange
			var gameId = 1;
			var game = new GameResponseDto(gameId, "Game 1", "PC", "Publisher 1", "Description 1", 59.99, DateTime.Now);
			_repositoryMock.Setup(r => r.GetById(gameId)).ReturnsAsync(game);

			// Act
			var response = await _sut.GetById(gameId);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal(gameId, response.ResultValue!.Id);
		}

		[Fact]
		public async Task UpdateGameTest()
		{
			// Arrange
			var gameId = 1;
			var updateDto = new GameUpdateDto
			{
				Name = "Updated Game",
				Platform = "Updated Platform",
				PublisherName = "Updated Publisher",
				Description = "Updated Description",
				Price = 79.99
			};
			_repositoryMock.Setup(r => r.Update(gameId, updateDto)).ReturnsAsync(true);

			// Act
			var response = await _sut.Update(gameId, updateDto);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		}
	}
}