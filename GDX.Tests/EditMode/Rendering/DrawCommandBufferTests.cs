// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace GDX.Rendering
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="DrawCommandBuffer" />
    ///     class.
    /// </summary>
    public class DrawCommandBufferTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void GetInstance_New_ReturnsInstance()
        {
            DrawCommandBuffer buffer = DrawCommandBuffer.GetInstance(0);
            Assert.IsNotNull(buffer);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetInstance_ByID_ReturnsInstance()
        {
            DrawCommandBuffer bufferA = DrawCommandBuffer.GetInstance(0);
            DrawCommandBuffer bufferB = DrawCommandBuffer.GetInstance(0);

            Assert.IsTrue(bufferA.Key == bufferB.Key);
            Assert.IsTrue(bufferA == bufferB);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void HasInstance_ByID_Valid()
        {
            DrawCommandBuffer dcb = DrawCommandBuffer.GetInstance(0);
            Assert.IsTrue(DrawCommandBuffer.HasInstance(0));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveInstance_ByID_RemovesInstance()
        {
            DrawCommandBuffer.GetInstance(0);
            DrawCommandBuffer.RemoveInstance(0);
            Assert.IsFalse(DrawCommandBuffer.HasInstance(0));
        }
    }
}