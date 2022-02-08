// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Classic.Developer.Reports;
using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable UnusedVariable

namespace Runtime.Classic.Developer.Reports
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="ResourcesAudit"/> class.
    /// </summary>
    public class ResourcesAuditTests
    {
        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Get_MockData_ReturnsObject()
        {
            ResourcesAudit state = ResourcesAudit.Get(new []
            {
                new ResourcesAudit.ResourcesQuery( "UnityEngine.Texture2D,UnityEngine"),
                new ResourcesAudit.ResourcesQuery(
                    "UnityEngine.Texture3D,UnityEngine",
                    GDX.Classic.Developer.Reports.Objects.TextureObjectInfo.TypeDefinition)
            });

            bool evaluate = state != null && state.KnownObjects.Count == 2;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Output_GetCommon_ReturnsReport()
        {
            string[] report = ResourcesAudit.GetCommon().Output();

            bool evaluate = report != null && report.Length > 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Output_GetAll_ReturnsReport()
        {
            string[] report = ResourcesAudit.GetAll().Output();

            bool evaluate = report != null && report.Length > 0;

            Assert.IsTrue(evaluate);
        }

        // TODO: CopyCount test ? load something twice?
    }
}