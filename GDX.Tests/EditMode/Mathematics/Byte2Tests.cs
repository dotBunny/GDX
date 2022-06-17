// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Text;
using NUnit.Framework;

namespace GDX.Mathematics
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="Byte2"/>.
    /// </summary>
    public class Byte2Tests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_CreateFromInteger()
        {
            Byte2 test = new Byte2(0, 1);

            Assert.IsTrue(test.X == 0 &&
                          test.Y == 1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_CreateFromBytes()
        {
            byte a = 97;
            byte b = 98;

            Byte2 test = new Byte2(a, b);

            Assert.IsTrue(test.X == 'a' &&
                          test.Y == 'b');
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_CreateFromByte()
        {
            byte a = 97;

            Byte2 test = new Byte2(a);

            Assert.IsTrue(test.X == 'a' &&
                          test.Y == 'a');
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_CreateFromByte2()
        {
            Byte2 test = new Byte2('a', 'b');
            Byte2 created = new Byte2(test);
            Assert.IsTrue(created.X == 'a' &&
                          created.Y == 'b');
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_CreateFromBool()
        {
            Byte2 test = new Byte2(true);
            Assert.IsTrue(test.X == 255 && test.Y == 255);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_CreateFromFloat()
        {
            Byte2 test = new Byte2(1f);
            Assert.IsTrue(test.X == 1 && test.Y == 1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_CreateFromDouble()
        {
            Byte2 test = new Byte2(1d);
            Assert.IsTrue(test.X == 1 && test.Y == 1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void XX_CreateNew()
        {
            Byte2 test = new Byte2(1,2);
            Byte2 eval = test.XX;
            Assert.IsTrue(eval.X == 1 && eval.Y == 1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void XY_CreateNew()
        {
            Byte2 test = new Byte2(1,2);
            Byte2 eval = test.XY;
            Assert.IsTrue(eval.X == 1 && eval.Y == 2);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void XY_SetValue()
        {
            Byte2 test = new Byte2(1,2);
            test.XY = new Byte2(10, 20);
            Assert.IsTrue(test.X == 10 && test.Y == 20);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void YX_CreateNew()
        {
            Byte2 test = new Byte2(1,2);
            Byte2 eval = test.YX;
            Assert.IsTrue(eval.X == 2 && eval.Y == 1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void YX_SetValue()
        {
            Byte2 test = new Byte2(1,2);
            test.YX = new Byte2(10, 20);
            Assert.IsTrue(test.X == 20 && test.Y == 10);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void YY_CreateNew()
        {
            Byte2 test = new Byte2(1,2);
            Byte2 eval = test.YY;
            Assert.IsTrue(eval.X == 2 && eval.Y == 2);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Accessor_Get_InRange()
        {
            Byte2 test = new Byte2(1,2);
            Assert.IsTrue(test[0] == 1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Accessor_Get_OutOfRange()
        {
            Byte2 test = new Byte2(1,2);
            Assert.Throws<IndexOutOfRangeException>(() => { byte fall = test[2]; });
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Accessor_Set_InRange()
        {
            Byte2 test = new Byte2(1,2);
            test[0] = 2;
            Assert.IsTrue(test[0] == 2);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Accessor_Set_OutOfRange()
        {
            Byte2 test = new Byte2(1,2);
            Assert.Throws<IndexOutOfRangeException>(() => { test[2] = 1; });
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Equals_Compare_Byte2()
        {
            Byte2 a = new Byte2(5,10);
            Byte2 b = new Byte2(5, 10);
            Assert.IsTrue(a.Equals(b));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Equals_Compare_Object()
        {
            Byte2 a = new Byte2(5,10);
            Byte2 b = new Byte2(5, 10);
            Assert.IsTrue(a.Equals((object)b));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ToString_Output_ByteIntegerValues()
        {
            Byte2 test = new Byte2('a', 'b');
            Localization.SetDefaultCulture();
            string evaluate = test.ToString("", new CultureInfo(Localization.Language.English.GetIETF_BCP47()));
            Assert.IsTrue(evaluate == "Byte2(97,98)");
        }
    }
}