namespace F1SessionFolderRename;

/// <summary>
/// The main class for the application
/// </summary>
internal static class Program
{
    #region Methods

    /// <summary>
    /// The main entry point for the application
    /// </summary>
    /// <param name="args">Arguments</param>
    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            if (Directory.Exists(args[0]))
            {
                var processor = new FolderRenameProcessor(args[0]);

                var foldersRenamed = processor.StartRenaming();

                Console.WriteLine($"Folders renamed: {foldersRenamed}");
            }
        }
        else
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("      path_to_folders_with_logged_packet_files");
            Console.WriteLine();
        }
    }

    #endregion // Methods
}