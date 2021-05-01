// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Developer.Reports;
using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable UnusedVariable

namespace Runtime.Developer.Reports
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="MemoryDump" /> class.
    /// </summary>
    public class MemoryDumpTests
    {
        [Test]
        [Category("GDX.Tests")]
        public void ManagedHeapSnapshot_NoData_CreatesState()
        {
            var state = MemoryDump.ManagedHeapSnapshot();

            bool evaluate = state != null;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void ManagedHeapSnapshotString_NoData_CreatesState()
        {

            var state = MemoryDump.ManagedHeapSnapshot(new MemoryDump.HeapQuery[]
            {
                new MemoryDump.HeapQuery("Texture2D", "UnityEngine.Texture2D,UnityEngine"),

                new MemoryDump.HeapQuery(
                    "Texture3D",
                    "UnityEngine.Texture3D,UnityEngine",
                    TextureObjectInfo.TypeDefinition)
            });

            bool evaluate = state != null;

            Assert.IsTrue(evaluate);
        }
    }
}