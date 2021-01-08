// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace GDX.IO.Compression
{
    public static class TarFile
    {
        /// <summary>
        ///     Extracts all the files in the specified tarball to a directory on the file system.
        /// </summary>
        /// <param name="sourceArchiveFileName">The path to the archive that is to be extracted.</param>
        /// <param name="destinationDirectoryName">
        ///     The path to the directory in which to place the extracted files, specified as a
        ///     relative or absolute path. A relative path is interpreted as relative to the current working directory.
        /// </param>
        /// <param name="forceGZipDataFormat">Enforce inflating the file via a <see cref="GZipStream"/>.</param>
        public static void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName, bool forceGZipDataFormat = false)
        {
            // We need to handle the gzip first before we address the archive itself
            if (forceGZipDataFormat || sourceArchiveFileName.EndsWith(".gz", StringComparison.InvariantCultureIgnoreCase))
            {
                using FileStream stream = File.OpenRead(sourceArchiveFileName);
                using GZipStream gzip = new GZipStream(stream, CompressionMode.Decompress);
                using MemoryStream memoryStream = new MemoryStream();

                // Loop through the stream
                int readByteCount;
                byte[] readBuffer = new byte[4096];
                do
                {
                    readByteCount = gzip.Read(readBuffer, 0, 4096);
                    memoryStream.Write(readBuffer, 0, readByteCount);
                } while (readByteCount == 4096);

                memoryStream.Seek(0, SeekOrigin.Begin);
                ExtractStream(memoryStream, destinationDirectoryName);
            }
            else
            {

                using FileStream fileStream = File.OpenRead(sourceArchiveFileName);
                ExtractStream(fileStream, destinationDirectoryName);
            }
        }


        /// <summary>
        ///     Extract a tarball to the <paramref name="destinationDirectoryName"/>.
        /// </summary>
        /// <param name="sourceStream">The <see cref="Stream"/> which to extract from.</param>
        /// <param name="destinationDirectoryName">Output directory to write the files.</param>
        private static void ExtractStream(Stream sourceStream, string destinationDirectoryName)
        {
            byte[] buffer = new byte[100];
            while (true)
            {
                sourceStream.Read(buffer, 0, 100);
                string currentName = Encoding.ASCII.GetString(buffer).Trim('\0');

                if (string.IsNullOrWhiteSpace(currentName))
                {
                    break;
                }

                sourceStream.Seek(24, SeekOrigin.Current);
                sourceStream.Read(buffer, 0, 12);

                long fileSize = Convert.ToInt64(Encoding.UTF8.GetString(buffer, 0, 12).Trim('\0').Trim(), 8);

                sourceStream.Seek(376L, SeekOrigin.Current);

                string output = Path.Combine(destinationDirectoryName, currentName);
                if (!Directory.Exists(Path.GetDirectoryName(output)))
                {
                    string directoryPath = Path.GetDirectoryName(output);
                    Directory.CreateDirectory(directoryPath);

                    //TODO: Need to handle permissions on nearly created
                    // DirectoryInfo directory = new DirectoryInfo(directoryPath);
                    // DirectorySecurity security = directory.GetAccessControl();
                    //
                    // security.AddAccessRule(new FileSystemAccessRule(@"MYDOMAIN\JohnDoe",
                    //     FileSystemRights.Modify,
                    //     AccessControlType.Deny));
                    //
                    // directory.SetAccessControl(security);
                }

                if (!currentName.Equals("./", StringComparison.InvariantCulture))
                {
                    using FileStream newFileStream = File.Open(output, FileMode.OpenOrCreate, FileAccess.Write);
                    byte[] fileContentBuffer = new byte[fileSize];
                    int newFileContentBufferLength = fileContentBuffer.Length;

                    sourceStream.Read(fileContentBuffer, 0, newFileContentBufferLength);
                    newFileStream.Write(fileContentBuffer, 0, newFileContentBufferLength);
                }

                long pos = sourceStream.Position;

                long offset = 512 - pos % 512;
                if (offset == 512)
                {
                    offset = 0;
                }

                sourceStream.Seek(offset, SeekOrigin.Current);
            }
        }
    }
}