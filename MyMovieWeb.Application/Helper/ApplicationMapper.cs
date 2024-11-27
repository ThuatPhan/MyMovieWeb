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

            //BlogTag
            CreateMap<BlogTagRequestDTO,BlogTag>();
            CreateMap<BlogTag, BlogTagDTO>();


            //BlogPost
            CreateMap<CreateBlogRequestDTO, BlogPost>()
                .ForMember(dest => dest.BlogPostTags, opt => opt.MapFrom(src => src.TagIds.Select(id => new BlogPostTag
                {
                    BlogTagId = id
                })));
                

            CreateMap<UpdateBlogRequestDTO, BlogPost>()
                .ForMember(dest => dest.BlogPostTags, opt => opt.MapFrom(src => src.TagIds.Select(id => new BlogPostTag
                {
                    BlogTagId = id
                })));
            CreateMap<BlogPost, BlogPostDTO>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.BlogPostTags.Select(mg => new BlogPostTagDTO
                {
                    BlogTagId = mg.BlogTagId,
                    TagName = mg.BlogTag.Name
                }).ToList()));

            //Movie
            CreateMap<CreateMovieRequestDTO, Movie>()
                .ForMember(dest => dest.Actors, opt => opt.MapFrom(src => string.Join(",", src.Actors)))
                .ForMember(dest => dest.MovieGenres, opt => opt.MapFrom(src => src.GenreIds.Select(id => new MovieGenre
                {
                    GenreId = id
                })))
                .BeforeMap((src, dest) =>
                {
                    dest.IsSeriesCompleted = src.IsSeries ? false : null;
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
                    if (src.IsSeriesCompleted.HasValue)
                    {
                        dest.IsSeriesCompleted = src.IsSeriesCompleted;
                    }
                    dest.IsSeries = dest.IsSeries ? true : false;
                });

            CreateMap<Movie, MovieDTO>()
                .ForMember(dest => dest.Actors, opt => opt.MapFrom(src => SplitActors(src.Actors)))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.MovieGenres.Select(mg => new MovieGenreDTO
                {
                    GenreId = mg.GenreId,
                    GenreName = mg.Genre.Name
                }).ToList()))
                .ForMember(dest => dest.Episodes, opt => opt.MapFrom(src => src.Episodes));

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
            CreateMap<WatchHistory, WatchHistoryDTO>();

            //Followed Movie
            CreateMap<FollowedMovie, FollowedMovieDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Movie.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Movie.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Movie.Description))
                .ForMember(dest => dest.Director, opt => opt.MapFrom(src => src.Movie.Director))
                .ForMember(dest => dest.Actors, opt => opt.MapFrom(src => SplitActors(src.Movie.Actors)))
                .ForMember(dest => dest.PosterUrl, opt => opt.MapFrom(src => src.Movie.PosterUrl))
                .ForMember(dest => dest.BannerUrl, opt => opt.MapFrom(src => src.Movie.BannerUrl))
                .ForMember(dest => dest.View, opt => opt.MapFrom(src => src.Movie.View))
                .ForMember(dest => dest.VideoUrl, opt => opt.MapFrom(src => src.Movie.VideoUrl))
                .ForMember(dest => dest.RateCount, opt => opt.MapFrom(src => src.Movie.RateCount))
                .ForMember(dest => dest.RateTotal, opt => opt.MapFrom(src => src.Movie.RateTotal))
                .ForMember(dest => dest.IsSeries, opt => opt.MapFrom(src => src.Movie.IsSeries))
                .ForMember(dest => dest.IsSeriesCompleted, opt => opt.MapFrom(src => src.Movie.IsSeriesCompleted))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Movie.MovieGenres.Select(mg => new MovieGenreDTO
                {
                    GenreId = mg.GenreId,
                    GenreName = mg.Genre.Name
                }).ToList()))
                .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.Movie.ReleaseDate));

            //Comment
            CreateMap<CreateMovieCommentRequestDTO, Comment>();
            CreateMap<CreateEpisodeCommentRequestDTO, Comment>();
            CreateMap<Comment, CommentDTO>();

            //Notification
            CreateMap<Notification, NotificationDTO>();
        }

        private static List<string> SplitActors(string actors)
        {
            return actors.Split(",").ToList() ?? new List<string>();
        }
    }
}
