// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX;
using GDX.Developer;
using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable UnusedVariable

namespace Runtime.Developer
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="CommandLineParser" /> class.
    /// </summary>
    public class CommandLineParserTests
    {
        private string[] _mockData;

        [SetUp]
        public void Setup()
        {
            _mockData = new[]
            {
                $"{Core.Config.developerCommandLineParserArgumentPrefix}KEY{Core.Config.developerCommandLineParserArgumentSplit}Value",
                "job", $"{Core.Config.developerCommandLineParserArgumentPrefix}SOMETHING"
            };
        }

        [TearDown]
        public void TearDown()
        {
            _mockData = null;
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ProcessArguments_MockData_ContainsFlag()
        {
            CommandLineParser.ProcessArguments(_mockData);

            bool evaluate = CommandLineParser.Flags.Contains("SOMETHING");

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ProcessArguments_MockData_HasValue()
        {
            CommandLineParser.ProcessArguments(_mockData);

            bool evaluate = CommandLineParser.Arguments.ContainsKey("KEY") &&
                            CommandLineParser.Arguments["KEY"] == "Value";

            Assert.IsTrue(evaluate);
        }
    }
}