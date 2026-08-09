using F1Server.Shared;

namespace F1Server.Tests.Shared;

/// <summary>
/// Class to test the session detection class
/// </summary>
[TestClass]
public class DetectSessionDataTests
{
    #region Methods

    /// <summary>
    /// Verifies that a valid session packet file is detected among a set of non-matching files
    /// </summary>
    [TestMethod]
    public void DetectSessionDetectsSessionAmongMultipleFiles()
    {
        var tempDir = Directory.CreateTempSubdirectory(nameof(DetectSessionDataTests));

        try
        {
            var sessionFile = Path.Combine(tempDir.FullName, "session.packet");

            File.Copy(Path.Combine("SampleData", "F1-2025-Session.packet"), sessionFile);

            var otherFile = Path.Combine(tempDir.FullName, "other.packet");

            File.WriteAllBytes(otherFile, new byte[50]);

            var detector = new DetectSessionData([otherFile, sessionFile], 2025);

            var isDetected = detector.DetectSession();

            Assert.IsTrue(isDetected, "A valid session packet among the files should be detected!");
            Assert.IsNotNull(detector.SessionData, "SessionData should be populated once a session is detected!");
            Assert.IsEmpty(detector.LastError, "LastError should remain empty on success!");
        }
        finally
        {
            tempDir.Delete(true);
        }
    }

    /// <summary>
    /// Verifies that no session is reported when none of the files match the expected session packet size
    /// </summary>
    [TestMethod]
    public void DetectSessionNoMatchingFileReturnsFalse()
    {
        var tempDir = Directory.CreateTempSubdirectory(nameof(DetectSessionDataTests));

        try
        {
            var otherFile = Path.Combine(tempDir.FullName, "other.packet");

            File.WriteAllBytes(otherFile, new byte[50]);

            var detector = new DetectSessionData([otherFile], 2025);

            var isDetected = detector.DetectSession();

            Assert.IsFalse(isDetected, "No session should be detected without a matching file!");
            Assert.IsNull(detector.SessionData, "SessionData should remain null without a detected session!");
        }
        finally
        {
            tempDir.Delete(true);
        }
    }

    /// <summary>
    /// Verifies that the session file is still reliably detected when analyzed in parallel alongside many
    /// non-matching files, exercising the Parallel.ForEach path guarded against the shared detection state
    /// </summary>
    [TestMethod]
    public void DetectSessionWithManyFilesFindsSessionWithoutCorruption()
    {
        var tempDir = Directory.CreateTempSubdirectory(nameof(DetectSessionDataTests));

        try
        {
            var files = new List<string>();

            for (var index = 0; index < 20; index++)
            {
                var otherFile = Path.Combine(tempDir.FullName, $"other{index}.packet");

                File.WriteAllBytes(otherFile, new byte[50]);

                files.Add(otherFile);
            }

            var sessionFile = Path.Combine(tempDir.FullName, "session.packet");

            File.Copy(Path.Combine("SampleData", "F1-2025-Session.packet"), sessionFile);

            files.Add(sessionFile);

            var detector = new DetectSessionData(files, 2025);

            var isDetected = detector.DetectSession();

            Assert.IsTrue(isDetected, "The session file among many non-matching files should be detected!");
            Assert.IsNotNull(detector.SessionData, "SessionData should be populated once a session is detected!");
        }
        finally
        {
            tempDir.Delete(true);
        }
    }

    #endregion // Methods
}