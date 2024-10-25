using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly IRepository<Genre> _genreRepo;
        private readonly IMapper _mapper;

        public GenreServices(IRepository<Genre> genreRepo, IMapper mapper)
        {
            _genreRepo = genreRepo;
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

        public Task<Result<bool>> DeleteGenre(int id)
        {
            throw new NotImplementedException();
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

    }
}
