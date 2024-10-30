﻿using AutoMapper;
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
                .ForMember(dest => dest.MovieGenres, opt => opt.MapFrom(src => src.GenreIds.Select(id => new MovieGenre
                {
                    GenreId = id
                })))
                .BeforeMap((src, dest) =>
                {
                    if (src.IsSeries.HasValue)
                    {
                        dest.IsSeriesCompleted = src.IsSeries.Value ? false : null;
                    }
                });

            CreateMap<UpdateMovieRequestDTO, Movie>()
                .ForMember(dest => dest.Actors, opt => opt.MapFrom(src => string.Join(",", src.Actors)))
                .ForMember(dest => dest.MovieGenres, opt => opt.MapFrom(src => src.GenreIds.Select(id => new MovieGenre
                {
                    GenreId = id
                })))
                .ForMember(dest => dest.IsSeriesCompleted, opt => opt.Condition(src => src.IsSeriesCompleted.HasValue))
                .BeforeMap((src, dest) =>
                {
                    if (dest.IsSeries == true)
                    {
                        dest.IsSeries = true;
                    }
                });

            CreateMap<Movie, MovieDTO>()
                .ForMember(dest => dest.Actors, opt => opt.MapFrom(src => SplitActors(src.Actors)))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.MovieGenres.Select(mg => new MovieGenreDTO
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
                    ReleaseDate = e.ReleaseDate,
                    VideoUrl = e.VideoUrl,
                    MovieId = e.MovieId
                }).ToList()));

            //Episode
            CreateMap<CreateEpisodeRequestDTO, Episode>();
            CreateMap<UpdateEpisodeRequestDTO, Episode>()
                .ForMember(dest => dest.EpisodeNumber, opt => opt.Condition(src => src.EpisodeNumber.HasValue));
            CreateMap<Episode, EpisodeDTO>();

            //Watch History
            CreateMap<WatchMovieRequestDTO, WatchHistory>()
                .ForMember(dest => dest.CurrentWatching, opt => opt.MapFrom(src => TimeSpan.FromSeconds(src.WatchingAt)));

            CreateMap<WatchEpisodeRequestDTO, WatchHistory>()
                .ForMember(dest => dest.CurrentWatching, opt => opt.MapFrom(src => TimeSpan.FromSeconds(src.WatchingAt)));
        }

        private static List<string> SplitActors(string actors)
        {
            return actors.Split(",").ToList() ?? new List<string>();
        }
    }
}