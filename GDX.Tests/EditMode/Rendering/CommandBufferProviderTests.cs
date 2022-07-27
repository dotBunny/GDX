// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace GDX.Rendering
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="CommandBufferProvider" />
    ///     class.
    /// </summary>
    public class CommandBufferProviderTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void GetDrawCommandBuffer_New_ReturnsInstance()
        {
            DrawCommandBuffer buffer = CommandBufferProvider.GetDrawCommandBuffer(0);
            Assert.IsNotNull(buffer);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetDrawCommandBuffer_ByID_ReturnsInstance()
        {
            DrawCommandBuffer bufferA = CommandBufferProvider.GetDrawCommandBuffer(0);
            DrawCommandBuffer bufferB = CommandBufferProvider.GetDrawCommandBuffer(0);

            Assert.IsTrue(bufferA.Key == bufferB.Key);
            Assert.IsTrue(bufferA == bufferB);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void HasDrawCommandBuffer_ByID_Valid()
        {
            CommandBufferProvider.GetDrawCommandBuffer(0);
            Assert.IsTrue(CommandBufferProvider.HasDrawCommandBuffer(0));
            CommandBufferProvider.RemoveDrawCommandBuffer(0);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveDrawCommandBuffer_ByID_RemovesInstance()
        {
            CommandBufferProvider.GetDrawCommandBuffer(0);
            CommandBufferProvider.RemoveDrawCommandBuffer(0);
            Assert.IsFalse(CommandBufferProvider.HasDrawCommandBuffer(0));
        }
    }
}