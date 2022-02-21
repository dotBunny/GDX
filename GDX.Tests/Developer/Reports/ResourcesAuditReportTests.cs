// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Developer.Reports;
using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable UnusedVariable

namespace GDX.Classic.Developer.Reports
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="ResourcesAuditReport"/> class.
    /// </summary>
    public class ResourcesAuditReportTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void Get_MockData_ReturnsObject()
        {
            ResourcesAuditReport state = ResourcesAuditReport.Get(new []
            {
                new ResourcesAuditReport.ResourcesQuery( "UnityEngine.Texture2D,UnityEngine"),
                new ResourcesAuditReport.ResourcesQuery(
                    "UnityEngine.Texture3D,UnityEngine",
                    GDX.Developer.Reports.Resource.Objects.TextureObjectInfo.TypeDefinition)
            });

            bool evaluate = state != null && state.KnownObjects.Count == 2;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Output_GetCommon_ReturnsReport()
        {
            string[] report = ResourcesAuditReport.GetCommon().Output();

            bool evaluate = report != null && report.Length > 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Output_GetAll_ReturnsReport()
        {
            string[] report = ResourcesAuditReport.GetAll().Output();

            bool evaluate = report != null && report.Length > 0;

            Assert.IsTrue(evaluate);
        }

        // TODO: CopyCount test ? load something twice?
    }
}