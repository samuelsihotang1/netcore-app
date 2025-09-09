using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public static class SqlServerDbCleaner
    {
        public static async Task DropAllTablesAsync(AppDbContext db)
        {
            const string sql = @"
DECLARE @sql NVARCHAR(MAX);

-- 1) Drop semua foreign key constraints
SELECT @sql = STRING_AGG(
    'ALTER TABLE ' + QUOTENAME(s.name) + '.' + QUOTENAME(t.name) +
    ' DROP CONSTRAINT ' + QUOTENAME(fk.name) + ';', CHAR(10))
FROM sys.foreign_keys fk
JOIN sys.tables t  ON fk.parent_object_id = t.object_id
JOIN sys.schemas s ON t.schema_id = s.schema_id;

IF @sql IS NOT NULL EXEC sp_executesql @sql;

-- 2) Drop semua table
SELECT @sql = STRING_AGG(
    'DROP TABLE ' + QUOTENAME(s.name) + '.' + QUOTENAME(t.name) + ';', CHAR(10))
FROM sys.tables t
JOIN sys.schemas s ON t.schema_id = s.schema_id;

IF @sql IS NOT NULL EXEC sp_executesql @sql;
";
            await db.Database.ExecuteSqlRawAsync(sql);
        }
    }
}
