using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace MyMovieWeb.Application.Helper
{
    public class FileUploadHelper
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _serverFileHost;

        public FileUploadHelper(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _serverFileHost = _configuration["ServerFileHost"]!;
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            using var imageFormData = new MultipartFormDataContent();
            using var imageStream = imageFile.OpenReadStream();
            var imageContent = new StreamContent(imageStream);

            imageFormData.Add(imageContent, "image", imageFile.FileName);

            var imageResponse = await _httpClient.PostAsync($"{_serverFileHost}/upload-image", imageFormData);

            if (!imageResponse.IsSuccessStatusCode)
            {
                throw new Exception("Failed to upload image");
            }

            var imageResponseString = await imageResponse.Content.ReadAsStringAsync();
            var imageResponseJson = JsonSerializer.Deserialize<Dictionary<string, string>>(imageResponseString);

            if (imageResponseJson != null && imageResponseJson.TryGetValue("photoUrl", out var imageUrl))
            {
                return imageUrl;
            }

            throw new Exception("Failed to get image url");
        }

        public async Task<string> UploadVideoAsync(IFormFile videoFile)
        {
            using var videoFormData = new MultipartFormDataContent();
            using var videoStream = videoFile.OpenReadStream();
            var videoContent = new StreamContent(videoStream);

            videoFormData.Add(videoContent, "video", videoFile.FileName);

            var videoResponse = await _httpClient.PostAsync($"{_serverFileHost}/upload-video", videoFormData);

            if (!videoResponse.IsSuccessStatusCode)
            {
                throw new Exception("Failed to upload video");
            }

            var videoResponseString = await videoResponse.Content.ReadAsStringAsync();
            var videoResponseJson = JsonSerializer.Deserialize<Dictionary<string, string>>(videoResponseString);

            if (videoResponseJson != null && videoResponseJson.TryGetValue("videoUrl", out var videoUrl))
            {
                return videoUrl;
            }

            throw new Exception("Failed to get video url");
        }

        public async Task<bool> DeleteImageFileAsync(string imageFilePath)
        {
            string fileName = imageFilePath.Substring(imageFilePath.LastIndexOf('/') + 1);
            var response = await _httpClient.DeleteAsync($"{_serverFileHost}/delete-image/{fileName}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            throw new Exception("Failed to delete image");
        }

        public async Task<bool> DeleteVideoFileAsync(string videoFilePath)
        {
            string fileName = videoFilePath.Substring(videoFilePath.LastIndexOf('/') + 1);
            var response = await _httpClient.DeleteAsync($"{_serverFileHost}/delete-video/{fileName}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            throw new Exception("Failed to delete image");
        }
    }
}
