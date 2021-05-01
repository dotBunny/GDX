// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Developer;
using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable UnusedVariable

namespace Runtime.Developer
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="MemoryReport" /> class.
    /// </summary>
    public class ResourcesReportTests
    {
        [Test]
        [Category("GDX.Tests")]
        public void ResourceSnapshot_NoData_CreatesState()
        {
            var state = ResourcesReport.GeneralReport();

            bool evaluate = state != null;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void ManagedHeapSnapshotString_NoData_CreatesState()
        {

            var state = ResourcesReport.Generate(new ResourcesQuery[]
            {
                new ResourcesQuery( "UnityEngine.Texture2D,UnityEngine"),
                new ResourcesQuery(
                    "UnityEngine.Texture3D,UnityEngine",
                    GDX.Developer.ObjectInfos.TextureObjectInfo.TypeDefinition)
            });

            bool evaluate = state != null;

            Assert.IsTrue(evaluate);
        }
    }
}