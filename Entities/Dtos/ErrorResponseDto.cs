namespace Entities.Dtos
{
    public class ErrorResponseDto
    {
        public int StatusCode { get; set; }

        public string Message { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

    }
}
