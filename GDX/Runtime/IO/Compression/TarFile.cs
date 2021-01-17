﻿// dotBunny licenses this file to you under the MIT license.
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
        /// <param name="forceGZipDataFormat">Enforce inflating the file via a <see cref="GZipStream" />.</param>
        public static void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName,
            bool forceGZipDataFormat = false)
        {
            const int readBufferSize = 4096;

            // We need to handle the gzip first before we address the archive itself
            if (forceGZipDataFormat ||
                sourceArchiveFileName.EndsWith(".gz", StringComparison.InvariantCultureIgnoreCase))
            {
                using FileStream stream = File.OpenRead(sourceArchiveFileName);
                using GZipStream gzip = new GZipStream(stream, CompressionMode.Decompress);
                using MemoryStream memoryStream = new MemoryStream();

                // Loop through the stream
                int readByteCount;
                byte[] readBuffer = new byte[readBufferSize];
                do
                {
                    readByteCount = gzip.Read(readBuffer, 0, readBufferSize);
                    memoryStream.Write(readBuffer, 0, readByteCount);
                } while (readByteCount == readBufferSize);

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
        ///     Extract a tarball to the <paramref name="destinationDirectoryName" />.
        /// </summary>
        /// <param name="sourceStream">The <see cref="Stream" /> which to extract from.</param>
        /// <param name="destinationDirectoryName">Output directory to write the files.</param>
        private static void ExtractStream(Stream sourceStream, string destinationDirectoryName)
        {
            const int readBufferSize = 100;
            const int contentOffset = 512;
            byte[] readBuffer = new byte[readBufferSize];
            while (true)
            {
                sourceStream.Read(readBuffer, 0, readBufferSize);
                string currentName = Encoding.ASCII.GetString(readBuffer).Trim('\0');

                if (string.IsNullOrWhiteSpace(currentName))
                {
                    break;
                }

                string destinationFilePath = Path.Combine(destinationDirectoryName, currentName);
                sourceStream.Seek(24, SeekOrigin.Current);
                sourceStream.Read(readBuffer, 0, 12);
                long fileSize = Convert.ToInt64(Encoding.UTF8.GetString(readBuffer, 0, 12).Trim('\0').Trim(), 8);
                sourceStream.Seek(376L, SeekOrigin.Current);

                // Do we need to make a directory?
                string parentDirectory = Path.GetDirectoryName(destinationFilePath);
                if (!Directory.Exists(parentDirectory))
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    DirectoryInfo directoryInfo = Directory.CreateDirectory(parentDirectory);
                    directoryInfo.Attributes &= ~FileAttributes.ReadOnly;
                }

                // Don't try to make directories as files
                if (!currentName.Equals("./", StringComparison.InvariantCulture) &&
                    !currentName.EndsWith("/") &&
                    !currentName.EndsWith("\\"))
                {
                    using FileStream newFileStream =
                        File.Open(destinationFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                    byte[] fileContentBuffer = new byte[fileSize];
                    int newFileContentBufferLength = fileContentBuffer.Length;
                    sourceStream.Read(fileContentBuffer, 0, newFileContentBufferLength);
                    newFileStream.Write(fileContentBuffer, 0, newFileContentBufferLength);
                }

                long nextOffset = contentOffset - sourceStream.Position % contentOffset;
                if (nextOffset == contentOffset)
                {
                    nextOffset = 0;
                }

                sourceStream.Seek(nextOffset, SeekOrigin.Current);
            }
        }
    }
}