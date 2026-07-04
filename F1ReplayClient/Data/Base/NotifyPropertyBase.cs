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
    /// <param name="propertName">Name of property</param>
    public void RaisePropertyChange(string propertName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertName));
    }

    #endregion // Methods
}