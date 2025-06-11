using AuctionAPI.Models.DTOs;

namespace AuctionAPI.Mappers
{
    public static class ApiResponseMapper
    {
        public static ApiResponse<T> Success<T>(T data, string message = "Request successful.")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> Fail<T>(string message, Dictionary<string, string[]>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default!,
                Errors = errors
            };
        }

        public static ApiResponse<T> Created<T>(T data, string message = "Resource created successfully.")
        {
            return Success(data, message);
        }

        public static ApiResponse<T> BadRequest<T>(string message)
        {
            return Fail<T>(message);
        }

        public static ApiResponse<T> Unauthorized<T>(string message = "Unauthorized access.")
        {
            return Fail<T>(message);
        }

        public static ApiResponse<T> Forbidden<T>(string message)
        {
            return Fail<T>(message);
        }

        public static ApiResponse<T> NotFound<T>(string message)
        {
            return Fail<T>(message);
        }

        public static ApiResponse<T> Conflict<T>(string message)
        {
            return Fail<T>(message);
        }

        public static ApiResponse<T> InternalError<T>(Exception ex)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = "Internal server error.",
                Errors = new Dictionary<string, string[]> { { "Exception", new[] { ex.Message } } }
            };
        }
    }

}