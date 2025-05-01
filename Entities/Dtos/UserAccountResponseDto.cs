namespace Entities.Dtos
{
    public class UserAccountResponseDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public long Phone { get; set; }

        public string Role { get; set; } = string.Empty;

        public ICollection<AddressResponseDto> Addresses { get; set; } = new List<AddressResponseDto>();

        public ICollection<PaymentResponseDto> Payments { get; set; } = new List<PaymentResponseDto>();
    }
}
