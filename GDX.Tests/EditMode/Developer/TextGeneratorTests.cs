﻿// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using NUnit.Framework;

namespace GDX.Developer
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="TextGenerator" /> class.
    /// </summary>
    public class TextGeneratorTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void AppendLineRange_MockData_AddsContent()
        {
            TextGenerator mockGenerator = new TextGenerator();
            string[] mockData = new[] { "Hello", "Jello" };
            mockGenerator.AppendLineRange(mockData);
            string output = mockGenerator.ToString();

            Assert.IsTrue(output == $"Hello{Environment.NewLine}Jello");
        }
    }
}