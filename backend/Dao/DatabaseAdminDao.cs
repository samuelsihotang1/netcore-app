using System.Security.Cryptography;
using System.Text;
using backend.Data;
using backend.Dto;
using backend.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Dao
{
    public class DatabaseAdminDao : IDatabaseAdminDao
    {
        private readonly IServiceProvider _sp;
        private readonly IConfiguration _config;

        public DatabaseAdminDao(IServiceProvider sp, IConfiguration config)
        {
            _sp = sp;
            _config = config;
        }

        public async Task<DatabaseResetResultDto> ResetAsync(DatabaseResetRequestDto input, CancellationToken ct = default)
        {
            if (!IsPasswordValid(input?.Password))
                throw new UnauthorizedAccessException("Invalid reset password.");

            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await SqlServerDbCleaner.DropAllTablesAsync(db);
            await db.Database.MigrateAsync(ct);
            await DbSeeder.InitializeAsync(scope.ServiceProvider);

            return new DatabaseResetResultDto { Message = "All tables dropped, migrated, and seeded." };
        }

        private bool IsPasswordValid(string? supplied)
        {
            var expected = _config["DB_RESET_PASSWORD"]
                           ?? Environment.GetEnvironmentVariable("DB_RESET_PASSWORD");

            if (string.IsNullOrWhiteSpace(expected) || string.IsNullOrWhiteSpace(supplied))
                return false;

            var a = Encoding.UTF8.GetBytes(expected);
            var b = Encoding.UTF8.GetBytes(supplied);
            return a.Length == b.Length && CryptographicOperations.FixedTimeEquals(a, b);
        }
    }
}
