namespace Entities.Dtos
{
    public class TokenResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;

        public string TokenType { get; set; } = "Bearer";

    }
}
