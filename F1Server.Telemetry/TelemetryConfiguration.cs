namespace F1Server.Telemetry;

/// <summary>
/// Represents the configuration settings required to connect to an InfluxDB instance
/// </summary>
public class TelemetryConfiguration
{
    #region Properties

    /// <summary>
    /// Is the configuration valid?
    /// </summary>
    public bool IsConfigured { get; private set; }

    /// <summary>
    /// Gets the hostname or IP address of the InfluxDB server
    /// </summary>
    public string InfluxDbHost { get; private set; }

    /// <summary>
    /// Gets the name of the storage bucket
    /// </summary>
    public string Bucket { get; private set; }

    /// <summary>
    /// Gets the name of the organization in InfluxDB
    /// </summary>
    public string Organization { get; private set; }

    /// <summary>
    /// Gets the authentication token for accessing InfluxDB
    /// </summary>
    public string Token { get; private set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Sets the configuration data required to connect to an InfluxDB instance
    /// </summary>
    /// <param name="influxDbHost">The hostname or IP address of the InfluxDB server. Cannot be null or empty</param>
    /// <param name="bucket">The name of the bucket in InfluxDB where data will be stored. Cannot be null or empty</param>
    /// <param name="organization">The name of the organization associated with the InfluxDB instance. Cannot be null or empty</param>
    /// <param name="token">The authentication token used to access the InfluxDB instance. Cannot be null or empty</param>
    /// <returns><see langword="true"/> if the configuration is valid; otherwise, <see langword="false"/></returns>
    public bool SetConfigurationData(string influxDbHost, string bucket, string organization, string token)
    {
        InfluxDbHost = influxDbHost;
        Bucket = bucket;
        Organization = organization;
        Token = token;

        IsConfigured = string.IsNullOrEmpty(InfluxDbHost) == false
                       && string.IsNullOrEmpty(Bucket) == false
                       && string.IsNullOrEmpty(Organization) == false
                       && string.IsNullOrEmpty(Token) == false;

        return IsConfigured;
    }

    #endregion // Methods
}