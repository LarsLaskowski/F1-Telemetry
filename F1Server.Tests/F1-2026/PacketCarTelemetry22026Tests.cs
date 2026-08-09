using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Tests;

/// <summary>
/// Class to test the additional car telemetry (CarTelemetry2) packet files, which are new in F1 2026
/// </summary>
[TestClass]
public class PacketCarTelemetry22026Tests
{
    #region Constants

    private const string SampleFile = @"SampleData/F1-2026-CarTelemetry2.packet";

    #endregion // Constants

    #region Fields

    private static PacketAnalyzer _packetAnalyzer;
    private static ReceivedPacketData _packetData;

    #endregion // Fields

    #region Initializer/Cleanup

    /// <summary>
    /// Class initializer
    /// </summary>
    /// <param name="testContext">Context</param>
    [ClassInitialize]
    public static void PacketCarTelemetry2Init(TestContext testContext)
    {
        var isFile = File.Exists(SampleFile);

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            var fileContent = File.ReadAllBytes(SampleFile);

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(fileContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of additional car telemetry packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2026-CarTelemetry2.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Static methods

    /// <summary>
    /// Reads the additional car telemetry data of the sample packet
    /// </summary>
    /// <returns>Additional car telemetry data of the sample packet</returns>
    private static ICarTelemetry2Base GetCarTelemetry2()
    {
        var packetHeader = _packetData.PacketHeader;

        Assert.IsNotNull(packetHeader, "Sample packet has no header!");

        var telemetryData = _packetAnalyzer.GetCarTelemetry2(packetHeader, File.ReadAllBytes(SampleFile));

        Assert.IsInstanceOfType<CarTelemetry2>(telemetryData, "Packet was not transformed into additional car telemetry!");

        var packetData = ((CarTelemetry2)telemetryData).PacketData;

        Assert.IsNotNull(packetData, "Additional car telemetry packet contains no data!");

        return packetData;
    }

    #endregion // Static methods

    #region Methods F1 2026

    /// <summary>
    /// Check whether the given file is an additional car telemetry packet
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetry2CheckCarTelemetry22026IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.CarTelemetry2;

        Assert.IsTrue(isCorrect, "Packet is not an additional car telemetry packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2026 packet
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetry2CheckCarTelemetry22026IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2026;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2026 packet!");
    }

    /// <summary>
    /// Check that the analyzer constructs the F1 2026 additional car telemetry object
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetry22026IsCarTelemetry2Object()
    {
        Assert.IsInstanceOfType<CarTelemetry22026>(GetCarTelemetry2(), "Additional car telemetry is not a F1 2026 object!");
    }

    /// <summary>
    /// Check that the packet carries one data set per car of the F1 2026 grid
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetry22026HasDataForAllCars()
    {
        Assert.HasCount(ConstData.F12026MaxCars, GetCarTelemetry2().CarTelemetry2Data, "Wrong number of cars in the additional car telemetry!");
    }

    /// <summary>
    /// Check the first car against the values of the sample packet
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetry2FirstCar2026ExpectedValues()
    {
        var carData = GetCarTelemetry2().CarTelemetry2Data[0];

        Assert.AreEqual(ActiveAeroMode.Corner, carData.ActiveAeroMode, "Wrong active aero mode of the first car!");
        Assert.IsTrue(carData.ActiveAeroAvailable, "Active aero must be available for the first car!");
        Assert.AreEqual(0, carData.ActiveAeroActivationDistance, "Wrong active aero activation distance of the first car!");
        Assert.IsTrue(carData.OvertakeAvailable, "Overtake mode must be available for the first car!");
        Assert.IsFalse(carData.OvertakeActive, "Overtake mode must not be active for the first car!");
        Assert.IsTrue(carData.Is2026Regulations, "The first car must run under the 2026 regulations!");
    }

    /// <summary>
    /// Check the second car, whose active aero mode and activation distance differ from the first
    /// car, so a wrong per-car stride of ten bytes is detected
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetry2SecondCar2026ExpectedValues()
    {
        var carData = GetCarTelemetry2().CarTelemetry2Data[1];

        Assert.AreEqual(ActiveAeroMode.Straight, carData.ActiveAeroMode, "Wrong active aero mode of the second car!");
        Assert.AreEqual(10, carData.ActiveAeroActivationDistance, "Wrong active aero activation distance of the second car!");
        Assert.AreEqual(25, carData.OvertakeActivationDistance, "Wrong overtake activation distance of the second car!");
    }

    /// <summary>
    /// Check the player car, which is the only car with an active overtake mode in the sample
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetry2PlayerCar2026ExpectedValues()
    {
        var carData = GetCarTelemetry2().CarTelemetry2Data[19];

        Assert.IsTrue(carData.OvertakeActive, "Overtake mode must be active for the player car!");
        Assert.AreEqual(190, carData.ActiveAeroActivationDistance, "Wrong active aero activation distance of the player car!");
        Assert.AreEqual(475, carData.OvertakeActivationDistance, "Wrong overtake activation distance of the player car!");
    }

    /// <summary>
    /// Check the last car of the grid, which verifies that the parser reaches the end of the
    /// 24 car array without drifting
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetry2LastCar2026ExpectedValues()
    {
        var carData = GetCarTelemetry2().CarTelemetry2Data[23];

        Assert.AreEqual(ActiveAeroMode.Straight, carData.ActiveAeroMode, "Wrong active aero mode of the last car!");
        Assert.IsFalse(carData.ActiveAeroAvailable, "Active aero must not be available for the last car!");
        Assert.AreEqual(230, carData.ActiveAeroActivationDistance, "Wrong active aero activation distance of the last car!");
    }

    /// <summary>
    /// Check that the wrong way flag is only set for the single car that carries it in the sample
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetry2WrongWay2026ExpectedValues()
    {
        var carTelemetry2Data = GetCarTelemetry2().CarTelemetry2Data;

        Assert.IsTrue(carTelemetry2Data[5].IsDrivingWrongWay, "Car 5 must be flagged as driving the wrong way!");
        Assert.IsFalse(carTelemetry2Data[4].IsDrivingWrongWay, "Car 4 must not be flagged as driving the wrong way!");
    }

    #endregion // Methods F1 2026
}