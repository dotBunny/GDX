using GDX.Classic.Developer.Reports;
using GDX.Classic.Developer.Reports.Objects;
using NUnit.Framework;
using UnityEngine;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable UnusedVariable

namespace Runtime.Developer.Reports
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="ResourcesDiff"/> class.
    /// </summary>
    public class ResourcesDiffTests
    {
        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Constructor_MockData_ReturnsObject()
        {
            var lhs = ResourcesAudit.GetCommon();
            var rhs = ResourcesAudit.GetAll();
            var diff = new ResourcesDiff(lhs, rhs);

            bool evaluate = diff != null;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Output_GetCommonAndAll_ReturnsReport()
        {
            var lhs = ResourcesAudit.GetCommon();
            var rhs = ResourcesAudit.GetAll();
            var diff = new ResourcesDiff(lhs, rhs);
            string[] report = diff.Output();

            bool evaluate = report != null && report.Length > 0;

            Assert.IsTrue(evaluate);
        }
    }
}