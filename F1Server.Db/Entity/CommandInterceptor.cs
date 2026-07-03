using System.Data.Common;

using F1Server.Core.Observability;

using Microsoft.EntityFrameworkCore.Diagnostics;

namespace F1Server.Db.Entity;

/// <summary>
/// Intercepts the creation of <see cref="DbCommand"/> instances during database operations
/// </summary>
internal class CommandInterceptor : DbCommandInterceptor
{
    #region Const fields

    /// <summary>
    /// Represents the tag used to identify database commands in logging or tracing contexts
    /// </summary>
    private const string DbCommandTag = "db.command";

    #endregion // Const fields

    #region DbCommandInterceptor

    /// <inheritdoc/>
    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        using (var currentActivity = AppActivity.SrvSource.StartActivity(nameof(ReaderExecuting)))
        {
            currentActivity?.SetTag(DbCommandTag, command.CommandText);
        }

        return base.ReaderExecuting(command, eventData, result);
    }

    /// <inheritdoc/>
    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        using var currentActivity = AppActivity.SrvSource.StartActivity(nameof(ReaderExecuted));

        return base.ReaderExecuted(command, eventData, result);
    }

    /// <inheritdoc/>
    public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
    {
        using (var currentActivity = AppActivity.SrvSource.StartActivity(nameof(ScalarExecuting)))
        {
            currentActivity?.SetTag(DbCommandTag, command.CommandText);
        }

        return base.ScalarExecuting(command, eventData, result);
    }

    /// <inheritdoc/>
    public override object? ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
    {
        using var currentActivity = AppActivity.SrvSource.StartActivity(nameof(ScalarExecuted));

        return base.ScalarExecuted(command, eventData, result);
    }

    /// <inheritdoc/>
    public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
    {
        using (var currentActivity = AppActivity.SrvSource.StartActivity(nameof(NonQueryExecuting)))
        {
            currentActivity?.SetTag(DbCommandTag, command.CommandText);
        }

        return base.NonQueryExecuting(command, eventData, result);
    }

    /// <inheritdoc/>
    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        using var currentActivity = AppActivity.SrvSource.StartActivity(nameof(NonQueryExecuted));

        return base.NonQueryExecuted(command, eventData, result);
    }

    /// <inheritdoc/>
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
    {
        using (var currentActivity = AppActivity.SrvSource.StartActivity(nameof(ReaderExecutingAsync)))
        {
            currentActivity?.SetTag(DbCommandTag, command.CommandText);
        }

        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    /// <inheritdoc/>
    public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
    {
        using (var currentActivity = AppActivity.SrvSource.StartActivity(nameof(ReaderExecutedAsync)))
        {
            currentActivity?.SetTag(DbCommandTag, command.CommandText);
        }

        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    /// <inheritdoc/>
    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
    {
        using (var currentActivity = AppActivity.SrvSource.StartActivity(nameof(ScalarExecutingAsync)))
        {
            currentActivity?.SetTag(DbCommandTag, command.CommandText);
        }

        return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }

    /// <inheritdoc/>
    public override ValueTask<object?> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object? result, CancellationToken cancellationToken = default)
    {
        using var currentActivity = AppActivity.SrvSource.StartActivity(nameof(ScalarExecutedAsync));

        return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
    }

    /// <inheritdoc/>
    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        using (var currentActivity = AppActivity.SrvSource.StartActivity(nameof(NonQueryExecutingAsync)))
        {
            currentActivity?.SetTag(DbCommandTag, command.CommandText);
        }

        return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }

    /// <inheritdoc/>
    public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        using var currentActivity = AppActivity.SrvSource.StartActivity(nameof(NonQueryExecutedAsync));

        return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }

    #endregion // DbCommandInterceptor
}