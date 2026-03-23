using AutoMapper;
using FCG.Catalog.Application.Services;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Repository;
using FCG.Catalog.Infra.Mapping;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Net;
using FCG.Catalog.Domain.Models.Catalog;

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
              Price = 299.90M
			};

            _repositoryMock.Setup(r => r.Create(It.IsAny<Game>())).Returns(id);

			// Act
			var response = await _sut.Create(dto);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
			Assert.Equal(id, response.ResultValue);
         Assert.Equal("Game created successfully.", response.Message);
		}

		[Fact]
		public async Task Create_ShouldReturnBadRequest_WhenDtoIsInvalid()
		{
			var dto = new GameRegisterDto
			{
				Name = "A",
				Platform = "P",
				PublisherName = "E",
				Description = "D",
				Price = 0
			};

			var response = await _sut.Create(dto);

			Assert.False(response.IsSuccess);
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.StartsWith("Invalid game data:", response.Message);
			_repositoryMock.Verify(r => r.Create(It.IsAny<Game>()), Times.Never);
		}

		[Fact]
		public async Task Create_ShouldReturnBadRequest_WhenGameAlreadyExists()
		{
			var dto = new GameRegisterDto
			{
				Name = "EA FC 26",
				Platform = "Playstation 5",
				PublisherName = "Electronic Arts",
				Description = "The next evolution of football.",
				Price = 299.90M
			};
			var existing = Game.Create(dto.Name, dto.Platform, dto.PublisherName, dto.Description, dto.Price);

			_repositoryMock.Setup(r => r.GetByName(dto.Name)).ReturnsAsync(existing);

			var response = await _sut.Create(dto);

			Assert.False(response.IsSuccess);
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Equal("Game already registered.", response.Message);
			_repositoryMock.Verify(r => r.Create(It.IsAny<Game>()), Times.Never);
		}

		[Fact]
		public async Task RemoveGameTest()
		{
			// Arrange
          var game = Game.Create("Game 1", "PC", "Publisher 1", "Description 1", 59.99M);

			_repositoryMock.Setup(r => r.GetById(game.Id)).ReturnsAsync(game);
         _repositoryMock.Setup(r => r.Remove(It.IsAny<Game>()));

			// Act
			var response = await _sut.Remove(game.Id);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		}

		[Fact]
		public async Task Remove_ShouldReturnNotFound_WhenGameDoesNotExist()
		{
			var id = Guid.NewGuid();
			_repositoryMock.Setup(r => r.GetById(id)).ReturnsAsync((Game?)null);

			var response = await _sut.Remove(id);

			Assert.False(response.IsSuccess);
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
			Assert.Equal("Game not found for removal.", response.Message);
			_repositoryMock.Verify(r => r.Remove(It.IsAny<Game>()), Times.Never);
		}

		[Fact]
		public async Task GetAllGamesTest()
		{
			// Arrange
			IEnumerable<Game> games =
			[
             Game.Create("Game 1", "PC", "Publisher 1", "Description 1", 59.99M),
				Game.Create("Game 2", "Console", "Publisher 2", "Description 2", 69.99M)
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
          var game = Game.Create("Game 1", "PC", "Publisher 1", "Description 1", 59.99M);

			_repositoryMock.Setup(r => r.GetById(game.Id)).ReturnsAsync(game);

			// Act
			var response = await _sut.GetById(game.Id);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal(game.Id, response.ResultValue!.Id);
		}

		[Fact]
		public async Task GetById_ShouldReturnNotFound_WhenGameDoesNotExist()
		{
			var id = Guid.NewGuid();
			_repositoryMock.Setup(r => r.GetById(id)).ReturnsAsync((Game?)null);

			var response = await _sut.GetById(id);

			Assert.False(response.IsSuccess);
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
			Assert.Equal("Game not found.", response.Message);
		}

		[Fact]
		public async Task UpdateGameTest()
		{
			// Arrange
          var game = Game.Create("Game 1", "PC", "Publisher 1", "Description 1", 59.99M);
			var updateDto = new GameUpdateDto
			{
				Description = "Updated Description",
               Price = 79.99M,
				IsAvailable = false
			};
			_repositoryMock.Setup(r => r.GetById(game.Id)).ReturnsAsync(game);
         _repositoryMock.Setup(r => r.Update(It.IsAny<Game>()));

			// Act
			var response = await _sut.Update(game.Id, updateDto);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		}

		[Fact]
		public async Task Update_ShouldReturnNotFound_WhenGameDoesNotExist()
		{
			var id = Guid.NewGuid();
			var updateDto = new GameUpdateDto
			{
				Description = "Updated Description",
				Price = 79.99M,
				IsAvailable = false
			};
			_repositoryMock.Setup(r => r.GetById(id)).ReturnsAsync((Game?)null);

			var response = await _sut.Update(id, updateDto);

			Assert.False(response.IsSuccess);
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
			Assert.Equal("Game not found for update.", response.Message);
			_repositoryMock.Verify(r => r.Update(It.IsAny<Game>()), Times.Never);
		}
	}
}