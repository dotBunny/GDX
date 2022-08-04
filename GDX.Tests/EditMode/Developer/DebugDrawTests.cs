// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace GDX.Developer
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="DebugDraw" />
    ///     class.
    /// </summary>
    public class DebugDrawTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void GetBuffer_New_ReturnsInstance()
        {
            DebugDrawBuffer buffer = DebugDraw.GetBuffer(0);
            Assert.IsNotNull(buffer);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetBuffer_ByID_ReturnsInstance()
        {
            DebugDrawBuffer bufferA = DebugDraw.GetBuffer(0);
            DebugDrawBuffer bufferB = DebugDraw.GetBuffer(0);

            Assert.IsTrue(bufferA.Key == bufferB.Key);
            Assert.IsTrue(bufferA == bufferB);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void HasBuffer_ByID_Valid()
        {
            DebugDraw.GetBuffer(0);
            Assert.IsTrue(DebugDraw.HasBuffer(0));
            DebugDraw.RemoveBuffer(0);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveBuffer_ByID_RemovesInstance()
        {
            DebugDraw.GetBuffer(0);
            DebugDraw.RemoveBuffer(0);
            Assert.IsFalse(DebugDraw.HasBuffer(0));
        }
    }
}