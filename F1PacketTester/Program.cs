using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using F1Server.Core.Data;
using F1Server.Core.Enumerations;

[assembly: InternalsVisibleToAttribute("F1Server.Tests")]

namespace F1PacketTester;

/// <summary>
/// Program to test packet data
/// </summary>
internal static class Program
{
    #region Methods

    /// <summary>
    /// The main entry point for the application
    /// </summary>
    /// <param name="args">Arguments passed to the application</param>
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        if (args.Length > 1)
        {
            if (Directory.Exists(args[0]))
            {
                var matchingDirectories = CollectMatchingDirectories(args[0], args[1]);

                var files = matchingDirectories.SelectMany(directory => Directory.EnumerateFiles(directory, "*"))
                                               .ToList();

                if (files.Count == 0)
                {
                    Console.WriteLine("No files found");

                    return;
                }

                var progressBar = new ConsoleProgressBar(files.Count, "Processing files");

                foreach (var file in files)
                {
                    FileTests(file, progressBar);

                    progressBar.Increment();
                }

                progressBar.Complete();
            }
            else
            {
                Console.WriteLine("Directory does not exist");
            }
        }
        else
        {
            Console.WriteLine("No directory specified");
        }
    }

    /// <summary>
    /// Test session packet
    /// </summary>
    /// <param name="file">Path to file</param>
    /// <param name="gameVersion">Game version</param>
    /// <param name="packetHeaderSize">Size of the packet header</param>
    /// <param name="progressBar">Progress bar used to report findings without breaking the bar</param>
    internal static void TestSessionPacket(string file, int gameVersion, int packetHeaderSize, ConsoleProgressBar progressBar)
    {
        var fInfo = new FileInfo(file);
        var rawData = File.ReadAllBytes(file);

        TestSessionPacket(fInfo, rawData, gameVersion, packetHeaderSize, progressBar);
    }

    /// <summary>
    /// Test session packet using packet data that has already been loaded
    /// </summary>
    /// <param name="fInfo">File information</param>
    /// <param name="rawData">Already loaded packet data</param>
    /// <param name="gameVersion">Game version</param>
    /// <param name="packetHeaderSize">Size of the packet header</param>
    /// <param name="progressBar">Progress bar used to report findings without breaking the bar</param>
    private static void TestSessionPacket(FileInfo fInfo, byte[] rawData, int gameVersion, int packetHeaderSize, ConsoleProgressBar progressBar)
    {
        var data = rawData.AsSpan();

        ref var memRef = ref MemoryMarshal.GetReference(data);

        if (gameVersion == 2024)
        {
            TestSessionPacket2024(packetHeaderSize, progressBar, fInfo, data, ref memRef);
        }
    }

    /// <summary>
    /// Test session packet for F1 2024
    /// </summary>
    /// <param name="packetHeaderSize">Size of the packet header</param>
    /// <param name="progressBar">Progress bar used to report findings without breaking the bar</param>
    /// <param name="fInfo">File information</param>
    /// <param name="data">Packet data</param>
    /// <param name="memRef">Reference to the packet data</param>
    private static void TestSessionPacket2024(int packetHeaderSize, ConsoleProgressBar progressBar, FileInfo fInfo, Span<byte> data, ref byte memRef)
    {
        if (data.Length > packetHeaderSize + 19)
        {
            byte marshalZones = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref memRef, packetHeaderSize + 19));

            if (marshalZones > 0)
            {
                progressBar.WriteLine($"Marshal zones ({marshalZones}) found in {fInfo.Name}");
            }
        }

        if (data.Length > packetHeaderSize + 124)
        {
            byte safetyCarStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref memRef, packetHeaderSize + 124));

            if (safetyCarStatus > 0)
            {
                progressBar.WriteLine($"Safety car status ({safetyCarStatus}) found in {fInfo.Name}");
            }
        }

        if (data.Length > packetHeaderSize + 640)
        {
            byte aiDifficulty = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref memRef, packetHeaderSize + 640));

            if (aiDifficulty > 0)
            {
                progressBar.WriteLine($"AI difficulty ({aiDifficulty}) found in {fInfo.Name}");
            }
        }
    }

    /// <summary>
    /// Walks the start directory and all of its subdirectories and collects every subfolder
    /// that matches the given name
    /// </summary>
    /// <param name="startDirectory">Directory to start the search from</param>
    /// <param name="subFolderName">Name of the subfolder to look for</param>
    /// <returns>List of full paths to the matching subfolders</returns>
    private static List<string> CollectMatchingDirectories(string startDirectory, string subFolderName)
    {
        var matchingDirectories = new List<string>();
        var spinner = new ConsoleSpinner("Scanning directories");
        var scanned = 0;

        // The start directory itself may already contain the searched subfolder
        var rootCandidate = Path.Combine(startDirectory, subFolderName);

        if (Directory.Exists(rootCandidate))
        {
            matchingDirectories.Add(rootCandidate);
        }

        // Walk through every subdirectory below the start directory
        foreach (var directory in Directory.EnumerateDirectories(startDirectory, "*", SearchOption.AllDirectories))
        {
            scanned++;

            var candidate = Path.Combine(directory, subFolderName);

            if (Directory.Exists(candidate))
            {
                matchingDirectories.Add(candidate);
            }

            spinner.Update($"{scanned} scanned, {matchingDirectories.Count} matches");
        }

        spinner.Complete($"{scanned} directories scanned, {matchingDirectories.Count} matches");

        return matchingDirectories;
    }

    /// <summary>
    /// File tests
    /// </summary>
    /// <param name="file">Path to file</param>
    /// <param name="progressBar">Progress bar used to report findings without breaking the bar</param>
    private static void FileTests(string file, ConsoleProgressBar progressBar)
    {
        var fInfo = new FileInfo(file);
        byte[] rawData;

        try
        {
            rawData = File.ReadAllBytes(file);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            progressBar.WriteLine($"Skipping {fInfo.Name}: {ex.Message}");

            return;
        }

        var packet = new ReceivedPacketData();

        packet.SetRawData(rawData);

        if (packet.PacketHeader is null)
        {
            if (packet.HeaderRejectionCode is HeaderRejectionCode.PacketTooShort or HeaderRejectionCode.Undersized2023Header)
            {
                progressBar.WriteLine($"File {fInfo.Name} is too small to contain a packet header");
            }

            return;
        }

        var packetType = packet.PacketHeader.PacketType;

        if (Enum.IsDefined(packetType) == false)
        {
            return;
        }

        var packetHeaderSize = packet.PacketHeader.HeaderSize;

        if (packetHeaderSize == 0)
        {
            return;
        }

        switch (packetType)
        {
            case PacketTypes.Event:
                {
                    TestEventPacket(fInfo, rawData, packetHeaderSize, progressBar);
                }
                break;

            case PacketTypes.Session:
                {
                    TestSessionPacket(fInfo, rawData, packet.PacketHeader.GameVersion, packetHeaderSize, progressBar);
                }
                break;
        }
    }

    /// <summary>
    /// Test event packet using packet data that has already been loaded
    /// </summary>
    /// <param name="fInfo">File information</param>
    /// <param name="rawData">Already loaded packet data</param>
    /// <param name="packetHeaderSize">Size of the packet header</param>
    /// <param name="progressBar">Progress bar used to report findings without breaking the bar</param>
    private static void TestEventPacket(FileInfo fInfo, byte[] rawData, int packetHeaderSize, ConsoleProgressBar progressBar)
    {
        var data = rawData.AsSpan();

        if (data.Length < packetHeaderSize + 4)
        {
            return;
        }

        ref var memRef = ref MemoryMarshal.GetReference(data);

        var eventCodeSpan = MemoryMarshal.CreateReadOnlySpan<byte>(ref Unsafe.Add(ref memRef, packetHeaderSize), 4);

        var eventString = Encoding.ASCII.GetString(eventCodeSpan).Trim('\0');

        if (eventString == EventCodes.PenaltyIssued
            || eventString == EventCodes.SpeedTrapTriggered
            || eventString == EventCodes.Flashback)
        {
            progressBar.WriteLine($"Event string: {eventString} found in {fInfo.Name}");
        }
    }

    #endregion // Methods
}