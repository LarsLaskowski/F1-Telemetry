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
    /// Invokes <see cref="F1PacketTester.Program.TestSessionPacket"/> and captures the console output it writes
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

            F1PacketTester.Program.TestSessionPacket(filePath, gameVersion, HeaderSize, new ConsoleProgressBar(1, "Test"));

            return writer.ToString();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    /// Builds a synthetic packet file with a valid header for the given game version and raw packet type byte
    /// </summary>
    /// <param name="gameVersion">Game version written into the header</param>
    /// <param name="packetTypeId">Raw packet type byte written into the header (the parsed packet type is this value plus one)</param>
    /// <param name="totalLength">Total length of the packet file in bytes</param>
    /// <returns>Path to the temporary packet file</returns>
    private static string CreateHeaderOnlyPacketFile(ushort gameVersion, byte packetTypeId, int totalLength)
    {
        var data = new byte[totalLength];

        BitConverter.GetBytes(gameVersion).CopyTo(data, 0);

        // From 2023 the header carries an extra GameYear byte before the packet type
        var packetTypeOffset = gameVersion >= 2023 ? 6 : 5;

        data[packetTypeOffset] = packetTypeId;

        var filePath = Path.GetTempFileName();

        File.WriteAllBytes(filePath, data);

        return filePath;
    }

    /// <summary>
    /// Invokes <see cref="F1PacketTester.Program.FileTests"/> and captures the console output it writes
    /// </summary>
    /// <param name="filePath">Path to the packet file</param>
    /// <returns>Captured console output</returns>
    private static string InvokeFileTests(string filePath)
    {
        var originalOut = Console.Out;

        try
        {
            using var writer = new StringWriter();

            Console.SetOut(writer);

            F1PacketTester.Program.FileTests(filePath, new ConsoleProgressBar(1, "Test"));

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

    /// <summary>
    /// Check that a readable session packet is dispatched without a skip message
    /// </summary>
    [TestMethod]
    public void ProgramFileTestsValidSessionPacketWritesNoSkipMessage()
    {
        var filePath = CreateHeaderOnlyPacketFile(2024, 1, HeaderSize);

        try
        {
            var output = InvokeFileTests(filePath);

            Assert.DoesNotContain("Skipping", output, "Unexpected skip message for a valid, readable session packet!");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// Check that a readable event packet is dispatched without a skip message
    /// </summary>
    [TestMethod]
    public void ProgramFileTestsValidEventPacketWritesNoSkipMessage()
    {
        var filePath = CreateHeaderOnlyPacketFile(2024, 3, HeaderSize + 4);

        try
        {
            var output = InvokeFileTests(filePath);

            Assert.DoesNotContain("Skipping", output, "Unexpected skip message for a valid, readable event packet!");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// Check that a file which cannot be read is skipped with a message instead of aborting the batch
    /// </summary>
    [TestMethod]
    public void ProgramFileTestsUnreadableFileWritesSkipMessage()
    {
        var directoryPath = Directory.CreateTempSubdirectory().FullName;

        try
        {
            var output = InvokeFileTests(directoryPath);

            Assert.Contains("Skipping", output, "Expected a skip message when the file cannot be read!");
        }
        finally
        {
            Directory.Delete(directoryPath);
        }
    }

    /// <summary>
    /// Check that a packet shorter than the minimum header size writes a too-small message instead of throwing
    /// </summary>
    [TestMethod]
    public void ProgramFileTestsTooShortFileWritesTooSmallMessage()
    {
        var filePath = Path.GetTempFileName();

        File.WriteAllBytes(filePath, new byte[4]);

        try
        {
            var output = InvokeFileTests(filePath);

            Assert.Contains("too small to contain a packet header", output, "Expected a too-small message for a packet shorter than the minimum header size!");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// Check that a packet with an undefined packet type is skipped without writing a message
    /// </summary>
    [TestMethod]
    public void ProgramFileTestsUndefinedPacketTypeWritesNoMessage()
    {
        var filePath = CreateHeaderOnlyPacketFile(2024, 250, HeaderSize);

        try
        {
            var output = InvokeFileTests(filePath);

            Assert.AreEqual(string.Empty, output, "No message expected for an undefined packet type!");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// Check that a packet with an unrecognized game version is skipped without writing a message
    /// </summary>
    [TestMethod]
    public void ProgramFileTestsUnrecognizedGameVersionWritesNoMessage()
    {
        var filePath = CreateHeaderOnlyPacketFile(1999, 1, 23);

        try
        {
            var output = InvokeFileTests(filePath);

            Assert.AreEqual(string.Empty, output, "No message expected for an unrecognized game version!");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    #endregion // Methods
}