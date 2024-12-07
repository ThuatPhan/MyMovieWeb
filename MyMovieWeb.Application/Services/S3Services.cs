using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MyMovieWeb.Application.Interfaces;

namespace MyMovieWeb.Application.Services
{
    public class S3Services : IS3Services
    {
        private readonly IConfiguration _configuration;
        private readonly AmazonS3Client _s3Client;
        private readonly string _bucketName;

        public S3Services(IConfiguration configuration)
        {
            _configuration = configuration;
            _bucketName = _configuration["AwsS3:BucketName"]!;

            _s3Client = new AmazonS3Client(Amazon.RegionEndpoint.APSoutheast1);
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var fileKey = Guid.NewGuid().ToString();
            var uploadRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                InputStream = file.OpenReadStream(),
                Key = fileKey,
                ContentType = file.ContentType,
            };

            var uploadResponse = await _s3Client.PutObjectAsync(uploadRequest);

            if (uploadResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return $"https://my-movie-file-bucket.s3.ap-southeast-1.amazonaws.com/{fileKey}";
            }

            throw new Exception($"Failed to upload file to bucket {_bucketName}");
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            string fileKey = fileUrl.Substring(fileUrl.LastIndexOf('/') + 1);
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = fileKey
            };

            var deleteResponse = await _s3Client.DeleteObjectAsync(deleteRequest);
            if (deleteResponse.HttpStatusCode != System.Net.HttpStatusCode.NoContent)
            {
                throw new Exception($"Failed to delete file from bucket {_bucketName}");
            }
        }
    }
}
