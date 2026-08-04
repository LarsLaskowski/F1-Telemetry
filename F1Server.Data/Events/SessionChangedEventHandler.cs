namespace F1Server.Data.Events;

/// <summary>
/// Delegate for the session changed event
/// </summary>
/// <param name="sender">Sender</param>
/// <param name="sessionChangedEventArgs">Eventdata</param>
public delegate void SessionChangedEventHandler(object sender, SessionChangedEventArgs sessionChangedEventArgs);