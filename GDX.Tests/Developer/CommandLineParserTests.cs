// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation, UnusedVariable

namespace GDX.Developer
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="CommandLineParser" /> class.
    /// </summary>
    public class CommandLineParserTests
    {
        string[] m_MockData;

        [SetUp]
        public void Setup()
        {
            m_MockData = new[]
            {
                $"{Core.Config.DeveloperCommandLineParserArgumentPrefix}KEY{Core.Config.DeveloperCommandLineParserArgumentSplit}Value",
                "job", $"{Core.Config.DeveloperCommandLineParserArgumentPrefix}SOMETHING"
            };
        }

        [TearDown]
        public void TearDown()
        {
            m_MockData = null;
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ProcessArguments_MockData_ContainsFlag()
        {
            CommandLineParser.ProcessArguments(m_MockData);

            bool evaluate = CommandLineParser.Flags.Contains("SOMETHING");

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ProcessArguments_MockData_HasValue()
        {
            CommandLineParser.ProcessArguments(m_MockData);

            bool evaluate = CommandLineParser.Arguments.ContainsKey("KEY") &&
                            CommandLineParser.Arguments["KEY"] == "Value";

            Assert.IsTrue(evaluate);
        }
    }
}