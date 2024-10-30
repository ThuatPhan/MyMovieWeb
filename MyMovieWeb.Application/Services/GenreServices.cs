using AutoMapper;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;

namespace MyMovieWeb.Application.Services
{
    public class GenreServices : IGenreServices
    {
        private readonly IGenreRepository _genreRepo;
        private readonly IRepository<Domain.Entities.MovieGenre> _movieGenreRepo;

        private readonly IMapper _mapper;

        public GenreServices(IMapper mapper, IGenreRepository genreRepo, IRepository<Domain.Entities.MovieGenre> movieGenreRepo)
        {
            _genreRepo = genreRepo;
            _movieGenreRepo = movieGenreRepo;
            _mapper = mapper;
        }

        public async Task<Result<GenreDTO>> CreateGenre(GenreRequestDTO genreRequestDTO)
        {
            Genre genreToCreate = _mapper.Map<Genre>(genreRequestDTO);
            Genre createdGenre = await _genreRepo.AddAsync(genreToCreate);
            GenreDTO genreDTO = _mapper.Map<GenreDTO>(createdGenre);

            return Result<GenreDTO>.Success(genreDTO, "Genre created successfully");
        }

        public async Task<Result<GenreDTO>> UpdateGenre(int id, GenreRequestDTO genreRequestDTO)
        {
            Genre? genreToUpdate = await _genreRepo.GetByIdAsync(id);

            if (genreToUpdate is null)
            {
                return Result<GenreDTO>.Failure($"Genre id {id} not found");
            }

            _mapper.Map(genreRequestDTO, genreToUpdate);

            Genre updatedGenre = await _genreRepo.UpdateAsync(genreToUpdate);
            GenreDTO genreDTO = _mapper.Map<GenreDTO>(updatedGenre);

            return Result<GenreDTO>.Success(genreDTO, "Genre updated successfully");

        }

        public async Task<Result<bool>> DeleteGenre(int id)
        {
            Genre? genreToDelete = await _genreRepo.GetByIdAsync(id);

            if (genreToDelete is null)
            {
                return Result<bool>.Failure($"Genre id {id} not found");
            }

            await _movieGenreRepo.RemoveRangeAsync(mg => mg.GenreId == id);
            await _genreRepo.RemoveAsync(genreToDelete);

            return Result<bool>.Success(true, "Genre deleted successfully");
        }

        public async Task<Result<int>> CountGenre()
        {
            int totalGenreCount = await _genreRepo.CountAsync();
            return Result<int>.Success(totalGenreCount, "Total genre count retrieved successfully");
        }

        public async Task<Result<GenreDTO>> GetGenreById(int id)
        {
            Genre? genre = await _genreRepo.GetByIdAsync(id);

            if (genre is null)
            {
                return Result<GenreDTO>.Failure($"Genre id {id} not found");
            }

            GenreDTO genreDTO = _mapper.Map<GenreDTO>(genre);

            return Result<GenreDTO>.Success(genreDTO, "Genre retrieved successfully");
        }

        public async Task<Result<List<GenreDTO>>> GetAllGenres()
        {
            IEnumerable<Genre> genres = await _genreRepo.GetAllAsync();
            List<GenreDTO> genreDTOs = _mapper.Map<List<GenreDTO>>(genres);

            return Result<List<GenreDTO>>.Success(genreDTOs, "Genres retrieved successfully");
        }

        public async Task<Result<List<GenreDTO>>> GetPagedGenres(int pageNumber, int pageSize)
        {
            IEnumerable<Genre> genres = await _genreRepo.GetPagedGenresAsync(pageNumber, pageSize);
            List<GenreDTO> genreDTOs = _mapper.Map<List<GenreDTO>>(genres);

            return Result<List<GenreDTO>>.Success(genreDTOs, "Genres retrieved successfully");
        }
    }
}
