namespace SmartGrid.Application.Common
{
    public class UserDTO
    {
        public string IdUser { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string CreatedAt { get; set; }
        public bool IsActivated { get; set; }
    }
}
