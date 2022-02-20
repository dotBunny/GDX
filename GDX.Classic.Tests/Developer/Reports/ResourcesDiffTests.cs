using GDX.Classic.Developer.Reports;
using NUnit.Framework;

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
        public void Output_GetCommonAndAll_ReturnsReport()
        {
            ResourcesAudit lhs = ResourcesAudit.GetCommon();
            ResourcesAudit rhs = ResourcesAudit.GetAll();
            ResourcesDiff diff = new ResourcesDiff(lhs, rhs);
            string[] report = diff.Output();

            bool evaluate = report != null && report.Length > 0;

            Assert.IsTrue(evaluate);
        }
    }
}