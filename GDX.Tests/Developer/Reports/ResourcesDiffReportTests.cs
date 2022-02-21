using GDX.Developer.Reports;
using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable UnusedVariable

namespace Runtime.Developer.Reports
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="ResourcesDiffReport"/> class.
    /// </summary>
    public class ResourcesDiffReportTests
    {
        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Output_GetCommonAndAll_ReturnsReport()
        {
            ResourcesAuditReport lhs = ResourcesAuditReport.GetCommon();
            ResourcesAuditReport rhs = ResourcesAuditReport.GetAll();
            ResourcesDiffReport diffReport = new ResourcesDiffReport(lhs, rhs);
            string[] report = diffReport.Output();

            bool evaluate = report != null && report.Length > 0;

            Assert.IsTrue(evaluate);
        }
    }
}