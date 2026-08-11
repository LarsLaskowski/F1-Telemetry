using System.Collections.Concurrent;
using System.Data.Common;
using System.Diagnostics;

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

    #region Fields

    /// <summary>
    /// Activities started for commands still in flight, keyed by <see cref="CommandCorrelatedEventData.CommandId"/> so
    /// the matching *Executed callback can stop the same span the *Executing callback started
    /// </summary>
    private readonly ConcurrentDictionary<Guid, Activity> _pendingActivities = new();

    #endregion // Fields

    #region Private methods

    /// <summary>
    /// Starts an <see cref="Activity"/> for a command about to be executed and keeps it open until the matching
    /// *Executed callback reports completion via <see cref="StopCommandActivity"/>
    /// </summary>
    /// <param name="activityName">Name of the activity, matching the intercepted *Executing method</param>
    /// <param name="command">Database command about to be executed</param>
    /// <param name="eventData">Event data correlating this call with its matching *Executed callback</param>
    private void StartCommandActivity(string activityName, DbCommand command, CommandEventData eventData)
    {
        var activity = AppActivity.SrvSource.StartActivity(activityName);

        if (activity is null)
        {
            return;
        }

        activity.SetTag(DbCommandTag, command.CommandText);

        _pendingActivities[eventData.CommandId] = activity;
    }

    /// <summary>
    /// Stops and disposes the <see cref="Activity"/> started for the command identified by <paramref name="eventData"/>,
    /// so the span duration covers the full execution of the command
    /// </summary>
    /// <param name="eventData">Event data correlating this call with the *Executing callback that started the activity</param>
    private void StopCommandActivity(CommandExecutedEventData eventData)
    {
        if (_pendingActivities.TryRemove(eventData.CommandId, out var activity))
        {
            activity.Dispose();
        }
    }

    #endregion // Private methods

    #region DbCommandInterceptor

    /// <inheritdoc/>
    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        StartCommandActivity(nameof(ReaderExecuting), command, eventData);

        return base.ReaderExecuting(command, eventData, result);
    }

    /// <inheritdoc/>
    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        StopCommandActivity(eventData);

        return base.ReaderExecuted(command, eventData, result);
    }

    /// <inheritdoc/>
    public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
    {
        StartCommandActivity(nameof(ScalarExecuting), command, eventData);

        return base.ScalarExecuting(command, eventData, result);
    }

    /// <inheritdoc/>
    public override object? ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
    {
        StopCommandActivity(eventData);

        return base.ScalarExecuted(command, eventData, result);
    }

    /// <inheritdoc/>
    public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
    {
        StartCommandActivity(nameof(NonQueryExecuting), command, eventData);

        return base.NonQueryExecuting(command, eventData, result);
    }

    /// <inheritdoc/>
    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        StopCommandActivity(eventData);

        return base.NonQueryExecuted(command, eventData, result);
    }

    /// <inheritdoc/>
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
    {
        StartCommandActivity(nameof(ReaderExecutingAsync), command, eventData);

        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    /// <inheritdoc/>
    public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
    {
        StopCommandActivity(eventData);

        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    /// <inheritdoc/>
    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
    {
        StartCommandActivity(nameof(ScalarExecutingAsync), command, eventData);

        return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }

    /// <inheritdoc/>
    public override ValueTask<object?> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object? result, CancellationToken cancellationToken = default)
    {
        StopCommandActivity(eventData);

        return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
    }

    /// <inheritdoc/>
    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        StartCommandActivity(nameof(NonQueryExecutingAsync), command, eventData);

        return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }

    /// <inheritdoc/>
    public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        StopCommandActivity(eventData);

        return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }

    #endregion // DbCommandInterceptor
}