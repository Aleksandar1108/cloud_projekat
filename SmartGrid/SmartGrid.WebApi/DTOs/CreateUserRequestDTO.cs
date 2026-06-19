namespace SmartGrid.WebApi.DTOs
{
    public class CreateUserRequestDTO
    {
        public required string Email { get; set; }
        public required string Role { get; set; }
    }
}
