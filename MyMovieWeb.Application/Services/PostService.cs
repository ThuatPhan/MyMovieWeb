using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;
using System.Linq.Expressions;

namespace MyMovieWeb.Application.Services
{
    public class PostService : IPostServices
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Tag> _tagRepo;
        private readonly IRepository<Post> _postRepo;
        private readonly IS3Services _s3Services;
        public PostService(
            IMapper mapper,
            IRepository<Tag> tagRepository,
            IRepository<Post> postRepository,
            IS3Services s3Services
        )
        {
            _postRepo = postRepository;
            _tagRepo = tagRepository;
            _mapper = mapper;
            _s3Services = s3Services;
        }

        public async Task<Result<PostDTO>> CreatePost(CreatePostRequestDTO postRequestDTO)
        {
            Post postToCreate = _mapper.Map<Post>(postRequestDTO);

            HashSet<int> existingTagIds = (await _tagRepo.GetAllAsync())
                .Select(g => g.Id)
                .ToHashSet();

            List<PostTags> invalidGenres = postToCreate.PostTags
                .Where(genre => !existingTagIds.Contains(genre.TagId))
                .ToList();

            if (invalidGenres.Any())
            {
                String invalidGenreIds = string.Join(", ", invalidGenres.Select(g => g.TagId));
                return Result<PostDTO>.Failure($"Tag id(s) {invalidGenreIds} are not defined");
            }

            string thumbNailUrl = await _s3Services.UploadFileAsync(postRequestDTO.Thumbnail);
            postToCreate.Thumbnail = thumbNailUrl;

            Post createdBlogPost = await _postRepo.AddAsync(postToCreate);
            PostDTO blogPostDTO = _mapper.Map<PostDTO>(createdBlogPost);

            return Result<PostDTO>.Success(blogPostDTO, "Post created successfully");
        }

        public async Task<Result<PostDTO>> UpdatePost(int id, UpdatePostRequestDTO postRequestDTO)
        {
            IQueryable<Post> query = _postRepo
                .GetBaseQuery(predicate: m => m.Id == id)
                .Include(p => p.PostTags);

            Post? postToUpdate = await query.FirstOrDefaultAsync();

            if (postToUpdate is null)
            {
                return Result<PostDTO>.Failure($"Post id {id} not found");
            }

            HashSet<int> existingGenreIds = (await _tagRepo.GetAllAsync())
                .Select(g => g.Id)
                .ToHashSet();

            List<int> invalidGenres = postRequestDTO.TagIds
                .Where(genreId => !existingGenreIds.Contains(genreId))
                .ToList();

            if (invalidGenres.Any())
            {
                string invalidGenreIds = string.Join(", ", invalidGenres.Select(g => g).ToList());
                return Result<PostDTO>.Failure($"Tag id(s) {invalidGenreIds} are not defined");
            }

            if (postRequestDTO.ThumbnailFile != null)
            {
                await _s3Services.DeleteFileAsync(postToUpdate.Thumbnail);
                string thumbnailUrl = await _s3Services.UploadFileAsync(postRequestDTO.ThumbnailFile);
                postToUpdate.Thumbnail = thumbnailUrl;
            }

            _mapper.Map(postRequestDTO, postToUpdate);

            await _postRepo.UpdateAsync(postToUpdate);

            Post? updatedMovie = await query
                .Include(m => m.PostTags)
                    .ThenInclude(mg => mg.Tag)
                    .FirstOrDefaultAsync();

            PostDTO blogDTO = _mapper.Map<PostDTO>(updatedMovie);

            return Result<PostDTO>.Success(blogDTO, "Post updated successfully");
        }

        public async Task<Result<bool>> DeletePost(int id)
        {
            Post? blogToDelete = await _postRepo.GetByIdAsync(id);
            if (blogToDelete is null)
            {
                return Result<bool>.Failure($"Blog id {id} not found");
            }

            await _s3Services.DeleteFileAsync(blogToDelete.Thumbnail);
            await _postRepo.RemoveAsync(blogToDelete);

            return Result<bool>.Success(true, "Blog deleted successfully");
        }

        public async Task<Result<PostDTO>> GetPostById(int id)
        {
            IQueryable<Post> query = _postRepo.GetBaseQuery(predicate: m => m.Id == id);

            Post? blog = await query
                .Include(m => m.PostTags)
                    .ThenInclude(mg => mg.Tag)
                .FirstOrDefaultAsync();

            if (blog is null)
            {
                return Result<PostDTO>.Failure($"Post id {id} not found");
            }

            PostDTO blogDTO = _mapper.Map<PostDTO>(blog);


            return Result<PostDTO>.Success(blogDTO, "Blog retrieved successfully");
        }

        public async Task<Result<List<PostDTO>>> FindAll(
            int pageNumber, 
            int pageSize, 
            Expression<Func<Post, bool>> predicate, 
            Func<IQueryable<Post>, IOrderedQueryable<Post>>? orderBy = null
        )
        {
            IQueryable<Post> query = _postRepo.GetBaseQuery(predicate);
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            IEnumerable<Post> blogs = await query
                .Include(m => m.PostTags)
                    .ThenInclude(mg => mg.Tag)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            List<PostDTO> blogDTOs = _mapper.Map<List<PostDTO>>(blogs);

            return Result<List<PostDTO>>.Success(blogDTOs, "Posts retrieved successfully");
        }

        public async Task<Result<int>> CountPost(Expression<Func<Post, bool>> predicate)
        {
            int totalCount = await _postRepo.CountAsync(predicate);
            return Result<int>.Success(totalCount, "Total post count retrieved successfully");
        }
    }
}
