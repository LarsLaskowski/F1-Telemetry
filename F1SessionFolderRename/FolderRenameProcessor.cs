using System.Text.RegularExpressions;

using F1Server.Core.Enumerations;
using F1Server.Shared;
using F1Server.Shared.Data;

namespace F1SessionFolderRename;

/// <summary>
/// The class that processes the folder renaming
/// </summary>
internal partial class FolderRenameProcessor
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="FolderRenameProcessor"/> class
    /// </summary>
    /// <param name="directoryToProcess">Path to a directory where the renaming should be processed</param>
    public FolderRenameProcessor(string directoryToProcess)
    {
        DirectoryToProcess = directoryToProcess;
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Directory to process for renaming
    /// </summary>
    public string DirectoryToProcess { get; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Starts the renaming of the folders
    /// </summary>
    /// <returns>Number of renamend directories</returns>
    public int StartRenaming()
    {
        var directoriesRenamed = 0;

        if (string.IsNullOrEmpty(DirectoryToProcess) == false
            && Directory.Exists(DirectoryToProcess))
        {
            Directory.EnumerateDirectories(DirectoryToProcess, "*", SearchOption.TopDirectoryOnly)
                     .Where(dir => GetDirectorySearchExpression().IsMatch(Path.GetFileName(dir)))
                     .ToList()
                     .ForEach(dir =>
                              {
                                  var isRenamed = ProcessDirectory(dir);

                                  if (isRenamed)
                                  {
                                      directoriesRenamed++;
                                  }
                              });
        }

        return directoriesRenamed;
    }

    /// <summary>
    /// Gets the regular expression to search for directories
    /// </summary>
    /// <returns>Regualar expression</returns>
    [GeneratedRegex(@"^\d+$", RegexOptions.IgnoreCase, 150)]
    private partial Regex GetDirectorySearchExpression();

    /// <summary>
    /// Processes the directory to rename
    /// </summary>
    /// <param name="directoryToRename">Name of the directory</param>
    /// <returns>Is renamed?</returns>
    private bool ProcessDirectory(string directoryToRename)
    {
        var isRenamed = false;

        var filesInDirectory = Directory.EnumerateFiles(directoryToRename, "packet-*", SearchOption.AllDirectories).ToList();

        if (filesInDirectory.Count > 0)
        {
            Console.WriteLine($"Processing directory {directoryToRename}");

            foreach (var file in filesInDirectory)
            {
                var gameVersion = FileFunctions.GetGameVersionFromFile(file);

                if (gameVersion > 0)
                {
                    var sessionDetector = new DetectSessionData(filesInDirectory, gameVersion);

                    var isSessionDetected = sessionDetector.DetectSession();

                    if (isSessionDetected && sessionDetector.SessionData != null)
                    {
                        isRenamed = RenameDirectory(directoryToRename, sessionDetector.SessionData!);

                        Console.WriteLine($"   renamed: {isRenamed}");
                    }

                    break;
                }
            }
        }

        return isRenamed;
    }

    /// <summary>
    /// Renames the directory
    /// </summary>
    /// <param name="directoryToRename">Source directory</param>
    /// <param name="sessionData">Session information</param>
    /// <returns>Renamed?</returns>
    private bool RenameDirectory(string directoryToRename, SessionDataInfo sessionData)
    {
        var isRenamed = false;

        var newDirectoryName = $"{sessionData.TrackName}_";

        if (sessionData.FormulaType != Formula.F1Modern)
        {
            if (sessionData.FormulaType == Formula.F2)
            {
                newDirectoryName += "F2_";
            }
            else if (sessionData.FormulaType == Formula.F12026)
            {
                newDirectoryName += "F1_";
            }
            else
            {
                newDirectoryName += $"{sessionData.FormulaType}_";
            }
        }

        newDirectoryName = AppendSessionType(newDirectoryName, sessionData);

        var fullDirName = Directory.GetParent(directoryToRename)?.FullName;

        if (string.IsNullOrEmpty(fullDirName) == false)
        {
            var newDirectoryPath = Path.Combine(fullDirName, newDirectoryName);

            if (Directory.Exists(newDirectoryPath) == false)
            {
                try
                {
                    Directory.Move(directoryToRename, newDirectoryPath);

                    isRenamed = true;
                }
                catch
                {
                    // Ignore exceptions
                }
            }
        }

        return isRenamed;
    }

    /// <summary>
    /// Appends the session type to the directory name
    /// </summary>
    /// <param name="newDirectoryName">Directory name</param>
    /// <param name="sessionData">Session information</param>
    /// <returns>Directory name with session type appended</returns>
    private string AppendSessionType(string newDirectoryName, SessionDataInfo sessionData)
    {
        return sessionData.SessionType switch
               {
                   SessionType.Practice1 => newDirectoryName + "Practice_1",
                   SessionType.Practice2 => newDirectoryName + "Practice_2",
                   SessionType.Practice3 => newDirectoryName + "Practice_3",
                   SessionType.ShortPractice => newDirectoryName + "ShortPractice",
                   SessionType.Qualifying1 => newDirectoryName + "Qualifying_1",
                   SessionType.Qualifying2 => newDirectoryName + "Qualifying_2",
                   SessionType.Qualifying3 => newDirectoryName + "Qualifying_3",
                   SessionType.ShortQualifying => newDirectoryName + "ShortQualifying",
                   SessionType.OneShotQualifying => newDirectoryName + "OneShotQualifying",
                   SessionType.SprintShootout1 => newDirectoryName + "SprintShootout_1",
                   SessionType.SprintShootout2 => newDirectoryName + "SprintShootout_2",
                   SessionType.SprintShootout3 => newDirectoryName + "SprintShootout_3",
                   SessionType.ShortSprintShootout => newDirectoryName + "ShortSprintShootout",
                   SessionType.OneShotSprintShootout => newDirectoryName + "OneShotSprintShootout",
                   SessionType.Race => newDirectoryName + "Race",
                   SessionType.Race2 => newDirectoryName + "Race2",
                   SessionType.Race3 => newDirectoryName + "Race3",
                   SessionType.TimeTrial => newDirectoryName + "TimeTrial",
                   SessionType.Sprint => newDirectoryName + "Sprint",
                   _ => newDirectoryName
               };
    }

    #endregion // Methods
}