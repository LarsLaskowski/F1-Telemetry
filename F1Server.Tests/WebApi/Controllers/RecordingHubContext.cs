using System.Collections.Concurrent;

using F1Server.WebApi.Hubs;

using Microsoft.AspNetCore.SignalR;

namespace F1Server.Tests.WebApi.Controllers;

/// <summary>
/// SignalR hub context recording every message sent to its clients to test the broadcasts of the controllers
/// </summary>
internal sealed class RecordingHubContext : IHubContext<LiveSessionHub>, IHubClients, IClientProxy, IGroupManager, IDisposable
{
    #region Fields

    /// <summary>
    /// Names of the sent hub methods with their arguments
    /// </summary>
    private readonly ConcurrentQueue<(string Method, object?[] Arguments)> _messages = new();

    /// <summary>
    /// Signaled whenever a message was sent
    /// </summary>
    private readonly AutoResetEvent _messageSent = new(false);

    #endregion // Fields

    #region Methods

    /// <summary>
    /// Waits until a message of the given hub method was sent
    /// </summary>
    /// <param name="method">Name of the hub method</param>
    /// <param name="timeout">Maximum waiting time in milliseconds</param>
    /// <returns>Arguments of the first sent message of the hub method, or null if none was sent within the timeout</returns>
    public object?[]? WaitForMessage(string method, int timeout)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(timeout);

        object?[]? arguments = null;

        while (arguments is null && DateTime.UtcNow < deadline)
        {
            arguments = _messages.FirstOrDefault(m => m.Method == method).Arguments;

            if (arguments is null)
            {
                _messageSent.WaitOne(TimeSpan.FromMilliseconds(100));
            }
        }

        return arguments;
    }

    #endregion // Methods

    #region IHubContext

    /// <inheritdoc/>
    IHubClients IHubContext<LiveSessionHub>.Clients => this;

    /// <inheritdoc/>
    IGroupManager IHubContext<LiveSessionHub>.Groups => this;

    #endregion // IHubContext

    #region IHubClients

    /// <inheritdoc/>
    public IClientProxy All => this;

    /// <inheritdoc/>
    public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds)
    {
        return this;
    }

    /// <inheritdoc/>
    public IClientProxy Client(string connectionId)
    {
        return this;
    }

    /// <inheritdoc/>
    public IClientProxy Clients(IReadOnlyList<string> connectionIds)
    {
        return this;
    }

    /// <inheritdoc/>
    public IClientProxy Group(string groupName)
    {
        return this;
    }

    /// <inheritdoc/>
    public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds)
    {
        return this;
    }

    /// <inheritdoc/>
    public IClientProxy Groups(IReadOnlyList<string> groupNames)
    {
        return this;
    }

    /// <inheritdoc/>
    public IClientProxy User(string userId)
    {
        return this;
    }

    /// <inheritdoc/>
    public IClientProxy Users(IReadOnlyList<string> userIds)
    {
        return this;
    }

    #endregion // IHubClients

    #region IClientProxy

    /// <inheritdoc/>
    public Task SendCoreAsync(string method, object?[] args, CancellationToken cancellationToken = default)
    {
        _messages.Enqueue((method, args));
        _messageSent.Set();

        return Task.CompletedTask;
    }

    #endregion // IClientProxy

    #region IGroupManager

    /// <inheritdoc/>
    public Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    #endregion // IGroupManager

    #region IDisposable

    /// <inheritdoc/>
    public void Dispose()
    {
        _messageSent.Dispose();
    }

    #endregion // IDisposable
}