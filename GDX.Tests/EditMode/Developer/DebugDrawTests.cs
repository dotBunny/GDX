// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace GDX.Rendering
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="DebugDraw" />
    ///     class.
    /// </summary>
    public class DebugDrawTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void GetDrawCommandBuffer_New_ReturnsInstance()
        {
            DebugLineBuffer buffer = DebugDraw.GetLineBuffer(0);
            Assert.IsNotNull(buffer);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetDrawCommandBuffer_ByID_ReturnsInstance()
        {
            DebugLineBuffer bufferA = DebugDraw.GetLineBuffer(0);
            DebugLineBuffer bufferB = DebugDraw.GetLineBuffer(0);

            Assert.IsTrue(bufferA.Key == bufferB.Key);
            Assert.IsTrue(bufferA == bufferB);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void HasDrawCommandBuffer_ByID_Valid()
        {
            DebugDraw.GetLineBuffer(0);
            Assert.IsTrue(DebugDraw.HasLineBuffer(0));
            DebugDraw.RemoveLineBuffer(0);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveDrawCommandBuffer_ByID_RemovesInstance()
        {
            DebugDraw.GetLineBuffer(0);
            DebugDraw.RemoveLineBuffer(0);
            Assert.IsFalse(DebugDraw.HasLineBuffer(0));
        }
    }
}