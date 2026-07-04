using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Tests that int8 packet fields keep their documented negative value instead of arriving as an unsigned byte
/// </summary>
[TestClass]
public class PacketSignedByteFieldsTests
{
    #region Constants

    /// <summary>
    /// Relative offset of DRSAllowed within the base car status data of a single car
    /// </summary>
    private const int DrsAllowedOffset = 22;

    /// <summary>
    /// Relative offset of the FIA flags byte within the F1 2020 car status data of a single car
    /// </summary>
    private const int FiaFlagsOffset2020 = 42;

    /// <summary>
    /// Relative offset of Gear within the car telemetry data of a single car
    /// </summary>
    private const int GearOffset = 15;

    /// <summary>
    /// Relative offset of TrackTemperature within the base session data
    /// </summary>
    private const int TrackTemperatureOffset = 1;

    /// <summary>
    /// Relative offset of AirTemperature within the base session data
    /// </summary>
    private const int AirTemperatureOffset = 2;

    /// <summary>
    /// Relative offset of the marshal zone count within the base session data
    /// </summary>
    private const int MarshalZonesCountOffset = 18;

    /// <summary>
    /// Relative offset of the first marshal zone's flag byte within the base session data
    /// </summary>
    private const int FirstMarshalZoneFlagOffset = 23;

    #endregion // Constants

    #region Static methods

