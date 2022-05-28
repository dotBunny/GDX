// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
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
        const string k_ProjectBrowserFieldName = "m_SearchFieldText";
        const string k_ProjectBrowserFieldNameAlternative = "m_StartGridSize";
        const string k_ProjectBrowserFullName = "UnityEditor.ProjectBrowser";
        const string k_ProjectBrowserMethodName = "GetAllProjectBrowsers";
        const string k_ProjectBrowserPropertyName = "isLocked";

        [Test]
        [Category(Literals.TestCategory)]
        public void GetDefault_Nullable_ReturnsNull()
        {
            Assert.IsTrue(typeof(EditorWindow).GetDefault() == null);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void GetDefault_NonNullable_ReturnsValue()
        {
            object defaultBoxedValue = typeof(int).GetDefault();
            int defaultValue = (int)defaultBoxedValue;
            Assert.IsTrue(defaultValue == 0);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void GetType_QualifiedType_ReturnsType()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void GetType_BadType_ReturnsNull()
        {
            Type projectBrowser = Reflection.GetType(TestLiterals.BadData);
            Assert.IsTrue(projectBrowser == null);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void InvokeStaticMethod_ProjectBrowser_ReturnsListOfBrowsers()
        {
            object response = Reflection.InvokeStaticMethod(k_ProjectBrowserFullName, k_ProjectBrowserMethodName);
            Assert.IsTrue(response != null);
        }

        [Test]
        [Category(Literals.TestCategory)]
        [Category(Literals.TestCategory)]
        public void InvokeStaticMethod_BadMethod_ReturnsNull()
        {
            object response = Reflection.InvokeStaticMethod(k_ProjectBrowserFullName, TestLiterals.BadData);
            Assert.IsTrue(response == null);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void InvokeStaticMethod_BadType_ReturnsNull()
        {
            object response = Reflection.InvokeStaticMethod(TestLiterals.BadData, TestLiterals.BadData);
            Assert.IsTrue(response == null);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void SetFieldOrPropertyValue_ProjectBrowser_SetIsLocked()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsTrue(Reflection.TryGetFieldOrPropertyValue(projectBrowserWindow, k_ProjectBrowserPropertyName, out object previous));

            Assert.IsTrue(Reflection.SetFieldOrPropertyValue(
                projectBrowserWindow, k_ProjectBrowserPropertyName, !(bool)previous));

            Assert.IsTrue(Reflection.TryGetFieldOrPropertyValue(projectBrowserWindow, k_ProjectBrowserPropertyName, out object newValue));
            Assert.IsTrue((bool)newValue == !(bool)previous);

            Assert.IsTrue(Reflection.SetFieldOrPropertyValue(
                projectBrowserWindow, k_ProjectBrowserPropertyName, previous));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void SetFieldOrPropertyValue_ProjectBrowser_SetSearchFieldText()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsTrue(Reflection.SetFieldOrPropertyValue(
                projectBrowserWindow, k_ProjectBrowserFieldName, TestLiterals.Foo));

            Reflection.TryGetFieldValue(
                projectBrowserWindow, projectBrowser, k_ProjectBrowserFieldName, out string tempValue);
            Assert.IsTrue(tempValue == TestLiterals.Foo);

            Assert.IsTrue(Reflection.SetFieldOrPropertyValue(
                projectBrowserWindow, k_ProjectBrowserFieldName, string.Empty));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void SetFieldOrPropertyValue_NoType_ReturnsFalse()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(Reflection.SetFieldOrPropertyValue(projectBrowserWindow, TestLiterals.BadData, TestLiterals.Foo));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void SetFieldOrPropertyValue_NoTarget_ReturnsFalse()
        {
            Assert.IsFalse(Reflection.SetFieldOrPropertyValue(null, TestLiterals.BadData, TestLiterals.Foo));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void SetFieldValue_ProjectBrowser_SetSearchFieldText()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsTrue(Reflection.SetFieldValue(
                projectBrowserWindow, projectBrowser, k_ProjectBrowserFieldName, TestLiterals.Foo));

            Assert.IsTrue(Reflection.TryGetFieldValue(
                projectBrowserWindow, projectBrowser, k_ProjectBrowserFieldName, out string query));
            Assert.IsTrue(query == TestLiterals.Foo);

            Assert.IsTrue(Reflection.SetFieldValue(
                projectBrowserWindow, projectBrowser, k_ProjectBrowserFieldName, string.Empty));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void SetFieldValue_NoType_ReturnsFalse()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(Reflection.SetFieldValue(projectBrowserWindow, null, TestLiterals.BadData,
                TestLiterals.Foo));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void SetFieldValue_BadField_ReturnsFalse()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(Reflection.SetFieldValue(projectBrowserWindow,
                    projectBrowser, TestLiterals.BadData, TestLiterals.Foo));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void SetPropertyValue_QualifiedProperty_IsSet()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);
            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsTrue(
                Reflection.TryGetPropertyValue(projectBrowserWindow, projectBrowser, k_ProjectBrowserPropertyName, out bool previousValue));

            Assert.IsTrue(
                Reflection.SetPropertyValue(projectBrowserWindow, projectBrowser, k_ProjectBrowserPropertyName, !previousValue));

            Reflection.TryGetPropertyValue(projectBrowserWindow, projectBrowser, k_ProjectBrowserPropertyName, out bool newValue);
            Assert.IsTrue(!previousValue == newValue);

            Reflection.SetPropertyValue(projectBrowserWindow, projectBrowser, k_ProjectBrowserPropertyName, previousValue);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void SetPropertyValue_NoType_IsFalse()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);
            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(
                Reflection.SetPropertyValue(projectBrowserWindow, null, k_ProjectBrowserPropertyName, false));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void SetPropertyValue_InvalidName_IsFalse()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);
            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(
                Reflection.SetPropertyValue(projectBrowserWindow, projectBrowser, TestLiterals.TestSeed, false));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void TryGetFieldOrPropertyValue_NoTarget_ReturnsDefault()
        {
            Assert.IsFalse(Reflection.TryGetFieldOrPropertyValue(
                null, k_ProjectBrowserFullName, out object missedObject));
            Assert.IsNull(missedObject);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void TryGetFieldOrPropertyValue_NoField_ReturnsDefault()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(Reflection.TryGetFieldOrPropertyValue(
                projectBrowserWindow, TestLiterals.BadData, out object missedObject));
            Assert.IsNull(missedObject);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void TryGetFieldOrPropertyValue_BadField_ReturnsStartGridSize()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsTrue(Reflection.TryGetFieldOrPropertyValue(
                projectBrowserWindow, k_ProjectBrowserFieldNameAlternative, out object gridSize));

            Assert.IsTrue(gridSize != null);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void TryGetFieldOrPropertyValue_ProjectBrowserField_ReturnsStartGridSize()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsTrue(Reflection.TryGetFieldOrPropertyValue(
                projectBrowserWindow, k_ProjectBrowserFieldNameAlternative, out object gridSize));

            Assert.IsTrue(gridSize != null);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void TryGetFieldOrPropertyValue_ProjectBrowserProperty_ReturnsIsLocked()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsTrue(Reflection.TryGetFieldOrPropertyValue(projectBrowserWindow, k_ProjectBrowserPropertyName,
                out object toggle));

            Assert.IsTrue(toggle != null);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void TryGetFieldValue_ProjectBrowser_ReturnsTrueGetFieldText()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsTrue(Reflection.TryGetFieldValue(projectBrowserWindow, projectBrowser, k_ProjectBrowserFieldName,
                out string query));

            Assert.IsTrue(query != null);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void TryGetFieldValue_NoType_ReturnsDefault()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(Reflection.TryGetFieldValue(
                projectBrowserWindow, null, TestLiterals.BadData, out string returnValue));
            Assert.IsNull(returnValue);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void TryGetFieldValue_BadField_ReturnsDefault()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(Reflection.TryGetFieldValue(
                projectBrowserWindow, projectBrowser, TestLiterals.BadData, out string returnValue));
            Assert.IsNull(returnValue);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void TryGetPropertyValue_QualifiedProperty_IsTrue()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);
            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsTrue(Reflection.TryGetPropertyValue(projectBrowserWindow, projectBrowser, k_ProjectBrowserPropertyName, out bool _));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void TryGetPropertyValue_NoType_IsFalse()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);
            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(Reflection.TryGetPropertyValue(projectBrowserWindow, null, k_ProjectBrowserPropertyName, out bool _));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void TryGetPropertyValue_InvalidName_IsFalse()
        {
            Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);
            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(Reflection.TryGetPropertyValue(projectBrowserWindow, projectBrowser, TestLiterals.BadData, out bool _));
        }
    }
}