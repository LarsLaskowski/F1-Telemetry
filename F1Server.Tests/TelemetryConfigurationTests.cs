using F1Server.Telemetry;

namespace F1Server.Tests;

/// <summary>
/// Class to test the telemetry configuration class
/// </summary>
[TestClass]
public class TelemetryConfigurationTests
{
    #region Test methods

    /// <summary>
    /// A complete set of connection data marks the telemetry configuration as configured
    /// </summary>
    [TestMethod]
    public void TelemetryConfigurationSetConfigurationDataCompleteDataReturnsTrue()
    {
        var telemetryConfiguration = new TelemetryConfiguration();

        var isConfigured = telemetryConfiguration.SetConfigurationData("http://localhost:8086", "bucket", "organization", "token");

        Assert.IsTrue(isConfigured, "Complete connection data should configure the telemetry!");
        Assert.IsTrue(telemetryConfiguration.IsConfigured, "The configured state should be stored!");
        Assert.AreEqual("http://localhost:8086", telemetryConfiguration.InfluxDbHost, "The InfluxDB host should be stored!");
        Assert.AreEqual("bucket", telemetryConfiguration.Bucket, "The bucket should be stored!");
        Assert.AreEqual("organization", telemetryConfiguration.Organization, "The organization should be stored!");
        Assert.AreEqual("token", telemetryConfiguration.Token, "The token should be stored!");
    }

    /// <summary>
    /// A missing connection value leaves the telemetry configuration unconfigured
    /// </summary>
    /// <param name="influxDbHost">InfluxDB host</param>
    /// <param name="bucket">Bucket</param>
    /// <param name="organization">Organization</param>
    /// <param name="token">Token</param>
    [TestMethod]
    [DataRow("", "bucket", "organization", "token")]
    [DataRow("http://localhost:8086", "", "organization", "token")]
    [DataRow("http://localhost:8086", "bucket", "", "token")]
    [DataRow("http://localhost:8086", "bucket", "organization", "")]
    public void TelemetryConfigurationSetConfigurationDataMissingValueReturnsFalse(string influxDbHost, string bucket, string organization, string token)
    {
        var telemetryConfiguration = new TelemetryConfiguration();

        var isConfigured = telemetryConfiguration.SetConfigurationData(influxDbHost, bucket, organization, token);

        Assert.IsFalse(isConfigured, "Incomplete connection data must not configure the telemetry!");
        Assert.IsFalse(telemetryConfiguration.IsConfigured, "The unconfigured state should be stored!");
    }

    #endregion // Test methods
}