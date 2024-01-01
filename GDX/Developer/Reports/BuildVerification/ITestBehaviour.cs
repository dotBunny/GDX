// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Developer.Reports.NUnit;

namespace GDX.Developer.Reports.BuildVerification
{
    /// <summary>
    ///     A build verification test behaviour interface.
    /// </summary>
    public interface ITestBehaviour
    {
        TestCase Check();

        /// <summary>
        ///     Get some semblance of identifiable information for a test.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" /> identifier for the test.
        /// </returns>
        string GetIdentifier();

        /// <summary>
        ///     Setup for the test.
        /// </summary>
        void Setup();

        /// <summary>
        ///     Tear down after the test.
        /// </summary>
        void TearDown();
    }
}