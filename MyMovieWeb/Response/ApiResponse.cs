namespace MyMovieWeb.Presentation.Response
{
    public class ApiResponse<T>
    {
        public bool Success { get; private set; }
        public T Data { get; private set; }
        public string Message { get; private set; }

        public static ApiResponse<T> SuccessResponse(T data, string message)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static ApiResponse<T> FailureResponse(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = default,
                Message = message
            };
        }

    }
}
