using AutoMapper;
using FCG.Catalog.Application.Services;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Models;
using FCG.Catalog.Infra.Mapping;
using FCG.Catalog.Infra.Repository;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Net;

namespace FCG.Catalog.Tests
{
	public class GameTests
	{
		private readonly Mock<IGameRepository> _repositoryMock;
		private readonly IMapper _mapper;
		private readonly GameService _sut;

		public GameTests()
		{
			_repositoryMock = new Mock<IGameRepository>();

			var expression = new MapperConfigurationExpression();
			expression.AddProfile<GameProfile>();
			var config = new MapperConfiguration(expression, NullLoggerFactory.Instance);
			_mapper = config.CreateMapper();

			_sut = new GameService(_repositoryMock.Object, _mapper);
		}

		[Fact]
		public async Task CreateGameTest()
		{
			var id = Guid.NewGuid();

            // Arrange
            var dto = new GameRegisterDto
			{
				Name = "EA FC 26",
				Platform = "Playstation 5",
				PublisherName = "Electronic Arts",
				Description = "The next evolution of football.",
				Price = 299.90
			};

			_repositoryMock.Setup(r => r.Create(It.IsAny<Game>())).ReturnsAsync(id);

			// Act
			var response = await _sut.Create(dto);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
			Assert.Equal(id, response.ResultValue);
			Assert.Equal("Jogo registrado com sucesso.", response.Message);
		}

		[Fact]
		public async Task RemoveGameTest()
		{
			// Arrange
			var game = Game.Create("Game 1", "PC", "Publisher 1", "Description 1", 59.99);

			_repositoryMock.Setup(r => r.GetById(game.Id)).ReturnsAsync(game);
			_repositoryMock.Setup(r => r.Remove(It.IsAny<Game>())).ReturnsAsync(true);

			// Act
			var response = await _sut.Remove(game.Id);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		}

		[Fact]
		public async Task GetAllGamesTest()
		{
			// Arrange
			IEnumerable<Game> games =
			[
				Game.Create("Game 1", "PC", "Publisher 1", "Description 1", 59.99),
				Game.Create("Game 2", "Console", "Publisher 2", "Description 2", 69.99)
			];
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
			var game = Game.Create("Game 1", "PC", "Publisher 1", "Description 1", 59.99);

			_repositoryMock.Setup(r => r.GetById(game.Id)).ReturnsAsync(game);

			// Act
			var response = await _sut.GetById(game.Id);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal(game.Id, response.ResultValue!.Id);
		}

		[Fact]
		public async Task UpdateGameTest()
		{
			// Arrange
			var game = Game.Create("Game 1", "PC", "Publisher 1", "Description 1", 59.99);
			var updateDto = new GameUpdateDto
			{
				Name = "Updated Game",
				Platform = "Updated Platform",
				PublisherName = "Updated Publisher",
				Description = "Updated Description",
				Price = 79.99
			};
			_repositoryMock.Setup(r => r.GetById(game.Id)).ReturnsAsync(game);
			_repositoryMock.Setup(r => r.Update(It.IsAny<Game>())).ReturnsAsync(true);

			// Act
			var response = await _sut.Update(game.Id, updateDto);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		}
	}
}