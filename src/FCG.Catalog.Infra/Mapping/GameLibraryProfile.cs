using AutoMapper;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Models.Library;

namespace FCG.Catalog.Infra.Mapping;

public class GameLibraryProfile : Profile
{
    public GameLibraryProfile()
    {
        CreateMap<GameLibraryItem, GameLibraryGameResponseDto>()
            .ConstructUsing(game => new GameLibraryGameResponseDto(
                game.GameId,
                game.Name,
                game.Platform,
                game.PublisherName,
                game.Description,
                game.UnitPrice));
    }
}
