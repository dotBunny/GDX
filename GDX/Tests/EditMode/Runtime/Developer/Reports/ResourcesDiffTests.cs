using GDX.Developer.Reports;
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
        [Category("GDX.Tests")]
        public void Constructor_MockData_ReturnsObject()
        {
            var lhs = ResourcesAudit.GetCommon();
            var rhs = ResourcesAudit.GetAll();
            var diff = new ResourcesDiff(lhs, rhs);

            bool evaluate = diff != null;

            Assert.IsTrue(evaluate);
        }
    }
}