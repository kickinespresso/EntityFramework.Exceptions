using Microsoft.Data.Sqlite;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using static SQLitePCL.raw;

namespace EntityFramework.Exceptions.Sqlite
{
    class SqliteExceptionProcessorStateManager : ExceptionProcessorStateManager<SqliteException >
    {
        public SqliteExceptionProcessorStateManager(StateManagerDependencies dependencies) : base(dependencies)
        {
        }

        protected override DatabaseError? GetDatabaseError(SqliteException  dbException)
        {
            switch (dbException.SqliteErrorCode)
            {
                case SQLITE_CONSTRAINT_PRIMARYKEY:
                case SQLITE_CONSTRAINT_UNIQUE:
                case SQLITE_CONSTRAINT:
                    return DatabaseError.UniqueConstraint;
                case SQLITE_TOOBIG:
                    return DatabaseError.MaxLength;
                case SQLITE_MISMATCH:
                case SQLITE_RANGE:
                    return DatabaseError.NumericOverflow;
                case SQLITE_CONSTRAINT_NOTNULL:
                    return DatabaseError.CannotInsertNull;
                case SQLITE_CONSTRAINT_FOREIGNKEY:
                    return DatabaseError.ReferenceConstraint;
                default:
                    return null;
            }
        }
    }

    public static class ExceptionProcessorExtensions
    {
        public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
        {
            self.ReplaceService<IStateManager, SqliteExceptionProcessorStateManager>();
            return self;
        }

        public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
        {
            self.ReplaceService<IStateManager, SqliteExceptionProcessorStateManager>();
            return self;
        }
    }
}
