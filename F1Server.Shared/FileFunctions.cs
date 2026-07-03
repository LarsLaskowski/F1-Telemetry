using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace F1Server.Shared;

/// <summary>
/// Class with functions to read data from F1 packet files
/// </summary>
public static class FileFunctions
{
    #region Methods

    /// <summary>
    /// Read the game version from header of file
    /// </summary>
    /// <param name="fileName">Name of the file</param>
    /// <returns>Game version</returns>
    public static int GetGameVersionFromFile(string fileName)
    {
        var gameVersion = 0;

        using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            // Check game version from first file
            var buffer = ArrayPool<byte>.Shared.Rent(30);

            try
            {
                var bytesRead = fs.Read(buffer, 0, 30);

                if (bytesRead > 0)
                {
                    ref var memRef = ref MemoryMarshal.GetReference(buffer.AsSpan());

                    gameVersion = Unsafe.ReadUnaligned<ushort>(ref memRef);
                }
            }
            catch
            {
                // Ignore exceptions in this step
            }
            finally
            {
                fs.Close();

                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        return gameVersion;
    }

    #endregion // Methods
}