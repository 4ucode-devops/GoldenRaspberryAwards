using AutoMapper;
using GoldenRaspberryAwards.Core.Dtos;
using GoldenRaspberryAwards.Core.Model;

namespace GoldenRaspberryAwards.Api.Mappers;

public class MovieMapperProfile : Profile
{
    public MovieMapperProfile()
    {
        CreateMap<MovieCsv, Movie>()
            .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Studios, opt => opt.MapFrom(src => src.Studios))
            .ForMember(dest => dest.IsWinner, opt => opt.MapFrom(src => src.IsWinner))
            .ForMember(dest => dest.MovieProducers, opt => opt.Ignore());

        CreateMap<Movie, MovieDTO>().ReverseMap();
    }
}
