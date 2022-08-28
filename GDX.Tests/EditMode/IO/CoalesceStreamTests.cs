// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Mathematics.Random;
using NUnit.Framework;

namespace GDX.IO
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="CoalesceStream"/>.
    /// </summary>
    public class CoalesceStreamTests
    {
        WELL1024a m_Well;

        [SetUp]
        public void Setup()
        {
            m_Well = new WELL1024a("coalesce");
        }

        [TearDown]
        public void Teardown()
        {
            m_Well.Dispose();
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_Empty()
        {
            CoalesceStream empty = new CoalesceStream();
            Assert.IsTrue(empty != null);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_Bytes()
        {
            byte[] testArray = new byte[5];
            m_Well.NextBytes(testArray);
            CoalesceStream bytes = new CoalesceStream(testArray);

            Assert.IsTrue(bytes.Position == 0);
            Assert.IsTrue(bytes != null);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Read_()
        {
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ReadByte_MockData_Valid()
        {
            byte[] testArray = new byte[5];
            m_Well.NextBytes(testArray);
            CoalesceStream bytes = new CoalesceStream(testArray);
            Assert.IsTrue(bytes.ReadByte() == 126);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ReadByte_Empty_NegativeOne()
        {
            CoalesceStream stream = new CoalesceStream();
            Assert.IsTrue(stream.ReadByte() == -1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Seek_()
        {
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SetLength_()
        {
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Write_()
        {
        }

        [Test]
        [Category(Core.TestCategory)]
        public void WriteByte_()
        {
        }
    }
}