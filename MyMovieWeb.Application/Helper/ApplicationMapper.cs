using AutoMapper;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Domain.Entities;

namespace MyMovieWeb.Application.Helper
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            //Genre
            CreateMap<GenreRequestDTO, Genre>();
            CreateMap<Genre, GenreDTO>();

            //Movie
            CreateMap<CreateMovieRequestDTO, Movie>()
                .ForMember(dest => dest.Actors, opt => opt.MapFrom(src => string.Join(",", src.Actors)))
                .ForMember(dest => dest.MovieGenres, opt => opt.MapFrom(src => src.GenreIds.Select(id => new Domain.Entities.MovieGenre
                {
                    GenreId = id
                })));

            CreateMap<UpdateMovieRequestDTO, Movie>()
                .ForMember(dest => dest.Actors, opt => opt.MapFrom(src => string.Join(",", src.Actors)))
                .ForMember(dest => dest.MovieGenres, opt => opt.MapFrom(src => src.GenreIds.Select(id => new Domain.Entities.MovieGenre
                {
                    GenreId = id
                })));

            CreateMap<Movie, MovieDTO>()
                .ForMember(dest => dest.Actors, opt => opt.MapFrom(src => SplitActors(src.Actors)))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.MovieGenres.Select(mg => new DTOs.Responses.MovieGenre
                {
                    GenreId = mg.GenreId,
                    GenreName = mg.Genre.Name
                }).ToList()))
                .ForMember(dest => dest.Episodes, opt => opt.MapFrom(src => src.Episodes.Select(e => new EpisodeDTO
                {
                    Id = e.Id,
                    Title = e.Title,
                    EpisodeNumber = e.EpisodeNumber,
                    Description = e.Description,
                    IsShow = e.IsShow,
                    CreatedDate = e.CreatedDate,
                    videoUrl = e.VideoUrl
                }).ToList()));

            //Episode
            CreateMap<CreateEpisodeRequestDTO, Episode>();
            CreateMap<UpdateEpisodeRequestDTO, Episode>();
            CreateMap<Episode, EpisodeDTO>();
        }

        private static List<string> SplitActors(string actors)
        {
            return actors.Split(",").ToList() ?? new List<string>();
        }
    }
}
