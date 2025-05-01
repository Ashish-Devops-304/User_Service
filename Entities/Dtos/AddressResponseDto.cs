namespace Entities.Dtos
{
    public class AddressResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public long Phone { get; set; }

        public string Line1 { get; set; } = string.Empty;

        public string Line2 { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public int Pincode { get; set; }

        public string State { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;
    }
}
