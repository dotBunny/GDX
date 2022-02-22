// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;

namespace GDX.IO
{
    // ReSharper disable once UnusedType.Global
    public class CoalesceStream : Stream
    {
        readonly List<byte[]> m_Blocks = new List<byte[]>();
        readonly long m_BlockSize = 65536;
        long m_LengthInternal;

        public CoalesceStream()
        {
            Position = 0;
        }

        public CoalesceStream(byte[] source)
        {
            Write(source, 0, source.Length);
            Position = 0;
        }

        public CoalesceStream(int length)
        {
            SetLength(length);
            Position = length;
            while (m_Blocks.Count <= BlockId)
            {
                m_Blocks.Add(new byte[m_BlockSize]);
            }
            Position = 0;
        }

        public CoalesceStream(long length)
        {
            SetLength(length);
            Position = length;
            while (m_Blocks.Count <= BlockId)
            {
                m_Blocks.Add(new byte[m_BlockSize]);
            }
            Position = 0;
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => true;
        public override long Length => m_LengthInternal;
        public sealed override long Position { get; set; }

        private byte[] Block
        {
            get
            {
                while (m_Blocks.Count <= BlockId)
                {
                    m_Blocks.Add(new byte[m_BlockSize]);
                }

                return m_Blocks[(int)BlockId];
            }
        }

        private long BlockId => Position / m_BlockSize;

        private long BlockOffset => Position % m_BlockSize;


        public override void Flush()
        {
        }

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
                long copySize = Math.Min(readCount, m_BlockSize - BlockOffset);
                Buffer.BlockCopy(Block, (int)BlockOffset, buffer, offset, (int)copySize);
                readCount -= copySize;
                offset += (int)copySize;

                read += (int)copySize;
                Position += copySize;
            } while (readCount > 0);

            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length - offset;
                    break;
            }

            return Position;
        }

        public sealed override void SetLength(long value)
        {
            m_LengthInternal = value;
        }

        public sealed override void Write(byte[] buffer, int offset, int count)
        {
            long initialPosition = Position;
            try
            {
                do
                {
                    int copySize = Math.Min(count, (int)(m_BlockSize - BlockOffset));

                    EnsureCapacity(Position + copySize);

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

        public override void WriteByte(byte value)
        {
            EnsureCapacity(Position + 1);
            Block[BlockOffset] = value;
            Position++;
        }

        private void EnsureCapacity(long intendedLength)
        {
            if (intendedLength > m_LengthInternal)
            {
                m_LengthInternal = intendedLength;
            }
        }
    }
}
