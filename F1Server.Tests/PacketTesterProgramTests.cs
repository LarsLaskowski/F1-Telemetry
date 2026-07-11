using F1PacketTester;

namespace F1Server.Tests;

/// <summary>
/// Tests of the F1 2024 session packet inspection in the F1PacketTester tool
/// </summary>
[TestClass]
public class PacketTesterProgramTests
{
    #region Constants

    /// <summary>
    /// Packet header size used by F1 2024 packets
    /// </summary>
    private const int HeaderSize = 29;

    /// <summary>
    /// Offset of the marshal zones byte relative to the packet header
    /// </summary>
    private const int MarshalZonesOffset = 19;

    /// <summary>
    /// Offset of the safety car status byte relative to the packet header
    /// </summary>
    private const int SafetyCarStatusOffset = 124;

    /// <summary>
    /// Offset of the AI difficulty byte relative to the packet header
    /// </summary>
    private const int AiDifficultyOffset = 640;

    #endregion // Constants

    #region Static methods

    /// <summary>
    /// Builds a minimal synthetic F1 2024 session packet with a controlled byte value at the given offset
    /// </summary>
    /// <param name="offset">Offset relative to the packet header</param>
    /// <param name="value">Byte value to place at the offset</param>
    /// <returns>Path to the temporary packet file</returns>
    private static string CreateSessionPacketFile(int offset, byte value)
    {
        var data = new byte[HeaderSize + AiDifficultyOffset + 1];

        data[HeaderSize + offset] = value;

        var filePath = Path.GetTempFileName();

        File.WriteAllBytes(filePath, data);

        return filePath;
    }

    /// <summary>
    /// Invokes <see cref="Program.TestSessionPacket"/> and captures the console output it writes
    /// </summary>
    /// <param name="filePath">Path to the packet file</param>
    /// <param name="gameVersion">Game version passed to the method</param>
    /// <returns>Captured console output</returns>
    private static string InvokeTestSessionPacket(string filePath, int gameVersion = 2024)
    {
        var originalOut = Console.Out;

        try
        {
            using var writer = new StringWriter();

            Console.SetOut(writer);

            Program.TestSessionPacket(filePath, gameVersion, HeaderSize, new ConsoleProgressBar(1, "Test"));

            return writer.ToString();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Check that a positive marshal zones value is reported
    /// </summary>
    [TestMethod]
    public void ProgramTestSessionPacketMarshalZonesPresentWritesMessage()
    {
        var filePath = CreateSessionPacketFile(MarshalZonesOffset, 3);

        try
        {
            var output = InvokeTestSessionPacket(filePath);

            Assert.Contains("Marshal zones (3)", output, "Expected marshal zones message not found!");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// Check that a zero marshal zones value is not reported
    /// </summary>
    [TestMethod]
    public void ProgramTestSessionPacketMarshalZonesZeroWritesNothing()
    {
        var filePath = CreateSessionPacketFile(MarshalZonesOffset, 0);

        try
        {
            var output = InvokeTestSessionPacket(filePath);

            Assert.DoesNotContain("Marshal zones", output, "Unexpected marshal zones message found!");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// Check that an active safety car status is reported (regression test for the previously inverted "== 0" check)
    /// </summary>
    [TestMethod]
    public void ProgramTestSessionPacketSafetyCarStatusPresentWritesMessage()
    {
        var filePath = CreateSessionPacketFile(SafetyCarStatusOffset, 2);

        try
        {
            var output = InvokeTestSessionPacket(filePath);

            Assert.Contains("Safety car status (2)", output, "Expected safety car status message not found!");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// Check that no safety car (status 0) is not reported
    /// </summary>
    [TestMethod]
    public void ProgramTestSessionPacketSafetyCarStatusZeroWritesNothing()
    {
        var filePath = CreateSessionPacketFile(SafetyCarStatusOffset, 0);

        try
        {
            var output = InvokeTestSessionPacket(filePath);

            Assert.DoesNotContain("Safety car status", output, "Unexpected safety car status message found!");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// Check that a positive AI difficulty value is reported
    /// </summary>
    [TestMethod]
    public void ProgramTestSessionPacketAiDifficultyPresentWritesMessage()
    {
        var filePath = CreateSessionPacketFile(AiDifficultyOffset, 50);

        try
        {
            var output = InvokeTestSessionPacket(filePath);

            Assert.Contains("AI difficulty (50)", output, "Expected AI difficulty message not found!");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// Check that a zero AI difficulty value is not reported
    /// </summary>
    [TestMethod]
    public void ProgramTestSessionPacketAiDifficultyZeroWritesNothing()
    {
        var filePath = CreateSessionPacketFile(AiDifficultyOffset, 0);

        try
        {
            var output = InvokeTestSessionPacket(filePath);

            Assert.DoesNotContain("AI difficulty", output, "Unexpected AI difficulty message found!");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// Check that packets from game versions other than F1 2024 are not inspected
    /// </summary>
    [TestMethod]
    public void ProgramTestSessionPacketNonF12024GameVersionWritesNothing()
    {
        var filePath = CreateSessionPacketFile(SafetyCarStatusOffset, 5);

        try
        {
            var output = InvokeTestSessionPacket(filePath, gameVersion: 2023);

            Assert.AreEqual(string.Empty, output, "No message expected for non-F1 2024 packets!");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// Check that a truncated packet does not throw and produces no message
    /// </summary>
    [TestMethod]
    public void ProgramTestSessionPacketTruncatedPacketDoesNotThrow()
    {
        var filePath = Path.GetTempFileName();

        File.WriteAllBytes(filePath, new byte[HeaderSize + 5]);

        try
        {
            var output = InvokeTestSessionPacket(filePath);

            Assert.AreEqual(string.Empty, output, "No message expected for a truncated packet!");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    #endregion // Methods
}