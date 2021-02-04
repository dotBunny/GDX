// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GDX.Developer;
using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable UnusedVariable

namespace Runtime
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="CommandLineParser" /> class.
    /// </summary>
    public class CommandLineParserTests
    {
        /// <summary>
        ///     Check if a simple flag is able to be detected.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_ProcessArguments_Flag()
        {
            string[] testArgs = {"--KEY=Value", "job", "--SOMETHING"};
            CommandLineParser.ProcessArguments(testArgs);
            Assert.IsTrue(CommandLineParser.Flags.Contains("SOMETHING"), "Expected to find SOMETHING flag.");
        }

        /// <summary>
        ///     Check if a KVP is successfully extracted.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_ProcessArguments_Value()
        {
            string[] testArgs = {"--KEY=Value", "job", "--SOMETHING"};
            CommandLineParser.ProcessArguments(testArgs);
            Assert.IsTrue(CommandLineParser.Arguments.ContainsKey("KEY") && CommandLineParser.Arguments["KEY"] == "Value",
                "Expected to find KEY with a value of Value");
        }
    }
}