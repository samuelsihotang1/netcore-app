using backend.Dto;

namespace backend.Interface
{
    public interface IDatabaseAdminDao
    {
        Task<DatabaseResetResultDto> ResetAsync(DatabaseResetRequestDto input, CancellationToken ct = default);
    }
}
