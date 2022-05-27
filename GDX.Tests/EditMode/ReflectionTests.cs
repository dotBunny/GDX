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
        const string k_ProjectBrowserFullName = "UnityEditor.ProjectBrowser";
        const string k_BadTypeFullName = "UnityEditor.ProjectBrowserBad";

        [Test]
        [Category(Core.TestCategory)]
        public void GetDefault_Nullable_ReturnsNull()
        {
            Assert.IsTrue(typeof(EditorWindow).GetDefault() == null);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetDefault_NonNullable_ReturnsValue()
        {
            object defaultBoxedValue = typeof(int).GetDefault();
            int defaultValue = (int)defaultBoxedValue;
            Assert.IsTrue(defaultValue == 0);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetType_QualifiedType_ReturnsType()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetType_BadType_ReturnsNull()
        {
            System.Type projectBrowser = Reflection.GetType(k_BadTypeFullName);
            Assert.IsTrue(projectBrowser == null);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void InvokeStaticMethod_ProjectBrowser_ReturnsListOfBrowsers()
        {
            object response = Reflection.InvokeStaticMethod(k_ProjectBrowserFullName, "GetAllProjectBrowsers");
            Assert.IsTrue(response != null);
        }

        [Test]
        [Category(Core.TestCategory)]
        [Category(Core.TestCategory)]
        public void InvokeStaticMethod_BadMethod_ReturnsNull()
        {
            object response = Reflection.InvokeStaticMethod(k_ProjectBrowserFullName, "GetAllProjectBrowsersBad");
            Assert.IsTrue(response == null);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void InvokeStaticMethod_BadType_ReturnsNull()
        {
            object response = Reflection.InvokeStaticMethod(k_BadTypeFullName, "GetAllProjectBrowsersBad");
            Assert.IsTrue(response == null);
        }



        [Test]
        [Category(Core.TestCategory)]
        public void SetFieldOrPropertyValue_ProjectBrowser_SetIsLocked()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsTrue(Reflection.TryGetFieldOrPropertyValue(projectBrowserWindow, "isLocked", out object previous));

            Assert.IsTrue(Reflection.SetFieldOrPropertyValue(
                projectBrowserWindow, "isLocked", !(bool)previous));

            Assert.IsTrue(Reflection.TryGetFieldOrPropertyValue(projectBrowserWindow, "isLocked", out object newValue));
            Assert.IsTrue((bool)newValue == !(bool)previous);

            Assert.IsTrue(Reflection.SetFieldOrPropertyValue(
                projectBrowserWindow, "isLocked", previous));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SetFieldOrPropertyValue_ProjectBrowser_SetSearchFieldText()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsTrue(Reflection.SetFieldOrPropertyValue(
                projectBrowserWindow, "m_SearchFieldText", "test"));

            Reflection.TryGetFieldValue<string>(
                projectBrowserWindow, projectBrowser, "m_SearchFieldText", out string tempValue);
            Assert.IsTrue(tempValue == "test");

            Assert.IsTrue(Reflection.SetFieldOrPropertyValue(
                projectBrowserWindow, "m_SearchFieldText", string.Empty));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SetFieldOrPropertyValue_NoType_ReturnsFalse()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(Reflection.SetFieldOrPropertyValue(projectBrowserWindow,
                "m_SearchFieldTextBad", "test"));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SetFieldOrPropertyValue_NoTarget_ReturnsFalse()
        {
            Assert.IsFalse(Reflection.SetFieldOrPropertyValue(null, "m_SearchFieldTextBad", "test"));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SetFieldValue_ProjectBrowser_SetSearchFieldText()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsTrue(Reflection.SetFieldValue(
                projectBrowserWindow, projectBrowser, "m_SearchFieldText", "test"));

            Assert.IsTrue(Reflection.TryGetFieldValue<string>(
                projectBrowserWindow, projectBrowser, "m_SearchFieldText", out string query));
            Assert.IsTrue(query == "test");

            Assert.IsTrue(Reflection.SetFieldValue(
                projectBrowserWindow, projectBrowser, "m_SearchFieldText", string.Empty));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SetFieldValue_NoType_ReturnsFalse()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(Reflection.SetFieldValue(projectBrowserWindow,
                    null, "m_SearchFieldTextBad", "test"));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SetFieldValue_BadField_ReturnsFalse()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(Reflection.SetFieldValue(projectBrowserWindow,
                    projectBrowser, "m_SearchFieldTextBad", "test"));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SetPropertyValue_QualifiedProperty_IsSet()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);
            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsTrue(
                Reflection.TryGetPropertyValue(projectBrowserWindow, projectBrowser, "isLocked", out bool previousValue));

            Assert.IsTrue(
                Reflection.SetPropertyValue(projectBrowserWindow, projectBrowser, "isLocked", !previousValue));

            Reflection.TryGetPropertyValue(projectBrowserWindow, projectBrowser, "isLocked", out bool newValue);
            Assert.IsTrue(!previousValue == newValue);

            Reflection.SetPropertyValue(projectBrowserWindow, projectBrowser, "isLocked", previousValue);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SetPropertyValue_NoType_IsFalse()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);
            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(
                Reflection.SetPropertyValue(projectBrowserWindow, null, "isLocked", false));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SetPropertyValue_InvalidName_IsFalse()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);
            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(
                Reflection.SetPropertyValue(projectBrowserWindow, projectBrowser, "isLockedBad", false));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetFieldOrPropertyValue_NoTarget_ReturnsDefault()
        {
            Assert.IsFalse(Reflection.TryGetFieldOrPropertyValue(
                null, k_ProjectBrowserFullName, out object missedObject));
            Assert.IsNull(missedObject);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetFieldOrPropertyValue_NoField_ReturnsDefault()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(Reflection.TryGetFieldOrPropertyValue(
                projectBrowserWindow, k_BadTypeFullName, out object missedObject));
            Assert.IsNull(missedObject);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetFieldOrPropertyValue_BadField_ReturnsStartGridSize()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsTrue(Reflection.TryGetFieldOrPropertyValue(
                projectBrowserWindow, "m_StartGridSize", out object gridSize));

            Assert.IsTrue(gridSize != null);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetFieldOrPropertyValue_ProjectBrowserField_ReturnsStartGridSize()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsTrue(Reflection.TryGetFieldOrPropertyValue(
                projectBrowserWindow, "m_StartGridSize", out object gridSize));

            Assert.IsTrue(gridSize != null);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetFieldOrPropertyValue_ProjectBrowserProperty_ReturnsIsLocked()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsTrue(Reflection.TryGetFieldOrPropertyValue(projectBrowserWindow, "isLocked",
                out object toggle));

            Assert.IsTrue(toggle != null);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetFieldValue_ProjectBrowser_ReturnsTrueGetFieldText()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsTrue(Reflection.TryGetFieldValue(
                projectBrowserWindow, projectBrowser, "m_SearchFieldText", out string query));

            Assert.IsTrue(query != null);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetFieldValue_NoType_ReturnsDefault()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(Reflection.TryGetFieldValue(
                projectBrowserWindow, null, "m_SearchFieldTextBad", out string returnValue));
            Assert.IsNull(returnValue);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetFieldValue_BadField_ReturnsDefault()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(Reflection.TryGetFieldValue(
                projectBrowserWindow, projectBrowser, "m_SearchFieldTextBad", out string returnValue));
            Assert.IsNull(returnValue);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetPropertyValue_QualifiedProperty_IsTrue()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);
            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsTrue(Reflection.TryGetPropertyValue(projectBrowserWindow, projectBrowser, "isLocked", out bool _));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetPropertyValue_NoType_IsFalse()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);
            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(Reflection.TryGetPropertyValue(projectBrowserWindow, null, "isLocked", out bool _));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetPropertyValue_InvalidName_IsFalse()
        {
            System.Type projectBrowser = Reflection.GetType(k_ProjectBrowserFullName);
            Assert.IsTrue(projectBrowser != null);
            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null);

            Assert.IsFalse(Reflection.TryGetPropertyValue(projectBrowserWindow, projectBrowser, "isLockedBad", out bool _));
        }
    }
}