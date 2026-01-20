using FCG.Catalog.Application.Services;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Infra.Repository;
using Moq;
using System.Net;

namespace FCG.Catalog.Tests
{
	public class CatalogTests
	{
		private readonly Mock<ICatalogRepository> _repositoryMock;
		private readonly CatalogService _sut;

		public CatalogTests()
		{
			_repositoryMock = new Mock<ICatalogRepository>();
			_sut = new CatalogService(_repositoryMock.Object);
		}

		[Fact]
		public async Task GetAllCatalogsTest()
		{
			// Arrange
			var catalogs = new List<CatalogResponseDto>
			{
				new() { UserId = 1, GameId = 1, Price = 199.90M },
				new() { UserId = 2, GameId = 2, Price = 299.90M }
			};
			_repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(catalogs);

			// Act
			var response = await _sut.GetAll();

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal(2, response.ResultValue!.Count());
		}

		[Fact]
		public async Task GetCatalogByUserId()
		{
			// Arrange
			var dto = new CatalogResponseDto
			{
				UserId = 1,
				GameId = 1,
				Price = 199.90M
			};
			_repositoryMock.Setup(r => r.GetByUserId(1)).ReturnsAsync(dto);

			// Act
			var response = await _sut.GetByUserId(1);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal(dto, response.ResultValue);
		}

		[Fact]
		public async Task CreateCatalogTest()
		{
			// Arrange
			var dto = new CatalogRegisterDto
			{
				UserId = 1,
				GameId = Guid.NewGuid(),
				Price = 199.90M
			};
			_repositoryMock.Setup(r => r.Create(dto)).ReturnsAsync(true);

			// Act
			var response = await _sut.Create(dto);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal(dto, response.ResultValue);
		}
	}
}
