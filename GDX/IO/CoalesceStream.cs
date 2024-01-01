// Copyright (c) 2020-2024 dotBunny Inc.
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
    ///     Max size being limited to <see cref="m_Length" /> type limitations.
    /// </remarks>
    public class CoalesceStream : Stream
    {
        /// <summary>
        ///     The default size of each allocated bucket array.
        /// </summary>
        const int k_DefaultBucketSize = 65536;

        /// <summary>
        ///     The internal arrays storage of buckets.
        /// </summary>
        readonly List<byte[]> m_Buckets = new List<byte[]>();

        /// <summary>
        ///     The bucket size used to allocate new arrays.
        /// </summary>
        readonly int m_BucketSize;

        /// <summary>
        ///     The perceived length of the data contained within.
        /// </summary>
        long m_Length;

        /// <summary>
        ///     Create a <see cref="CoalesceStream" />.
        /// </summary>
        /// <param name="bucketSize">The bucket allocation size.</param>
        public CoalesceStream(int bucketSize = k_DefaultBucketSize)
        {
            Position = 0;
            m_BucketSize = bucketSize;
        }

        /// <summary>
        ///     Create a <see cref="CoalesceStream" /> and fill it with the data found in <paramref name="source" />.
        /// </summary>
        /// <param name="source">An array used to prefill the <see cref="CoalesceStream" /> with.</param>
        /// <param name="bucketSize">The bucket allocation size.</param>
        public CoalesceStream(byte[] source, int bucketSize = k_DefaultBucketSize)
        {
            m_BucketSize = bucketSize;
            Write(source, 0, source.Length);
            Position = 0;
        }

        /// <summary>
        ///     Preallocate a <see cref="CoalesceStream" /> at the desired <paramref name="length" />.
        /// </summary>
        /// <param name="length">The desired pre-allocated size.</param>
        /// <param name="bucketSize">The bucket allocation size.</param>
        public CoalesceStream(int length, int bucketSize = k_DefaultBucketSize)
        {
            m_BucketSize = bucketSize;
            SetLength(length);
            Position = length;
            while (m_Buckets.Count <= BucketIndex)
            {
                m_Buckets.Add(new byte[m_BucketSize]);
            }

            Position = 0;
        }

        /// <summary>
        ///     Preallocate a <see cref="CoalesceStream" /> at the desired <paramref name="length" />.
        /// </summary>
        /// <param name="length">The desired pre-allocated size.</param>
        /// <param name="bucketSize">The bucket allocation size.</param>
        public CoalesceStream(long length, int bucketSize = k_DefaultBucketSize)
        {
            m_BucketSize = bucketSize;
            SetLength(length);
            Position = length;
            while (m_Buckets.Count <= BucketIndex)
            {
                m_Buckets.Add(new byte[m_BucketSize]);
            }

            Position = 0;
        }

        /// <summary>
        ///     Is this <see cref="Stream" /> capable of reading?
        /// </summary>
        public override bool CanRead => true;

        /// <summary>
        ///     Is this <see cref="Stream" /> capable of seeking?
        /// </summary>
        public override bool CanSeek => true;

        /// <summary>
        ///     Is this <see cref="Stream" /> capable of writing?
        /// </summary>
        public override bool CanWrite => true;

        /// <summary>
        ///     Get the combined length of the internal arrays.
        /// </summary>
        public override long Length => m_Length;

        /// <summary>
        ///     Get the current position in the <see cref="Stream" />.
        /// </summary>
        public sealed override long Position { get; set; }

        /// <summary>
        ///     Get the current bucket.
        /// </summary>
        byte[] Bucket
        {
            get
            {
                while (m_Buckets.Count <= BucketIndex)
                {
                    m_Buckets.Add(new byte[m_BucketSize]);
                }

                return m_Buckets[(int)BucketIndex];
            }
        }

        /// <summary>
        ///     Determine the current bucket index based on the position and bucket size.
        /// </summary>
        long BucketIndex => Position / m_BucketSize;

        /// <summary>
        ///     Determine the current bucket offset based on the position and bucket size.
        /// </summary>
        long BucketOffset => Position % m_BucketSize;


        /// <summary>
        ///     Flush reading and writing buffers.
        /// </summary>
        /// <remarks>
        ///     Does nothing for the <see cref="CoalesceStream" />.
        /// </remarks>
        [ExcludeFromCodeCoverage]
        public override void Flush()
        {
        }

        /// <summary>
        ///     Read from the <see cref="CoalesceStream" /> into a buffer.
        /// </summary>
        /// <param name="buffer">The target buffer to write the read data into.</param>
        /// <param name="offset">The offset position to start writing into the <paramref name="buffer" />.</param>
        /// <param name="count">The number of <see cref="byte" />s to read.</param>
        /// <returns>The number of bytes read.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when a negative amounts of bytes are requested, or a negative offset is provided.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when the provided <paramref name="buffer" /> is null.
        /// </exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            long readCount = count;

            if (readCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), readCount,
                    "Number of bytes to copy cannot be negative.");
            }

            long remaining = m_Length - Position;
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
                long copySize = Math.Min(readCount, m_BucketSize - BucketOffset);
                Buffer.BlockCopy(Bucket, (int)BucketOffset, buffer, offset, (int)copySize);
                readCount -= copySize;
                offset += (int)copySize;

                read += (int)copySize;
                Position += copySize;
            } while (readCount > 0);

            return read;
        }

        /// <summary>
        ///     Seek the internal position to a new location.
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
        ///     Arbitrarily set the internal length of the <see cref="CoalesceStream" />
        /// </summary>
        /// <param name="value">The new length value.</param>
        public sealed override void SetLength(long value)
        {
            m_Length = value;
        }

        /// <summary>
        ///     Write into the <see cref="CoalesceStream" /> at the current position.
        /// </summary>
        /// <param name="buffer">The source array to read data from</param>
        /// <param name="offset">An offset of where to start in the <paramref name="buffer" />.</param>
        /// <param name="count">The number of bytes to read from the <paramref name="buffer" />.</param>
        public sealed override void Write(byte[] buffer, int offset, int count)
        {
            long initialPosition = Position;
            try
            {
                do
                {
                    int copySize = Math.Min(count, (int)(m_BucketSize - BucketOffset));
                    long intendedLength = Position + copySize;
                    if (intendedLength > m_Length)
                    {
                        m_Length = intendedLength;
                    }

                    Buffer.BlockCopy(buffer, offset, Bucket, (int)BucketOffset, copySize);
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
        ///     Read a singular <see cref="byte" /> from the current position, incrementing the position.
        /// </summary>
        /// <returns>A valid byte as an int, or -1.</returns>
        public override int ReadByte()
        {
            if (Position >= m_Length)
            {
                return -1;
            }

            byte b = Bucket[BucketOffset];
            Position++;

            return b;
        }

        /// <summary>
        ///     Write a singular <see cref="byte" /> to the <see cref="CoalesceStream" />, incrementing the position.
        /// </summary>
        /// <param name="value">The <see cref="byte" /> to write to the <see cref="CoalesceStream" />.</param>
        public override void WriteByte(byte value)
        {
            long intendedLength = Position + 1;
            if (intendedLength > m_Length)
            {
                m_Length = intendedLength;
            }

            Bucket[BucketOffset] = value;
            Position++;
        }
    }
}