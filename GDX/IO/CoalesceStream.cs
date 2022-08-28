// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace GDX.IO
{
    /// <summary>
    ///     A byte backed stream which combines multiple arrays acting as one uniform stream.
    /// </summary>
    /// <remarks>
    ///     Max size being limited to <see cref="m_LengthInternal"/> type limitations.
    /// </remarks>
    public class CoalesceStream : Stream
    {
        /// <summary>
        ///     The size of each allocated block array.
        /// </summary>
        const long k_BlockSize = 65536;

        /// <summary>
        ///     The internal arrays storage of blocks.
        /// </summary>
        readonly List<byte[]> m_Blocks = new List<byte[]>();

        /// <summary>
        ///     The perceived length of the data contained within.
        /// </summary>
        long m_LengthInternal;

        /// <summary>
        ///     Create a <see cref="CoalesceStream"/>.
        /// </summary>
        public CoalesceStream()
        {
            Position = 0;
        }

        /// <summary>
        ///     Create a <see cref="CoalesceStream"/> and fill it with the data found in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">An array used to prefill the <see cref="CoalesceStream"/> with.</param>
        public CoalesceStream(byte[] source)
        {
            Write(source, 0, source.Length);
            Position = 0;
        }

        /// <summary>
        ///     Preallocate a <see cref="CoalesceStream"/> at the desired <paramref name="length"/>.
        /// </summary>
        /// <param name="length">The desired pre-allocated size.</param>
        public CoalesceStream(int length)
        {
            SetLength(length);
            Position = length;
            while (m_Blocks.Count <= BlockIndex)
            {
                m_Blocks.Add(new byte[k_BlockSize]);
            }
            Position = 0;
        }

        /// <summary>
        ///     Preallocate a <see cref="CoalesceStream"/> at the desired <paramref name="length"/>.
        /// </summary>
        /// <param name="length">The desired pre-allocated size.</param>
        public CoalesceStream(long length)
        {
            SetLength(length);
            Position = length;
            while (m_Blocks.Count <= BlockIndex)
            {
                m_Blocks.Add(new byte[k_BlockSize]);
            }
            Position = 0;
        }

        /// <summary>
        ///     Is this <see cref="Stream"/> capable of reading?
        /// </summary>
        public override bool CanRead => true;

        /// <summary>
        ///     Is this <see cref="Stream"/> capable of seeking?
        /// </summary>
        public override bool CanSeek => true;
        /// <summary>
        ///     Is this <see cref="Stream"/> capable of writing?
        /// </summary>
        public override bool CanWrite => true;

        /// <summary>
        ///     Get the combined length of the internal arrays.
        /// </summary>
        public override long Length => m_LengthInternal;

        /// <summary>
        ///     Get the current position in the <see cref="Stream"/>.
        /// </summary>
        public sealed override long Position { get; set; }

        /// <summary>
        ///     Get the current block.
        /// </summary>
        byte[] Block
        {
            get
            {
                while (m_Blocks.Count <= BlockIndex)
                {
                    m_Blocks.Add(new byte[k_BlockSize]);
                }

                return m_Blocks[(int)BlockIndex];
            }
        }

        /// <summary>
        ///     Determine the current block index based on the position and block size.
        /// </summary>
        long BlockIndex => Position / k_BlockSize;

        /// <summary>
        ///     Determine the current block offset based on the position and block size.
        /// </summary>
        long BlockOffset => Position % k_BlockSize;


        /// <summary>
        ///     Flush reading and writing buffers.
        /// </summary>
        /// <remarks>
        ///     Does nothing for the <see cref="CoalesceStream"/>.
        /// </remarks>
        [ExcludeFromCodeCoverage]
        public override void Flush()
        {
        }

        /// <summary>
        ///     Read from the <see cref="CoalesceStream"/> into a buffer.
        /// </summary>
        /// <param name="buffer">The target buffer to write the read data into.</param>
        /// <param name="offset">The offset position to start writing into the <paramref name="buffer"/>.</param>
        /// <param name="count">The number of <see cref="byte"/>s to read.</param>
        /// <returns>The number of bytes read.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when a negative amounts of bytes are requested, or a negative offset is provided.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when the provided <paramref name="buffer"/> is null.
        /// </exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            long readCount = count;

            if (readCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), readCount,
                    "Number of bytes to copy cannot be negative.");
            }

            long remaining = m_LengthInternal - Position;
            if (readCount > remaining)
            {
                readCount = remaining;
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), "Buffer cannot be null.");
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), offset, "Destination offset cannot be negative.");
            }

            int read = 0;
            do
            {
                long copySize = Math.Min(readCount, k_BlockSize - BlockOffset);
                Buffer.BlockCopy(Block, (int)BlockOffset, buffer, offset, (int)copySize);
                readCount -= copySize;
                offset += (int)copySize;

                read += (int)copySize;
                Position += copySize;
            } while (readCount > 0);

            return read;
        }

        /// <summary>
        /// Seek the internal position to a new location.
        /// </summary>
        /// <param name="offset">The value to offset the internal position by.</param>
        /// <param name="origin">The origin of the offset.</param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;

                case SeekOrigin.End:
                    Position = Length - offset;
                    break;
                case SeekOrigin.Current:
                default:
                    Position += offset;
                    break;
            }

            return Position;
        }

        /// <summary>
        ///     Arbitrarily set the internal length of the <see cref="CoalesceStream"/>
        /// </summary>
        /// <param name="value">The new length value.</param>
        public sealed override void SetLength(long value)
        {
            m_LengthInternal = value;
        }

        /// <summary>
        ///     Write into the <see cref="CoalesceStream"/> at the current position.
        /// </summary>
        /// <param name="buffer">The source array to read data from</param>
        /// <param name="offset">An offset of where to start in the <paramref name="buffer"/>.</param>
        /// <param name="count">The number of bytes to read from the <paramref name="buffer"/>.</param>
        public sealed override void Write(byte[] buffer, int offset, int count)
        {
            long initialPosition = Position;
            try
            {
                do
                {
                    int copySize = Math.Min(count, (int)(k_BlockSize - BlockOffset));
                    long intendedLength = Position + copySize;
                    if (intendedLength > m_LengthInternal)
                    {
                        m_LengthInternal = intendedLength;
                    }

                    Buffer.BlockCopy(buffer, offset, Block, (int)BlockOffset, copySize);
                    count -= copySize;
                    offset += copySize;

                    Position += copySize;
                } while (count > 0);
            }
            catch (Exception)
            {
                Position = initialPosition;
                throw;
            }
        }

        /// <summary>
        ///     Read a singular <see cref="byte"/> from the current position, incrementing the position.
        /// </summary>
        /// <returns>A valid byte as an int, or -1.</returns>
        public override int ReadByte()
        {
            if (Position >= m_LengthInternal)
            {
                return -1;
            }

            byte b = Block[BlockOffset];
            Position++;

            return b;
        }

        /// <summary>
        ///     Write a singular <see cref="byte"/> to the <see cref="CoalesceStream"/>, incrementing the position.
        /// </summary>
        /// <param name="value">The <see cref="byte"/> to write to the <see cref="CoalesceStream"/>.</param>
        public override void WriteByte(byte value)
        {
            long intendedLength = Position + 1;
            if (intendedLength > m_LengthInternal)
            {
                m_LengthInternal = intendedLength;
            }
            Block[BlockOffset] = value;
            Position++;
        }
    }
}
