// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;
using NUnit.Framework;

namespace GDX.Developer
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="TransientReference" /> class.
    /// </summary>
    public class TransientReferenceTests
    {
        [Test]
        [Category(Literals.TestCategory)]
        public void TransientReference_ObjectBased_CreatedReference()
        {

            TextGenerator mockGenerator = new TextGenerator();
            TransientReference mockReference = new TransientReference(mockGenerator);
            Assert.IsTrue(mockReference != null);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void TransientReference_ObjectBasedResurrection_CreatedReference()
        {

            TextGenerator mockGenerator = new TextGenerator();
            TransientReference mockReference = new TransientReference(mockGenerator, true);
            Assert.IsTrue(mockReference != null);
        }

//         [Test]
//         [Category(Core.TestCategory)]
//         public void TransientReference_SerializationInfo_CreatedReference()
//         {
//             SerializationInfo info = new SerializationInfo(typeof(TextGenerator), null, false);
//             TransientReference mockReference = new TransientReference(info, null);
//             Assert.IsTrue(mockReference != null, "Expect created reference.");
//         }

        [Test]
        [Category(Literals.TestCategory)]
        public void CompareTo_DuplicateTransientReference_ReturnsOne()
        {
            TextGenerator mockGenerator = new TextGenerator();
            TransientReference mockReference = new TransientReference(mockGenerator, true);
            TransientReference testReference = new TransientReference(mockGenerator);

            Assert.IsTrue(mockReference.CompareTo(testReference) == 1);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void CompareTo_BadTransientReference_ReturnsZero()
        {
            TextGenerator mockGenerator = new TextGenerator();
            TextGenerator mockGeneratorSecond = new TextGenerator();
            TransientReference mockReference = new TransientReference(mockGenerator, true);
            TransientReference testReference = new TransientReference(mockGeneratorSecond);

            Assert.IsTrue(mockReference.CompareTo(testReference) == 0);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void CompareTo_OriginalObject_ReturnsOne()
        {
            TextGenerator mockGenerator = new TextGenerator();
            TransientReference mockReference = new TransientReference(mockGenerator, true);
            Assert.IsTrue(mockReference.CompareTo(mockGenerator) == 1);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void CompareTo_WeakReference_ReturnsOne()
        {
            TextGenerator mockGenerator = new TextGenerator();

            WeakReference weakReference = new WeakReference(mockGenerator);
            TransientReference mockReference = new TransientReference(mockGenerator, true);
            Assert.IsTrue(mockReference.CompareTo(weakReference) == 1);
        }

    }
}