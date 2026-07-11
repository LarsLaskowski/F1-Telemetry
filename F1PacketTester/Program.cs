using System.ComponentModel;
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
    #region Constants

    /// <summary>
    /// Offset of the packet type byte for game versions before F1 2023
    /// (game version uint16, major, minor and packet version bytes)
    /// </summary>
    private const int PacketTypeOffset = 5;

    /// <summary>
    /// Offset of the packet type byte for F1 2023 and newer
    /// (an additional game year byte precedes the version fields)
    /// </summary>
    private const int PacketTypeOffset2023 = 6;

    /// <summary>
    /// Minimum number of bytes required to read the packet type from the header
    /// </summary>
    private const int MinimumHeaderSize = PacketTypeOffset2023 + 1;

    #endregion // Constants

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
        var data = File.ReadAllBytes(file).AsSpan();

        ref var memRef = ref MemoryMarshal.GetReference(data);

        if (gameVersion == 2024)
        {
            TestSessionPacket2024(packetHeaderSize, progressBar, fInfo, data, memRef);
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
    private static void TestSessionPacket2024(int packetHeaderSize, ConsoleProgressBar progressBar, FileInfo fInfo, Span<byte> data, byte memRef)
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
        var data = File.ReadAllBytes(file).AsSpan();

        if (data.Length < MinimumHeaderSize)
        {
            progressBar.WriteLine($"File {fInfo.Name} is too small to contain a packet header");

            return;
        }

        ref var memRef = ref MemoryMarshal.GetReference(data);

        // The game version is stored in the first two bytes of every packet header (uint16)
        var gameVersion = Unsafe.ReadUnaligned<ushort>(ref memRef);

        // Since F1 2023 the header contains an additional game year byte before the version
        // fields, which shifts the packet type byte by one
        var packetTypeOffset = gameVersion >= 2023 ? PacketTypeOffset2023 : PacketTypeOffset;

        // The packet type (zero based packet id) follows the version fields in the header
        var packetId = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref memRef, packetTypeOffset));

        var packetType = (PacketTypes)packetId + 1;

        if (Enum.IsDefined(packetType) == false)
        {
            return;
        }

        var packetHeaderSize = gameVersion switch
                               {
                                   2019 => ConstData.F12019HeaderSize,
                                   2020 => ConstData.F12020HeaderSize,
                                   2021 => ConstData.F12020HeaderSize,
                                   2022 => ConstData.F12020HeaderSize,
                                   2023 => ConstData.F12023HeaderSize,
                                   2024 => ConstData.F12024HeaderSize,
                                   2025 => ConstData.F12025HeaderSize,
                                   2026 => ConstData.F12026HeaderSize,
                                   _ => throw new InvalidEnumArgumentException()
                               };

        switch (packetType)
        {
            case PacketTypes.Event:
                {
                    TestEventPacket(file, packetHeaderSize, progressBar);
                }
                break;

            case PacketTypes.Session:
                {
                    TestSessionPacket(file, gameVersion, packetHeaderSize, progressBar);
                }
                break;
        }
    }

    /// <summary>
    /// Test event packet
    /// </summary>
    /// <param name="file">Path to file</param>
    /// <param name="packetHeaderSize">Size of the packet header</param>
    /// <param name="progressBar">Progress bar used to report findings without breaking the bar</param>
    private static void TestEventPacket(string file, int packetHeaderSize, ConsoleProgressBar progressBar)
    {
        var fInfo = new FileInfo(file);
        var data = File.ReadAllBytes(file).AsSpan();

        if (data.Length < packetHeaderSize + 4)
        {
            return;
        }

        ref var memRef = ref MemoryMarshal.GetReference(data);

        var eventCodeSpan = MemoryMarshal.CreateReadOnlySpan<byte>(ref Unsafe.Add(ref memRef, packetHeaderSize), 4);

        var eventString = Encoding.ASCII.GetString(eventCodeSpan).Trim('\0');

        if (eventString == "PENA"
            || eventString == "SPTP"
            || eventString == "FLBK")
        {
            progressBar.WriteLine($"Event string: {eventString} found in {fInfo.Name}");
        }
    }

    #endregion // Methods
}