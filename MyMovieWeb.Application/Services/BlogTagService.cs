using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Application.Services
{
    public class BlogTagService : IBlogTagService
    {
        private readonly IRepository<BlogTag> _tagRepo;
        private readonly IRepository<BlogPostTag> _postTagRepo;

        private readonly IMapper _mapper;
        public BlogTagService(
            IMapper mapper,
            IRepository<BlogTag> tagReposiotory,
            IRepository<BlogPostTag> postTagRepository)
        {
            _tagRepo = tagReposiotory;
            _postTagRepo = postTagRepository;
            _mapper = mapper;
        }
        
        public async Task<Result<int>> CountTag()
        {
            int totalTagCount = await _tagRepo.CountAsync();
            return Result<int>.Success(totalTagCount, "Total Tag count retrieved successfully");
        }

        public async Task<Result<BlogTagDTO>> CreateTag(BlogTagRequestDTO tagRequestDTO)
        {
            BlogTag tagToCreate = _mapper.Map<BlogTag>(tagRequestDTO);
            BlogTag createdTag  = await _tagRepo.AddAsync(tagToCreate);
            BlogTagDTO tagDTO = _mapper.Map<BlogTagDTO>(createdTag);

            return Result<BlogTagDTO>.Success(tagDTO, "Tag created successfully");
        }

        public async Task<Result<bool>> DeleteTag(int id)
        {
            BlogTag? tagToDelete = await _tagRepo.GetByIdAsync(id);

            if (tagToDelete is null)
            {
                return Result<bool>.Failure($"Tag id {id} not found");
            }

            await _postTagRepo.RemoveRangeAsync(mg => mg.BlogTagId == id);
            await _tagRepo.RemoveAsync(tagToDelete);

            return Result<bool>.Success(true, "Tag deleted successfully");
        }

        public async Task<Result<List<BlogTagDTO>>> GetAllTag()
        {
            IEnumerable<BlogTag> tags = await _tagRepo.GetAllAsync();
            List<BlogTagDTO> tagDTOs = _mapper.Map<List<BlogTagDTO>>(tags);

            return Result<List<BlogTagDTO>>.Success(tagDTOs, "Tag retrieved successfully");
        }

        public async Task<Result<List<BlogTagDTO>>> GetAllTags(int pageNumber, int pageSize)
        {
            IQueryable<BlogTag> query = _tagRepo.GetBaseQuery(predicate: _ => true);

            IEnumerable<BlogTag> tags = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            List<BlogTagDTO> tagDTOs = _mapper.Map<List<BlogTagDTO>>(tags);

            return Result<List<BlogTagDTO>>.Success(tagDTOs, "Tag retrieved successfully");
        }

        public async Task<Result<BlogTagDTO>> GetTagById(int id)
        {
            BlogTag? tag = await _tagRepo.GetByIdAsync(id);

            if (tag is null)
            {
                return Result<BlogTagDTO>.Failure($"Tag id {id} not found");
            }

            BlogTagDTO blogTagDTO = _mapper.Map<BlogTagDTO>(tag);

            return Result<BlogTagDTO>.Success(blogTagDTO, "Tag retrieved successfully");
        }

        public async Task<Result<BlogTagDTO>> UpdateTag(int id, BlogTagRequestDTO tagRequestDTO)
        {
            BlogTag? tagToUpdate = await _tagRepo.GetByIdAsync(id);

            if (tagToUpdate is null)
            {
                return Result<BlogTagDTO>.Failure($"Tag id {id} not found");
            }

            _mapper.Map(tagRequestDTO, tagToUpdate);

            BlogTag updatedTag = await _tagRepo.UpdateAsync(tagToUpdate);
            BlogTagDTO tagDTO = _mapper.Map<BlogTagDTO>(updatedTag);

            return Result<BlogTagDTO>.Success(tagDTO, "Tag updated successfully");
        }
    }
}
