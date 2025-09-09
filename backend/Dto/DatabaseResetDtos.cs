namespace backend.Dto
{
    public class DatabaseResetRequestDto
    {
        public string Password { get; set; } = null!;
    }

    public class DatabaseResetResultDto
    {
        public string Message { get; set; } = null!;
    }
}
