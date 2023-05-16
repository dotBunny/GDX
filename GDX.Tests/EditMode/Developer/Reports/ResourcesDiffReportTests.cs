// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace GDX.Developer.Reports
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="ResourcesDiffReport"/> class.
    /// </summary>
    public class ResourcesDiffReportTests
    {
        [Test]
        [Category(Core.TestCategory)]
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