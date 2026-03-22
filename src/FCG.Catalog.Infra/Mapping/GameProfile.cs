using AutoMapper;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Models.Catalog;

namespace FCG.Catalog.Infra.Mapping;

public class GameProfile : Profile
{
    public GameProfile()
    {
        CreateMap<GameRegisterDto, Game>()
            .ConvertUsing((dto, _, context) =>
            {
                return Game.Create(
                    dto.Name,
                    dto.Platform,
                    dto.PublisherName,
                    dto.Description,
                    dto.Price
                );
            });

        //CreateMap<GameRegisterDto, Game>()
        //    .ForMember(d => d.Id, opt => opt.Ignore())
        //    .ForMember(d => d.CreatedAt, opt => opt.Ignore());

        CreateMap<GameUpdateDto, Game>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Game, GameResponseDto>()
            .ConstructUsing(g => new GameResponseDto
            {
                Id = g.Id,
                Name = g.Name,
                Platform = g.Platform,
                PublisherName = g.PublisherName,
                Description = g.Description,
                Price = g.Price,
                IsAvailable = g.IsAvailable,
                CreatedAtUtc = g.CreatedAt
            });
    }
}
