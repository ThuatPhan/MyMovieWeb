using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace MyMovieWeb.Application.Helper
{
    public class FileUploadHelper
    {
        private readonly HttpClient _httpClient;

        public FileUploadHelper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            using var imageFormData = new MultipartFormDataContent();
            using var imageStream = imageFile.OpenReadStream();
            var imageContent = new StreamContent(imageStream);

            imageFormData.Add(imageContent, "image", imageFile.FileName);

            var imageResponse = await _httpClient.PostAsync("http://localhost:3000/upload-image", imageFormData);

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
            else
            {
                throw new Exception("Failed to get image url");
            }
        }

        public async Task<string> UploadVideoAsync(IFormFile videoFile)
        {
            using var videoFormData = new MultipartFormDataContent();
            using var videoStream = videoFile.OpenReadStream();
            var videoContent = new StreamContent(videoStream);

            videoFormData.Add(videoContent, "video", videoFile.FileName);

            var videoResponse = await _httpClient.PostAsync("http://localhost:3000/upload-video", videoFormData);

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
            else
            {
                throw new Exception("Failed to get video url");
            }
        }
    }
}
