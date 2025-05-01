namespace Entities.Dtos
{
    public class PaymentResponseDto
    {
        public Guid Id { get; set; }

        public string Type { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? PaymentValue { get; set; } 
    }
}
