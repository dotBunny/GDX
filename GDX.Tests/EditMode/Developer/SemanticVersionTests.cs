// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace GDX.Developer
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="CommandLineParser" /> class.
    /// </summary>
    public class SemanticVersionTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void SemanticVersion_NoVersion_Origin()
        {
            SemanticVersion version = new SemanticVersion();
            Assert.IsTrue(version.Major == 0);
            Assert.IsTrue(version.Minor == 0);
            Assert.IsTrue(version.Patch == 0);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SemanticVersion_FromString_FourPartsParse()
        {
            SemanticVersion version = new SemanticVersion("2020.3.2.1");
            Assert.IsTrue(version.Major == 2020);
            Assert.IsTrue(version.Minor == 3);
            Assert.IsTrue(version.Patch == 2);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SemanticVersion_FromString_ThreePartsParse()
        {
            SemanticVersion version = new SemanticVersion("2020.3.2.1");
            Assert.IsTrue(version.Major == 2020);
            Assert.IsTrue(version.Minor == 3);
            Assert.IsTrue(version.Patch == 2);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SemanticVersion_FromString_TwoPartsParse()
        {
            SemanticVersion version = new SemanticVersion("2020.3");
            Assert.IsTrue(version.Major == 2020);
            Assert.IsTrue(version.Minor == 3);
            Assert.IsTrue(version.Patch == 0);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SemanticVersion_FromString_OnePartParse()
        {
            SemanticVersion version = new SemanticVersion("2020");
            Assert.IsTrue(version.Major == 2020);
            Assert.IsTrue(version.Minor == 0);
            Assert.IsTrue(version.Patch == 0);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SemanticVersion_UnityVersion_ThreePartsParse()
        {
            SemanticVersion version = new SemanticVersion("2020.3.1f1");
            Assert.IsTrue(version.Major == 2020);
            Assert.IsTrue(version.Minor == 3);
            Assert.IsTrue(version.Patch == 1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SemanticVersion_FromString_TooManyParts()
        {
            SemanticVersion version = new SemanticVersion("2020.1.1.1.1.1.1.1");
            Assert.IsTrue(version.Major == 0);
            Assert.IsTrue(version.Minor == 0);
            Assert.IsTrue(version.Patch == 0);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Equality_MockData_Matches()
        {
            SemanticVersion versionA = new SemanticVersion("2020.3.1");
            SemanticVersion versionB = new SemanticVersion("2020.3.1f1");
            Assert.IsTrue(versionA == versionB);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Equals_MockData_Matches()
        {
            SemanticVersion versionA = new SemanticVersion("2020.3.1");
            SemanticVersion versionB = new SemanticVersion("2020.3.1f1");
            Assert.IsTrue(versionA.Equals(versionB));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetHashCode_MockData_Valid()
        {
            SemanticVersion version = new SemanticVersion("2020.3.1");
            int hashcode = version.GetHashCode();
            Assert.IsTrue(hashcode == 318371370);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GreaterThanOrEqualTo_MockData_Evaluate()
        {
            SemanticVersion majorA = new SemanticVersion("2022.0.0");
            SemanticVersion majorB = new SemanticVersion("2022.0.0");
            Assert.IsTrue(majorA >= majorB);

            SemanticVersion majorC = new SemanticVersion("2023.0.0");
            SemanticVersion majorD = new SemanticVersion("2022.0.0");
            Assert.IsTrue(majorC >= majorD);

            SemanticVersion minorA = new SemanticVersion("2022.1.0");
            SemanticVersion minorB = new SemanticVersion("2022.1.0");
            Assert.IsTrue(minorA >= minorB);

            SemanticVersion minorC = new SemanticVersion("2022.2.0");
            SemanticVersion minorD = new SemanticVersion("2022.1.0");
            Assert.IsTrue(minorC >= minorD);

            SemanticVersion patchA = new SemanticVersion("2022.0.1");
            SemanticVersion patchB = new SemanticVersion("2022.0.1");
            Assert.IsTrue(patchA >= patchB);

            SemanticVersion patchC = new SemanticVersion("2022.0.2");
            SemanticVersion patchD = new SemanticVersion("2022.0.1");
            Assert.IsTrue(patchC >= patchD);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GreaterThan_MockData_Evaluate()
        {
            SemanticVersion majorA = new SemanticVersion("2022.0.0");
            SemanticVersion majorB = new SemanticVersion("2021.0.0");
            Assert.IsTrue(majorA > majorB);

            SemanticVersion minorA = new SemanticVersion("2022.1.0");
            SemanticVersion minorB = new SemanticVersion("2022.0.0");
            Assert.IsTrue(minorA > minorB);

            SemanticVersion patchA = new SemanticVersion("2022.0.1");
            SemanticVersion patchB = new SemanticVersion("2022.0.0");
            Assert.IsTrue(patchA > patchB);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Inequality_MockData_DoesNotMatch()
        {
            SemanticVersion versionA = new SemanticVersion("2020.3.1");
            SemanticVersion versionB = new SemanticVersion("2020.3.2f1");
            Assert.IsTrue(versionA != versionB);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void LessThanOrEqualTo_MockData_Evaluate()
        {
            SemanticVersion majorA = new SemanticVersion("2022.0.0");
            SemanticVersion majorB = new SemanticVersion("2022.0.0");
            Assert.IsTrue(majorA <= majorB);

            SemanticVersion majorC = new SemanticVersion("2021.0.0");
            SemanticVersion majorD = new SemanticVersion("2022.0.0");
            Assert.IsTrue(majorC <= majorD);

            SemanticVersion minorA = new SemanticVersion("2022.1.0");
            SemanticVersion minorB = new SemanticVersion("2022.1.0");
            Assert.IsTrue(minorA <= minorB);

            SemanticVersion minorC = new SemanticVersion("2022.0.0");
            SemanticVersion minorD = new SemanticVersion("2022.1.0");
            Assert.IsTrue(minorC <= minorD);

            SemanticVersion patchA = new SemanticVersion("2022.0.1");
            SemanticVersion patchB = new SemanticVersion("2022.0.1");
            Assert.IsTrue(patchA <= patchB);

            SemanticVersion patchC = new SemanticVersion("2022.0.0");
            SemanticVersion patchD = new SemanticVersion("2022.0.1");
            Assert.IsTrue(patchC <= patchD);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void LessThan_MockData_Evaluate()
        {
            SemanticVersion majorA = new SemanticVersion("2021.0.0");
            SemanticVersion majorB = new SemanticVersion("2022.0.0");
            Assert.IsTrue(majorA < majorB);

            SemanticVersion minorA = new SemanticVersion("2022.0.0");
            SemanticVersion minorB = new SemanticVersion("2022.1.0");
            Assert.IsTrue(minorA < minorB);

            SemanticVersion patchA = new SemanticVersion("2022.0.0");
            SemanticVersion patchB = new SemanticVersion("2022.0.1");
            Assert.IsTrue(patchA < patchB);
        }
    }
}