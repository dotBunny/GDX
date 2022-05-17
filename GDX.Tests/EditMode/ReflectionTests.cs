// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;
using UnityEditor;

namespace GDX
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="Reflection" />
    ///     class.
    /// </summary>
    public class ReflectionTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void GetDefault_Nullable_ReturnsNull()
        {
            Assert.IsTrue(typeof(EditorWindow).GetDefault() == null, "A null value was expected.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetDefault_NonNullable_ReturnsValue()
        {
            object defaultBoxedValue = typeof(int).GetDefault();
            int defaultValue = (int)defaultBoxedValue;
            Assert.IsTrue(defaultValue == 0,  "A non-null value was expected.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetFieldValue_ProjectBrowser_ReturnsSearchFieldText()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser.");

            string searchContext = Reflection.GetFieldValue<string>(
                projectBrowserWindow, "UnityEditor.ProjectBrowser", "m_SearchFieldText");

            Assert.IsTrue(searchContext != null, "Expected non-null value for m_SearchFieldText.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetFieldValue_NoType_ReturnsDefault()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser.");

            string searchContext = Reflection.GetFieldValue<string>(
                projectBrowserWindow, "UnityEditor.ProjectBrowserBad", "m_SearchFieldTextBad");

            Assert.IsTrue(searchContext == null, "Expected default value for non existing type.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetFieldOrPropertyValue_NoTarget_ReturnsNull()
        {
            object missedObject = Reflection.GetFieldOrPropertyValue(
                null, "UnityEditor.ProjectBrowser");

            Assert.IsTrue(missedObject == null, "Expected null value when no target.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetFieldOrPropertyValue_ProjectBrowserField_ReturnsStartGridSize()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser");

            object gridSize = Reflection.GetFieldOrPropertyValue(
                projectBrowserWindow, "m_StartGridSize");

            Assert.IsTrue(gridSize != null, "Expected non-null value for m_StartGridSize.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetFieldOrPropertyValue_ProjectBrowserProperty_ReturnsIsLocked()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser.");

            object toggle = Reflection.GetFieldOrPropertyValue(
                projectBrowserWindow, "isLocked");

            Assert.IsTrue(toggle != null, "Expected non-null value for isLocked.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetType_QualifiedType_ReturnsType()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetType_BadType_ReturnsNull()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowserBad");
            Assert.IsTrue(projectBrowser == null, "Expected null type for bad type.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void InvokeStaticMethod_ProjectBrowser_ReturnsListOfBrowsers()
        {
            object response = Reflection.InvokeStaticMethod("UnityEditor.ProjectBrowser", "GetAllProjectBrowsers");
            Assert.IsTrue(response != null);
        }

        [Test]
        [Category(Core.TestCategory)]
        [Category(Core.TestCategory)]
        public void InvokeStaticMethod_BadMethod_ReturnsNull()
        {
            object response = Reflection.InvokeStaticMethod("UnityEditor.ProjectBrowser", "GetAllProjectBrowsersBad");
            Assert.IsTrue(response == null);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void InvokeStaticMethod_BadType_ReturnsNull()
        {
            object response = Reflection.InvokeStaticMethod("UnityEditor.ProjectBrowserBad", "GetAllProjectBrowsersBad");
            Assert.IsTrue(response == null);
        }
    }
}