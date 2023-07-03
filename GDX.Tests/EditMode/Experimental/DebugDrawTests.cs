// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace GDX.Experimental
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="DebugDraw" />
    ///     class.
    /// </summary>
    public class DebugDrawTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void GetUnmanagedBuffer_New_ReturnsInstance()
        {
            DebugDrawBuffer buffer = DebugDraw.GetUnmanagedBuffer(0);
            Assert.IsNotNull(buffer);
            DebugDraw.RemoveUnmanagedBuffer(0);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetUnmanagedBuffer_ByID_ReturnsInstance()
        {
            DebugDrawBuffer bufferA = DebugDraw.GetUnmanagedBuffer(0);
            DebugDrawBuffer bufferB = DebugDraw.GetUnmanagedBuffer(0);

            Assert.IsTrue(bufferA.Key == bufferB.Key);
            Assert.IsTrue(bufferA == bufferB);

            DebugDraw.RemoveUnmanagedBuffer(bufferA.Key);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void HasUnmanagedBuffer_ByID_Valid()
        {
            DebugDraw.GetUnmanagedBuffer(0);
            Assert.IsTrue(DebugDraw.HasUnmanagedBuffer(0));
            DebugDraw.RemoveUnmanagedBuffer(0);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUnmanagedBuffer_ByID_RemovesInstance()
        {
            DebugDraw.GetUnmanagedBuffer(0);
            DebugDraw.RemoveUnmanagedBuffer(0);
            Assert.IsFalse(DebugDraw.HasUnmanagedBuffer(0));
        }
    }
}