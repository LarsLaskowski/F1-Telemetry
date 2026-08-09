using F1Server.Shared;

namespace F1Server.Tests.Shared;

/// <summary>
/// Class to test the file helper functions
/// </summary>
[TestClass]
public class FileFunctionsTests
{
    #region Methods

    /// <summary>
    /// Verifies that the game version is correctly read from a full-size sample packet file
    /// </summary>
    [TestMethod]
    public void GetGameVersionFromFileReturnsVersionFromHeader()
    {
        var version = FileFunctions.GetGameVersionFromFile(Path.Combine("SampleData", "F1-2025-Session.packet"));

        Assert.AreEqual(2025, version, "The parsed game version should match the sample file!");
    }

    /// <summary>
    /// Verifies that a file too short to contain the 2-byte game version does not yield a parsed value
    /// </summary>
    [TestMethod]
    public void GetGameVersionFromFileSingleByteFileReturnsZero()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            File.WriteAllBytes(tempFile, [0x01]);

            var version = FileFunctions.GetGameVersionFromFile(tempFile);

            Assert.AreEqual(0, version, "A 1-byte file must not yield a parsed game version!");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    /// Verifies that an empty file does not yield a parsed value
    /// </summary>
    [TestMethod]
    public void GetGameVersionFromFileEmptyFileReturnsZero()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            var version = FileFunctions.GetGameVersionFromFile(tempFile);

            Assert.AreEqual(0, version, "An empty file must not yield a parsed game version!");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    #endregion // Methods
}