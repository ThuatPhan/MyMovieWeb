using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Helper;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Application.Services
{
    public class BlogPostService : IBlogPostService
    {
        private readonly FileUploadHelper _uploadHelper;
        private readonly IMapper _mapper;
        private readonly IRepository<BlogTag> _tagRepo;
        private readonly IRepository<BlogPost> _blogPostRepo;
        public BlogPostService(
            IMapper mapper,
            IRepository<BlogTag> tagRepository,
            IRepository<BlogPost> blogPostRepo,
            FileUploadHelper uploadHelper)
        {
            _blogPostRepo = blogPostRepo;
            _tagRepo = tagRepository;
            _mapper = mapper;
            _uploadHelper = uploadHelper;
        }
        public async Task<Result<BlogPostDTO>> CreateBlogPost(CreateBlogRequestDTO blogRequestDTO)
        {
            BlogPost blogPostToCreate = _mapper.Map<BlogPost>(blogRequestDTO);

            HashSet<int> existingTagIds = (await _tagRepo.GetAllAsync())
                .Select(g => g.Id)
                .ToHashSet();

            List<BlogPostTag> invalidGenres = blogPostToCreate.BlogPostTags
                .Where(genre => !existingTagIds.Contains(genre.BlogTagId))
                .ToList();

            if (invalidGenres.Any())
            {
                String invalidGenreIds = string.Join(", ", invalidGenres.Select(g => g.BlogTagId));
                return Result<BlogPostDTO>.Failure($"Tag id(s) {invalidGenreIds} are not defined");
            }

            string thumbNailUrl = await _uploadHelper.UploadImageAsync(blogRequestDTO.Thumbnail);
            blogPostToCreate.Thumbnail = thumbNailUrl;

            BlogPost createdBlogPost = await _blogPostRepo.AddAsync(blogPostToCreate);
            BlogPostDTO blogPostDTO = _mapper.Map<BlogPostDTO>(createdBlogPost);

            return Result<BlogPostDTO>.Success(blogPostDTO, "Blog created successfully");
        }

        public async Task<Result<bool>> DeleteBlogPost(int id)
        {
            BlogPost? blogToDelete = await _blogPostRepo.GetByIdAsync(id);
            if (blogToDelete is null)
            {
                return Result<bool>.Failure($"Blog id {id} not found");
            }

            var deleteFileTasks = Task.WhenAll(
                _uploadHelper.DeleteImageFileAsync(blogToDelete.Thumbnail)
            );

            await deleteFileTasks;

            await _blogPostRepo.RemoveAsync(blogToDelete);

            return Result<bool>.Success(true, "Blog deleted successfully");
        }

        public async Task<Result<List<BlogPostDTO>>> FindAllBlogPost(int pageNumber, int pageSize, Expression<Func<BlogPost, bool>> predicate, Func<IQueryable<BlogPost>, IOrderedQueryable<BlogPost>>? orderBy = null)
        {
            IQueryable<BlogPost> query = _blogPostRepo.GetBaseQuery(predicate);
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            IEnumerable<BlogPost> blogs = await query
                .Include(m => m.BlogPostTags)
                    .ThenInclude(mg => mg.BlogTag)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            List<BlogPostDTO> blogDTOs = _mapper.Map<List<BlogPostDTO>>(blogs);

            return Result<List<BlogPostDTO>>.Success(blogDTOs, "Blog retrieved successfully");
        }

        public async Task<Result<BlogPostDTO>> GetBlogPostById(int id)
        {
            IQueryable<BlogPost> query = _blogPostRepo.GetBaseQuery(predicate: m => m.Id == id);

            BlogPost? blog = await query
                .Include(m => m.BlogPostTags)
                    .ThenInclude(mg => mg.BlogTag)
                .FirstOrDefaultAsync();

            if (blog is null)
            {
                return Result<BlogPostDTO>.Failure($"Blog id {id} not found");
            }

            BlogPostDTO blogDTO = _mapper.Map<BlogPostDTO>(blog);
            

            return Result<BlogPostDTO>.Success(blogDTO, "Blog retrieved successfully");
        }

        public async Task<Result<BlogPostDTO>> UpdateBlogPost(int id, UpdateBlogRequestDTO blogRequestDTO)
        {
            IQueryable<BlogPost> query = _blogPostRepo.GetBaseQuery(predicate: m => m.Id == id);
            BlogPost? blogToUpdate = await query.FirstOrDefaultAsync();

            if (blogToUpdate is null)
            {
                return Result<BlogPostDTO>.Failure($"Blog id {id} not found");
            }

            HashSet<int> existingGenreIds = (await _tagRepo.GetAllAsync())
                .Select(g => g.Id)
                .ToHashSet();

            List<int> invalidGenres = blogRequestDTO.TagIds
                .Where(genreId => !existingGenreIds.Contains(genreId))
                .ToList();

            if (invalidGenres.Any())
            {
                string invalidGenreIds = string.Join(", ", invalidGenres.Select(g => g).ToList());
                return Result<BlogPostDTO>.Failure($"Tag id(s) {invalidGenreIds} are not defined");
            }

            if (blogRequestDTO.Thumbnail != null)
            {
                await _uploadHelper.DeleteImageFileAsync(blogToUpdate.Thumbnail);
                string thumbNailUrl = await _uploadHelper.UploadImageAsync(blogRequestDTO.Thumbnail);
                blogToUpdate.Thumbnail = thumbNailUrl;
            }

            _mapper.Map(blogRequestDTO, blogToUpdate);

            await _blogPostRepo.UpdateAsync(blogToUpdate);

            BlogPost? updatedMovie = await query
                .Include(m => m.BlogPostTags)
                    .ThenInclude(mg => mg.BlogTag)
                    .FirstOrDefaultAsync();

            BlogPostDTO blogDTO = _mapper.Map<BlogPostDTO>(updatedMovie);

            return Result<BlogPostDTO>.Success(blogDTO, "Blog updated successfully");
        }
    }
}
