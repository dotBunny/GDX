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
    ///     A collection of unit tests to validate functionality of the <see cref="ResourcesReport"/> class.
    /// </summary>
    public class ResourcesReportTests
    {
        [Test]
        [Category("GDX.Tests")]
        public void Get_MockData_ReturnsObject()
        {

            var state = ResourcesReport.Get(new []
            {
                new ResourcesQuery( "UnityEngine.Texture2D,UnityEngine"),
                new ResourcesQuery(
                    "UnityEngine.Texture3D,UnityEngine",
                    GDX.Developer.ObjectInfos.TextureObjectInfo.TypeDefinition)
            });


            System.IO.File.WriteAllLines(
                System.IO.Path.Combine(UnityEngine.Application.dataPath, "test.log"), state.Output());

            bool evaluate = state != null && state.KnownObjects.Count == 2;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Output_GetCommon_ReturnsReport()
        {
            var report = ResourcesReport.GetCommon().Output();

            bool evaluate = report != null && report.Length > 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Output_GetAll_ReturnsReport()
        {
            var report = ResourcesReport.GetAll().Output();

            System.IO.File.WriteAllLines(
                System.IO.Path.Combine(UnityEngine.Application.dataPath, "test.log"), report);

            bool evaluate = report != null && report.Length > 0;

            Assert.IsTrue(evaluate);
        }
    }
}