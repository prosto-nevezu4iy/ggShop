using AutoMapper;
using CatalogService.DTOs;
using CatalogService.Entities;

namespace CatalogService.RequestHelpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Game, GameDto>();
        CreateMap<Platform, PlatformDto>();
        CreateMap<Genre, GenreDto>();
        CreateMap<Publisher, PublisherDto>();
        CreateMap<CreateGameDto, Game>()
            .ForMember(dest => dest.PublisherId, opt => opt.MapFrom(src => src.Publisher))
            .ForMember(dest => dest.Platforms, opt => opt.MapFrom(src => src.Platforms))
            .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres));
        CreateMap<Guid, Publisher>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src));
        CreateMap<Guid, Platform>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src));
        CreateMap<Guid, Genre>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src));
    }
}
