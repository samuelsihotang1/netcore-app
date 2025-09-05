namespace backend.Dtos
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public DateTime ExpiresAtUtc { get; set; }

        public long UserId { get; set; }
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