    /// <summary>
    /// Creates a synthetic packet header for a given game version
    /// </summary>
    /// <param name="gameVersion">Game version to encode in the header</param>
    /// <param name="headerSize">Header size of the game version</param>
    /// <returns>Parsed packet header</returns>
    private static PacketHeader CreatePacketHeader(int gameVersion, int headerSize)
    {
        var rawData = new byte[headerSize];

        rawData[0] = (byte)(gameVersion & 0xFF);
        rawData[1] = (byte)((gameVersion >> 8) & 0xFF);

        var receivedData = new ReceivedPacketData();

        receivedData.SetRawData(rawData);

        Assert.IsNotNull(receivedData.PacketHeader, $"Synthetic header for game version {gameVersion} could not be created!");

        return receivedData.PacketHeader;
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// DRSAllowed must decode the documented -1 (unknown) instead of the raw byte value 255
    /// </summary>
    [TestMethod]
    public void GetCarStatusDrsAllowedByteValue255ReturnsMinusOne()
    {
        var packetHeader = CreatePacketHeader(2020, ConstData.F12020HeaderSize);

        var packetContent = new byte[ConstData.F12020HeaderSize + ConstData.F12020CarStatusSize];

        packetContent[ConstData.F12020HeaderSize + DrsAllowedOffset] = 0xFF;

        var packetAnalyzer = new PacketAnalyzer();

        var carStatus = packetAnalyzer.GetCarStatus(packetHeader, packetContent);

        if (carStatus is CarStatus { PacketData: CarStatus2020 carStatus2020 })
        {
            Assert.AreEqual(-1, carStatus2020.CarStatusData[0].DRSAllowed, "DRSAllowed must decode the documented unknown value -1!");
        }
        else
        {
            Assert.Fail("Synthetic F1 2020 car status packet did not produce a CarStatus2020 object!");
        }
    }

    /// <summary>
    /// The vehicle FIA flags must decode the documented -1 (unknown) instead of the raw byte value 255
    /// </summary>
    [TestMethod]
    public void GetCarStatusFiaFlagsByteValue255ReturnsUnknown()
    {
        var packetHeader = CreatePacketHeader(2020, ConstData.F12020HeaderSize);

        var packetContent = new byte[ConstData.F12020HeaderSize + ConstData.F12020CarStatusSize];

        packetContent[ConstData.F12020HeaderSize + FiaFlagsOffset2020] = 0xFF;

        var packetAnalyzer = new PacketAnalyzer();

        var carStatus = packetAnalyzer.GetCarStatus(packetHeader, packetContent);

        if (carStatus is CarStatus { PacketData: CarStatus2020 carStatus2020 })
        {
            Assert.AreEqual(VehicleFiaFlagColor.Unknown, carStatus2020.CarStatusData[0].FiaFlags, "FiaFlags must decode the documented unknown value -1!");
        }
        else
        {
            Assert.Fail("Synthetic F1 2020 car status packet did not produce a CarStatus2020 object!");
        }
    }

    /// <summary>
    /// Gear must decode the documented -1 (reverse gear) instead of the raw byte value 255
    /// </summary>
    [TestMethod]
    public void GetCarTelemetryGearByteValue255ReturnsMinusOne()
    {
        var packetHeader = CreatePacketHeader(2020, ConstData.F12020HeaderSize);

        var packetContent = new byte[ConstData.F12020HeaderSize + ConstData.F12020CarTelemetrySize];

        packetContent[ConstData.F12020HeaderSize + GearOffset] = 0xFF;

        var packetAnalyzer = new PacketAnalyzer();

        var carTelemetry = packetAnalyzer.GetCarTelemetry(packetHeader, packetContent);

        if (carTelemetry is CarTelemetry { PacketData: CarTelemetry2020 carTelemetry2020 })
        {
            Assert.AreEqual(-1, carTelemetry2020.CarTelemetryData[0].Gear, "Gear must decode reverse gear as -1!");
        }
        else
        {
            Assert.Fail("Synthetic F1 2020 car telemetry packet did not produce a CarTelemetry2020 object!");
        }
    }

    /// <summary>
    /// Track and air temperature must decode negative int8 values instead of the raw unsigned byte value
    /// </summary>
    [TestMethod]
    public void GetSessionDataTemperaturesByteValue255ReturnMinusOne()
    {
        var packetHeader = CreatePacketHeader(2020, ConstData.F12020HeaderSize);

        var packetContent = new byte[ConstData.F12020HeaderSize + ConstData.F12020SessionSize];

        packetContent[ConstData.F12020HeaderSize + TrackTemperatureOffset] = 0xFF;
        packetContent[ConstData.F12020HeaderSize + AirTemperatureOffset] = 0xFF;

        var packetAnalyzer = new PacketAnalyzer();

        var sessionData = packetAnalyzer.GetSessionData(packetHeader, packetContent);

        if (sessionData is SessionData { PacketData: ISessionData2020 sessionData2020 })
        {
            Assert.AreEqual(-1, sessionData2020.TrackTemperature, "TrackTemperature must decode negative int8 values!");
            Assert.AreEqual(-1, sessionData2020.AirTemperature, "AirTemperature must decode negative int8 values!");
        }
        else
        {
            Assert.Fail("Synthetic F1 2020 session packet did not produce an ISessionData2020 object!");
        }
    }

    /// <summary>
    /// Marshal zone flags must decode the documented -1 (invalid) instead of the raw byte value 255
    /// </summary>
    [TestMethod]
    public void GetSessionDataZoneFlagByteValue255ReturnsInvalid()
    {
        var packetHeader = CreatePacketHeader(2020, ConstData.F12020HeaderSize);

        var packetContent = new byte[ConstData.F12020HeaderSize + ConstData.F12020SessionSize];

        packetContent[ConstData.F12020HeaderSize + MarshalZonesCountOffset] = 1;
        packetContent[ConstData.F12020HeaderSize + FirstMarshalZoneFlagOffset] = 0xFF;

        var packetAnalyzer = new PacketAnalyzer();

        var sessionData = packetAnalyzer.GetSessionData(packetHeader, packetContent);

        if (sessionData is SessionData { PacketData: ISessionData2020 sessionData2020 })
        {
            Assert.AreEqual(ZoneFlagColor.Invalid, sessionData2020.MarshalZone[0].ZoneFlag, "ZoneFlag must decode the documented invalid value -1!");
        }
        else
        {
            Assert.Fail("Synthetic F1 2020 session packet did not produce an ISessionData2020 object!");
        }
    }

    #endregion // Methods
}