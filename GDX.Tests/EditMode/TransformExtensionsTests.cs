﻿// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;
using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of <see cref="UnityEngine.Transform"/> extensions.
    /// </summary>
    public class TransformExtensionsTests
    {
        Transform m_BaseTransform;

        GameObject m_ChildOne;
        GameObject m_ChildTwo;
        GameObject m_ChildThree;

        [SetUp]
        public void Setup()
        {
            m_BaseTransform = new GameObject(TestLiterals.Foo).transform;

            m_ChildOne = new GameObject(TestLiterals.Foo);
            m_ChildOne.transform.SetParent(m_BaseTransform, false);

            m_ChildTwo = new GameObject(TestLiterals.Bar);
            m_ChildTwo.transform.SetParent(m_BaseTransform, false);

            m_ChildThree = new GameObject(TestLiterals.HelloWorld);
            m_ChildThree.transform.SetParent(m_BaseTransform, false);
            m_ChildThree.SetActive(false);
        }

        [TearDown]
        public void TearDown()
        {
            if (m_ChildThree != null)
            {
                Object.DestroyImmediate(m_ChildThree);
            }

            if (m_ChildTwo != null)
            {
                Object.DestroyImmediate(m_ChildTwo);
            }

            if (m_ChildOne != null)
            {
                Object.DestroyImmediate(m_ChildOne);
            }

            if (m_BaseTransform != null)
            {
                Object.DestroyImmediate(m_BaseTransform.gameObject);
            }
        }

        [Test]
        [Category(Core.TestCategory)]
        public void DestroyChildren_RemoveAll_NoChildren()
        {
            m_BaseTransform.DestroyChildren(true, true, true);
            int count = m_BaseTransform.childCount;
            Assert.IsTrue(count == 0);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void DestroyChildren_RemoveActiveOnly_OneChildRemaining()
        {
            m_BaseTransform.DestroyChildren(false, false, true);
            int count = m_BaseTransform.childCount;
            Assert.IsTrue(count == 1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetActiveChildrenCount_MockData_AccurateCount()
        {
            Assert.IsTrue(m_BaseTransform.GetActiveChildCount() == 2);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetFirstComponentInChildrenComplex_MockData_FirstTransform()
        {
            Transform t = m_BaseTransform.GetFirstComponentInChildrenComplex<Transform>(true, 0, 1);
            Assert.IsTrue(t != null);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetScenePath_MockData_RootPath()
        {
            Assert.IsTrue(m_BaseTransform.GetScenePath() == $"/{TestLiterals.Foo}");
        }
    }
}