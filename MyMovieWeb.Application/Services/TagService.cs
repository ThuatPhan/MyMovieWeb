using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;

namespace MyMovieWeb.Application.Services
{
    public class TagService : ITagServices
    {
        private readonly IRepository<Tag> _tagRepo;
        private readonly IRepository<PostTags> _postTagRepo;

        private readonly IMapper _mapper;
        public TagService(
            IMapper mapper,
            IRepository<Tag> tagReposiotory,
            IRepository<PostTags> postTagRepository
        )
        {
            _tagRepo = tagReposiotory;
            _postTagRepo = postTagRepository;
            _mapper = mapper;
        }

        public async Task<Result<int>> CountTag()
        {
            int totalTagCount = await _tagRepo.CountAsync();
            return Result<int>.Success(totalTagCount, "Total tag count retrieved successfully");
        }

        public async Task<Result<TagDTO>> CreateTag(TagRequestDTO tagRequestDTO)
        {
            Tag tagToCreate = _mapper.Map<Tag>(tagRequestDTO);
            Tag createdTag = await _tagRepo.AddAsync(tagToCreate);
            TagDTO tagDTO = _mapper.Map<TagDTO>(createdTag);

            return Result<TagDTO>.Success(tagDTO, "Tag created successfully");
        }

        public async Task<Result<bool>> DeleteTag(int id)
        {
            Tag? tagToDelete = await _tagRepo.GetByIdAsync(id);

            if (tagToDelete is null)
            {
                return Result<bool>.Failure($"Tag id {id} not found");
            }

            var deleteTaks = new List<Task>
            {
                _postTagRepo.RemoveRangeAsync(mg => mg.TagId == id),
                _tagRepo.RemoveAsync(tagToDelete)
            };

            await Task.WhenAll(deleteTaks);

            return Result<bool>.Success(true, "Tag deleted successfully");
        }

        public async Task<Result<List<TagDTO>>> GetAllTags()
        {
            IEnumerable<Tag> tags = await _tagRepo.GetAllAsync();
            List<TagDTO> tagDTOs = _mapper.Map<List<TagDTO>>(tags);

            return Result<List<TagDTO>>.Success(tagDTOs, "Tags retrieved successfully");
        }

        public async Task<Result<List<TagDTO>>> GetAllTags(int pageNumber, int pageSize)
        {
            IQueryable<Tag> query = _tagRepo.GetBaseQuery(predicate: _ => true);

            IEnumerable<Tag> tags = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            List<TagDTO> tagDTOs = _mapper.Map<List<TagDTO>>(tags);

            return Result<List<TagDTO>>.Success(tagDTOs, "Tag retrieved successfully");
        }

        public async Task<Result<TagDTO>> GetTagById(int id)
        {
            Tag? tag = await _tagRepo.GetByIdAsync(id);

            if (tag is null)
            {
                return Result<TagDTO>.Failure($"Tag id {id} not found");
            }

            TagDTO blogTagDTO = _mapper.Map<TagDTO>(tag);

            return Result<TagDTO>.Success(blogTagDTO, "Tag retrieved successfully");
        }

        public async Task<Result<TagDTO>> UpdateTag(int id, TagRequestDTO tagRequestDTO)
        {
            Tag? tagToUpdate = await _tagRepo.GetByIdAsync(id);

            if (tagToUpdate is null)
            {
                return Result<TagDTO>.Failure($"Tag id {id} not found");
            }

            _mapper.Map(tagRequestDTO, tagToUpdate);

            Tag updatedTag = await _tagRepo.UpdateAsync(tagToUpdate);
            TagDTO tagDTO = _mapper.Map<TagDTO>(updatedTag);

            return Result<TagDTO>.Success(tagDTO, "Tag updated successfully");
        }
    }
}
