using System.Security.Cryptography;
using System.Text;
using backend.Data;
using backend.Dto;
using backend.Interface;

namespace backend.Dao
{
    public class DatabaseAdminDao : IDatabaseAdminDao
    {
        private readonly AppDbContext _db;
        private readonly IServiceProvider _sp;
        private readonly IConfiguration _config;

        public DatabaseAdminDao(AppDbContext db, IServiceProvider sp, IConfiguration config)
        {
            _db = db;
            _sp = sp;
            _config = config;
        }

        public async Task<DatabaseResetResultDto> ResetAsync(DatabaseResetRequestDto input, CancellationToken ct = default)
        {
            if (!IsPasswordValid(input?.Password))
                throw new UnauthorizedAccessException("Invalid reset password.");

            await SqlServerDbCleaner.DropAllTablesAsync(_db);
            await DbSeeder.InitializeAsync(_sp);

            return new DatabaseResetResultDto { Message = "All tables dropped, migrated, and seeded." };
        }

        private bool IsPasswordValid(string? supplied)
        {
            var expected = _config["DB_RESET_PASSWORD"]
                           ?? Environment.GetEnvironmentVariable("DB_RESET_PASSWORD");
            if (string.IsNullOrWhiteSpace(expected) || string.IsNullOrWhiteSpace(supplied)) return false;

            var a = Encoding.UTF8.GetBytes(expected);
            var b = Encoding.UTF8.GetBytes(supplied);
            return a.Length == b.Length && CryptographicOperations.FixedTimeEquals(a, b);
        }
    }
}
