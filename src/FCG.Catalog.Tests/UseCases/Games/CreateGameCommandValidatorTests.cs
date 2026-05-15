using Xunit;
using FCG.Catalog.Application.UseCases.Games;

namespace FCG.Catalog.Tests.UseCases.Games
{
    public class CreateGameCommandValidatorTests
    {
        [Fact]
        public void Should_Fail_When_Title_Is_Empty()
        {
            var validator = new CreateGameCommandValidator();
            var cmd = new CreateGameCommand("", "desc", 10m);
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Title");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Fail_When_Price_Not_Greater_Than_Zero(decimal price)
        {
            var validator = new CreateGameCommandValidator();
            var cmd = new CreateGameCommand("Title", "desc", price);
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Price");
        }

        [Fact]
        public void Should_Pass_Valid_Command()
        {
            var validator = new CreateGameCommandValidator();
            var cmd = new CreateGameCommand("Title", "desc", 9.99m);
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }
    }
}
