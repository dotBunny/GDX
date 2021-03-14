// dotBunny licenses this file to you under the MIT license.
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
            GDXConfig config = GDXConfig.Get();
            _mockData = new[]
            {
                $"{config.developerCommandLineParserArgumentPrefix}KEY{config.developerCommandLineParserArgumentSplit}Value",
                "job", $"{config.developerCommandLineParserArgumentPrefix}SOMETHING"
            };
        }

        [TearDown]
        public void TearDown()
        {
            _mockData = null;
        }

        [Test]
        [Category("GDX.Tests")]
        public void ParseArguments_MockData_ContainsFlag()
        {

           // Environment.
            CommandLineParser.ParseArguments();

            bool evaluate = CommandLineParser.Flags.Contains("SOMETHING");

            Assert.IsTrue(evaluate);
        }


        [Test]
        [Category("GDX.Tests")]
        public void ProcessArguments_MockData_ContainsFlag()
        {
            CommandLineParser.ProcessArguments(_mockData);

            bool evaluate = CommandLineParser.Flags.Contains("SOMETHING");

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void ProcessArguments_MockData_HasValue()
        {
            CommandLineParser.ProcessArguments(_mockData);

            bool evaluate = CommandLineParser.Arguments.ContainsKey("KEY") &&
                            CommandLineParser.Arguments["KEY"] == "Value";

            Assert.IsTrue(evaluate);
        }
    }
}