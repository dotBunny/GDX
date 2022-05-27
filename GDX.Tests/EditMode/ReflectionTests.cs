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



        [Test]
        [Category(Core.TestCategory)]
        public void SetFieldOrPropertyValue_ProjectBrowser_SetIsLocked()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser.");

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
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser.");

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
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser.");

            Assert.IsFalse(Reflection.SetFieldOrPropertyValue(projectBrowserWindow,
                "m_SearchFieldTextBad", "test"),
                "Expected default value for non existing type.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SetFieldOrPropertyValue_NoTarget_ReturnsFalse()
        {
            Assert.IsFalse(Reflection.SetFieldOrPropertyValue(null, "m_SearchFieldTextBad", "test"),
                "Expected false value when no target.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SetFieldValue_ProjectBrowser_SetSearchFieldText()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser.");

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
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser.");

            Assert.IsFalse(Reflection.SetFieldValue(projectBrowserWindow,
                    null, "m_SearchFieldTextBad", "test"),
                "Expected default value for non existing type.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SetPropertyValue_QualifiedProperty_IsSet()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");
            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser.");

            Assert.IsTrue(
                Reflection.TryGetPropertyValue(projectBrowserWindow, projectBrowser, "isLocked", out bool previousValue),
                "Expected to be able to retrieve the value.");

            Assert.IsTrue(
                Reflection.SetPropertyValue(projectBrowserWindow, projectBrowser, "isLocked", !previousValue),
                "Expected to be able to set the value.");

            Reflection.TryGetPropertyValue(projectBrowserWindow, projectBrowser, "isLocked", out bool newValue);
            Assert.IsTrue(!previousValue == newValue, "Expected set value to match retried value.");

            Reflection.SetPropertyValue(projectBrowserWindow, projectBrowser, "isLocked", previousValue);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SetPropertyValue_NoType_IsFalse()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");
            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser.");

            Assert.IsFalse(
                Reflection.SetPropertyValue(projectBrowserWindow, null, "isLocked", false),
                "Expected to not be able to set the value.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SetPropertyValue_InvalidName_IsFalse()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");
            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser.");

            Assert.IsFalse(
                Reflection.SetPropertyValue(projectBrowserWindow, projectBrowser, "isLockedBad", false),
                "Expected to not be able to set the value.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetFieldOrPropertyValue_NoTarget_ReturnsDefault()
        {
            Assert.IsFalse(Reflection.TryGetFieldOrPropertyValue(
                null, "UnityEditor.ProjectBrowser", out object missedObject));
            Assert.IsNull(missedObject, "Expected null returned object with no target.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetFieldOrPropertyValue_NoField_ReturnsDefault()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser");

            Assert.IsFalse(Reflection.TryGetFieldOrPropertyValue(
                projectBrowserWindow, "UnityEditor.ProjectBrowserBad", out object missedObject));
            Assert.IsNull(missedObject, "Expected null returned object with a bad field or property.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetFieldOrPropertyValue_BadField_ReturnsStartGridSize()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser");

            Assert.IsTrue(Reflection.TryGetFieldOrPropertyValue(
                projectBrowserWindow, "m_StartGridSize", out object gridSize));

            Assert.IsTrue(gridSize != null, "Expected non-null value for m_StartGridSize.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetFieldOrPropertyValue_ProjectBrowserField_ReturnsStartGridSize()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser");

            Assert.IsTrue(Reflection.TryGetFieldOrPropertyValue(
                projectBrowserWindow, "m_StartGridSize", out object gridSize));

            Assert.IsTrue(gridSize != null, "Expected non-null value for m_StartGridSize.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetFieldOrPropertyValue_ProjectBrowserProperty_ReturnsIsLocked()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser.");

            Assert.IsTrue(Reflection.TryGetFieldOrPropertyValue(projectBrowserWindow, "isLocked",
                out object toggle));

            Assert.IsTrue(toggle != null, "Expected non-null value for isLocked.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetFieldValue_ProjectBrowser_ReturnsTrueGetFieldText()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser.");

            Assert.IsTrue(Reflection.TryGetFieldValue(
                projectBrowserWindow, projectBrowser, "m_SearchFieldText", out string query));

            Assert.IsTrue(query != null, "Expected non-null value for m_SearchFieldText.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetFieldValue_NoType_ReturnsDefault()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser.");

            Assert.IsFalse(Reflection.TryGetFieldValue(
                projectBrowserWindow, null, "m_SearchFieldTextBad", out string returnValue));
            Assert.IsNull(returnValue, "Expected return value to be null.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetFieldValue_BadField_ReturnsDefault()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");

            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser.");

            Assert.IsFalse(Reflection.TryGetFieldValue(
                projectBrowserWindow, projectBrowser, "m_SearchFieldTextBad", out string returnValue));
            Assert.IsNull(returnValue, "Expected return value to be null.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetPropertyValue_QualifiedProperty_IsTrue()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");
            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser.");

            Assert.IsTrue(Reflection.TryGetPropertyValue(projectBrowserWindow, projectBrowser, "isLocked", out bool _),
                "Expected true response from method.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetPropertyValue_NoType_IsFalse()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");
            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser.");

            Assert.IsFalse(Reflection.TryGetPropertyValue(projectBrowserWindow, null, "isLocked", out bool _),
                "Expected false response from method.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetPropertyValue_InvalidName_IsFalse()
        {
            System.Type projectBrowser = Reflection.GetType("UnityEditor.ProjectBrowser");
            Assert.IsTrue(projectBrowser != null, "Expected to be able to find UnityEditor.ProjectBrowser type.");
            EditorWindow projectBrowserWindow = EditorWindow.GetWindow(projectBrowser);
            Assert.IsTrue(projectBrowserWindow != null, "Expected reference to ProjectBrowser.");

            Assert.IsFalse(Reflection.TryGetPropertyValue(projectBrowserWindow, projectBrowser, "isLockedBad", out bool _),
                "Expected false response from method.");
        }
    }
}