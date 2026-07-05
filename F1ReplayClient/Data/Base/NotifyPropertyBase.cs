using System.ComponentModel;

namespace F1ReplayClient.Data.Base;

/// <summary>
/// Base class for NotifyPropertyChanged events
/// </summary>
internal class NotifyPropertyBase : INotifyPropertyChanged
{
    #region INotifyPropertyChanged

    /// <summary>
    /// PropertyChanged event
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion // INotifyPropertyChanged

    #region Methods

    /// <summary>
    /// Property value has changed
    /// </summary>
    /// <param name="propertyName">Name of property</param>
    public void RaisePropertyChange(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion // Methods
}