// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace GDX.Mathematics
{
    /// <summary>
    ///     A <see cref="byte" /> vector.
    /// </summary>
    [DebuggerTypeProxy(typeof(DebuggerProxy))]
    [Serializable]
    public struct Byte2 : IEquatable<Byte2>, IFormattable
    {
        /// <summary>
        ///     X <see cref="byte" />.
        /// </summary>
        public byte X;

        /// <summary>
        ///     Y <see cref="byte" />.
        /// </summary>
        public byte Y;

        /// <summary>
        ///     Create a <see cref="Byte2" /> from two <see cref="int" /> values.
        /// </summary>
        /// <param name="x">X value.</param>
        /// <param name="y">Y value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Byte2(int x, int y)
        {
            X = (byte)x;
            Y = (byte)y;
        }

        /// <summary>
        ///     Constructs a <see cref="Byte2" /> from two <see cref="byte" /> values.
        /// </summary>
        /// <param name="x">X value.</param>
        /// <param name="y">Y value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Byte2(byte x, byte y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Constructs a <see cref="Byte2" /> from a <see cref="Byte2" /> value.
        /// </summary>
        /// <param name="xy">The value to copy.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Byte2(Byte2 xy)
        {
            X = xy.X;
            Y = xy.Y;
        }

        /// <summary>
        ///     Constructs a <see cref="Byte2" /> from a single <see cref="byte" /> value by assigning it to every component.
        /// </summary>
        /// <param name="v">The value to copy to <see cref="X" /> and <see cref="Y" />.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Byte2(byte v)
        {
            X = v;
            Y = v;
        }

        /// <summary>
        ///     Constructs a <see cref="Byte2" /> from a single <see cref="bool" /> value by converting it to <see cref="byte" />
        ///     and assigning it to every component.
        /// </summary>
        /// <param name="v">The <see cref="bool" /> value to transcribe.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Byte2(bool v)
        {
            X = v ? (byte)255 : (byte)0;
            Y = v ? (byte)255 : (byte)0;
        }

        /// <summary>
        ///     Constructs a <see cref="Byte2" /> from a single <see cref="float" /> value by converting it to <see cref="byte" />
        ///     and assigning it to every component.
        /// </summary>
        /// <param name="v">The <see cref="float" /> value to transcribe.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Byte2(float v)
        {
            X = (byte)v;
            Y = (byte)v;
        }

        /// <summary>
        ///     Constructs a <see cref="Byte2" /> from a single <see cref="double" /> value by converting it to <see cref="byte" />
        ///     and assigning it to every component.
        /// </summary>
        /// <param name="v">The <see cref="double" /> value to transcribe.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Byte2(double v)
        {
            X = (byte)v;
            Y = (byte)v;
        }

        /// <summary>
        ///     Constructs a <see cref="Byte2" /> from a <see cref="Unity.Mathematics.bool2" /> by conversion.
        /// </summary>
        /// <param name="v">The value to transcribe.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Byte2(bool2 v)
        {
            X = v.x ? (byte)255 : (byte)0;
            Y = v.y ? (byte)255 : (byte)0;
        }

        /// <summary>
        ///     Constructs a <see cref="Byte2" /> from a <see cref="Unity.Mathematics.float2" /> by conversion.
        /// </summary>
        /// <param name="v">The value to transcribe.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Byte2(float2 v)
        {
            X = (byte)v.x;
            Y = (byte)v.y;
        }

        /// <summary>
        ///     Constructs a <see cref="Byte2" /> from a <see cref="Unity.Mathematics.double2" /> by conversion.
        /// </summary>
        /// <param name="v">The value to transcribe.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Byte2(double2 v)
        {
            X = (byte)v.x;
            Y = (byte)v.y;
        }

        /// <summary>
        ///     Get a new <see cref="Byte2" /> created with <see cref="X" /> as both components.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        public Byte2 XX
#pragma warning restore IDE1006
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Byte2(X, X);
        }

        /// <summary>
        ///     Get a new <see cref="Byte2" /> created with identical components.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        public Byte2 XY
#pragma warning restore IDE1006
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Byte2(X, Y);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>
        ///     Get a new <see cref="Byte2" /> created with swapped components.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        public Byte2 YX
#pragma warning restore IDE1006
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Byte2(Y, X);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                Y = value.X;
                X = value.Y;
            }
        }

        /// <summary>
        ///     Get a new <see cref="Byte2" /> created with <see cref="Y" /> as both components.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        public Byte2 YY
#pragma warning restore IDE1006
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Byte2(Y, Y);
        }

        /// <summary>
        ///     Get the <see cref="byte" /> at the provided <paramref name="index" />.
        /// </summary>
        /// <param name="index">Returns the byte element at a specified index.</param>
        /// <exception cref="IndexOutOfRangeException">Out of range check.</exception>
        public unsafe byte this[int index]
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if ((uint)index >= 2)
                {
                    throw new IndexOutOfRangeException("The index must be between[0...1]");
                }
#endif
                fixed (Byte2* array = &this)
                {
                    return ((byte*)array)[index];
                }
            }
            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if ((uint)index >= 2)
                {
                    throw new IndexOutOfRangeException("The index must be between[0...1]");
                }
#endif
                fixed (byte* array = &X)
                {
                    array[index] = value;
                }
            }
        }

        /// <summary>
        ///     Does the <see cref="Byte2" /> equal another <see cref="Byte2" />.
        /// </summary>
        /// <param name="rhs">Target <see cref="Byte2" /> to compare with.</param>
        /// <returns>Returns true if the <see cref="Byte2" /> is equal to a given <see cref="Byte2" />, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Byte2 rhs)
        {
            return X == rhs.X && Y == rhs.Y;
        }

        /// <summary>
        ///     Convert the <see cref="Byte2" /> to a <see cref="string" /> using the provided <paramref name="format" />.
        /// </summary>
        /// <param name="format">Specified format <see cref="string" />.</param>
        /// <param name="formatProvider">Culture-specific format information.</param>
        /// <returns>
        ///     Returns a string representation of the <see cref="Byte2" /> using a specified format and culture-specific
        ///     format information.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"Byte2({X.ToString(format, formatProvider)},{Y.ToString(format, formatProvider)})";
        }

        /// <summary>
        ///     Does the <see cref="Byte2" /> equal another <see cref="object" /> (casted).
        /// </summary>
        /// <param name="o">Target <see cref="object" /> to compare with.</param>
        /// <returns>Returns true if the <see cref="Byte2" /> is equal to a given <see cref="Byte2" />, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object o)
        {
            return o != null && Equals((Byte2)o);
        }


        /// <summary>
        ///     Implicitly converts a single <see cref="byte" /> value to a <see cref="Byte2" /> by assigning it to every
        ///     component.
        /// </summary>
        /// <param name="v">The <see cref="byte" /> value to transcribe.</param>
        /// <returns>A new <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Byte2(byte v)
        {
            return new Byte2(v);
        }

        /// <summary>
        ///     Explicitly converts a single <see cref="bool" /> value to a <see cref="Byte2" /> by converting it to
        ///     <see cref="byte" /> and assigning it to every component.
        /// </summary>
        /// <param name="v">The <see cref="bool" /> value to transcribe.</param>
        /// <returns>A new <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Byte2(bool v)
        {
            return new Byte2(v);
        }

        /// <summary>
        ///     Explicitly converts a single <see cref="float" /> value to a <see cref="Byte2" /> by converting it to
        ///     <see cref="byte" /> and assigning it to every component.
        /// </summary>
        /// <param name="v">The <see cref="float" /> value to transcribe.</param>
        /// <returns>A new <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Byte2(float v)
        {
            return new Byte2(v);
        }

        /// <summary>
        ///     Explicitly converts a single <see cref="double" /> value to a <see cref="Byte2" /> by converting it to
        ///     <see cref="byte" /> and assigning it to every component.
        /// </summary>
        /// <param name="v">The <see cref="double" /> value to transcribe.</param>
        /// <returns>A new <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Byte2(double v)
        {
            return new Byte2(v);
        }

        /// <summary>
        ///     Multiply two <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>Returns the result of a  multiplication operation on two <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator *(Byte2 lhs, Byte2 rhs)
        {
            return new Byte2(lhs.X * rhs.X, lhs.Y * rhs.Y);
        }

        /// <summary>
        ///     Multiply a <see cref="Byte2" /> by a <see cref="byte" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="byte" />.</param>
        /// <returns>
        ///     Returns the result of a multiplication operation on a <see cref="Byte2" /> and a
        ///     <see cref="byte" /> value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator *(Byte2 lhs, byte rhs)
        {
            return new Byte2(lhs.X * rhs, lhs.Y * rhs);
        }

        /// <summary>
        ///     Multiply a <see cref="byte" /> by a <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="byte" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>
        ///     Returns the result of a multiplication operation on a <see cref="byte" /> and a
        ///     <see cref="Byte2" /> value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator *(byte lhs, Byte2 rhs)
        {
            return new Byte2(lhs * rhs.X, lhs * rhs.Y);
        }

        /// <summary>
        ///     Add two <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>Returns the result of an addition operation on two <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator +(Byte2 lhs, Byte2 rhs)
        {
            return new Byte2(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }

        /// <summary>
        ///     Add a <see cref="byte" /> to both components of a <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="byte" />.</param>
        /// <returns>
        ///     Returns the result of an addition operation on an <see cref="Byte2" /> and an <see cref="byte" />
        ///     value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator +(Byte2 lhs, byte rhs)
        {
            return new Byte2(lhs.X + rhs, lhs.Y + rhs);
        }

        /// <summary>
        ///     Add a <see cref="Byte2" /> to a <see cref="byte" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="byte" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>
        ///     Returns the result of an addition operation on an <see cref="byte" /> value and an
        ///     <see cref="Byte2" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator +(byte lhs, Byte2 rhs)
        {
            return new Byte2(lhs + rhs.X, lhs + rhs.Y);
        }

        /// <summary>
        ///     Subtract a <see cref="Byte2" /> from another <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>Returns the result of a subtraction operation on two <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator -(Byte2 lhs, Byte2 rhs)
        {
            return new Byte2(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

        /// <summary>
        ///     Subtract a <see cref="byte" /> from both components of a <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="byte" />.</param>
        /// <returns>
        ///     Returns the result of a subtraction operation on a <see cref="Byte2" /> and a <see cref="byte" />
        ///     value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator -(Byte2 lhs, byte rhs)
        {
            return new Byte2(lhs.X - rhs, lhs.Y - rhs);
        }

        /// <summary>
        ///     Subtract both components of a <see cref="Byte2" /> from a <see cref="byte" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="byte" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>
        ///     Returns the result of a subtraction operation on an <see cref="byte" /> value and an
        ///     <see cref="Byte2" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator -(byte lhs, Byte2 rhs)
        {
            return new Byte2(lhs - rhs.X, lhs - rhs.Y);
        }

        /// <summary>
        ///     Divide a <see cref="Byte2" /> by another <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>Returns the result of a division operation on two <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator /(Byte2 lhs, Byte2 rhs)
        {
            return new Byte2(lhs.X / rhs.X, lhs.Y / rhs.Y);
        }

        /// <summary>
        ///     Divide a <see cref="Byte2" /> by a <see cref="byte" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="byte" />.</param>
        /// <returns>
        ///     Returns the result of a division operation on a <see cref="Byte2" /> and <see cref="byte" />
        ///     value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator /(Byte2 lhs, byte rhs)
        {
            return new Byte2(lhs.X / rhs, lhs.Y / rhs);
        }

        /// <summary>
        ///     Divide a <see cref="byte" /> by a <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="byte" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>
        ///     Returns the result of a division operation on a <see cref="byte" /> value and
        ///     <see cref="Byte2" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator /(byte lhs, Byte2 rhs)
        {
            return new Byte2(lhs / rhs.X, lhs / rhs.Y);
        }

        /// <summary>
        ///     Modulus a <see cref="Byte2" /> by another <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>Returns the result of a modulus operation on two <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator %(Byte2 lhs, Byte2 rhs)
        {
            return new Byte2(lhs.X % rhs.X, lhs.Y % rhs.Y);
        }

        /// <summary>
        ///     Modulus a <see cref="Byte2" /> by a <see cref="byte" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="byte" />.</param>
        /// <returns>
        ///     Returns the result of a modulus operation on a <see cref="Byte2" /> and <see cref="byte" />
        ///     value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator %(Byte2 lhs, byte rhs)
        {
            return new Byte2(lhs.X % rhs, lhs.Y % rhs);
        }

        /// <summary>
        ///     Modulus a <see cref="byte" /> by a <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="byte" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>
        ///     Returns the result of a modulus operation on a <see cref="byte" /> value and
        ///     <see cref="Byte2" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator %(byte lhs, Byte2 rhs)
        {
            return new Byte2(lhs % rhs.X, lhs % rhs.Y);
        }

        /// <summary>
        ///     Increment <see cref="Byte2" /> values.
        /// </summary>
        /// <param name="val">Target <see cref="Byte2" />.</param>
        /// <returns>Returns the result of an increment operation on a <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator ++(Byte2 val)
        {
            return new Byte2(++val.X, ++val.Y);
        }

        /// <summary>
        ///     Decrement <see cref="Byte2" /> values.
        /// </summary>
        /// <param name="val">Target <see cref="Byte2" />.</param>
        /// <returns>Returns the result of a decrement operation on a <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator --(Byte2 val)
        {
            return new Byte2(--val.X, --val.Y);
        }

        /// <summary>
        ///     Unary minus <see cref="Byte2" /> values.
        /// </summary>
        /// <param name="val">Target <see cref="Byte2" />.</param>
        /// <returns>Returns the result of an unary minus operation on a <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator -(Byte2 val)
        {
            return new Byte2(-val.X, -val.Y);
        }

        /// <summary>
        ///     Unary plus <see cref="Byte2" /> values.
        /// </summary>
        /// <param name="val">Target <see cref="Byte2" />.</param>
        /// <returns>Returns the result of an unary plus operation on a <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator +(Byte2 val)
        {
            return new Byte2(+val.X, +val.Y);
        }

        /// <summary>
        ///     Bitwise NOT <see cref="Byte2" /> values.
        /// </summary>
        /// <param name="val">Target <see cref="Byte2" />.</param>
        /// <returns>Returns the result of a bitwise NOT operation on a <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator ~(Byte2 val)
        {
            return new Byte2(~val.X, ~val.Y);
        }

        /// <summary>
        ///     Bitwise AND two <see cref="Byte2" /> values.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>Returns the result of a bitwise AND operation on two <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator &(Byte2 lhs, Byte2 rhs)
        {
            return new Byte2(lhs.X & rhs.X, lhs.Y & rhs.Y);
        }

        /// <summary>
        ///     Bitwise AND a <see cref="Byte2" /> and a <see cref="byte" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="byte" />.</param>
        /// <returns>
        ///     Returns the result of a bitwise AND operation on a <see cref="Byte2" /> and a <see cref="byte" />
        ///     value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator &(Byte2 lhs, byte rhs)
        {
            return new Byte2(lhs.X & rhs, lhs.Y & rhs);
        }

        /// <summary>
        ///     Bitwise AND a <see cref="byte" /> and a <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="byte" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>
        ///     Returns the result of a bitwise AND operation on a <see cref="byte" /> and a <see cref="Byte2" />
        ///     value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator &(byte lhs, Byte2 rhs)
        {
            return new Byte2(lhs & rhs.X, lhs & rhs.Y);
        }

        /// <summary>
        ///     Bitwise OR two <see cref="Byte2" /> values.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>Returns the result of a bitwise OR operation on two <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator |(Byte2 lhs, Byte2 rhs)
        {
            return new Byte2(lhs.X | rhs.X, lhs.Y | rhs.Y);
        }

        /// <summary>
        ///     Bitwise OR a <see cref="Byte2" /> and a <see cref="byte" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="byte" />.</param>
        /// <returns>
        ///     Returns the result of a bitwise OR operation on a <see cref="Byte2" /> and a <see cref="byte" />
        ///     value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator |(Byte2 lhs, byte rhs)
        {
            return new Byte2(lhs.X | rhs, lhs.Y | rhs);
        }

        /// <summary>
        ///     Bitwise OR a <see cref="byte" /> and a <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="byte" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>
        ///     Returns the result of a bitwise OR operation on a <see cref="byte" /> and a <see cref="Byte2" />
        ///     value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator |(byte lhs, Byte2 rhs)
        {
            return new Byte2(lhs | rhs.X, lhs | rhs.Y);
        }

        /// <summary>
        ///     Bitwise XOR two <see cref="Byte2" /> values.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>Returns the result of a bitwise EXCLUSIVE OR operation on two <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator ^(Byte2 lhs, Byte2 rhs)
        {
            return new Byte2(lhs.X ^ rhs.X, lhs.Y ^ rhs.Y);
        }

        /// <summary>
        ///     Bitwise XOR a <see cref="Byte2" /> and a <see cref="byte" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="byte" />.</param>
        /// <returns>
        ///     Returns the result of a bitwise XOR operation on a <see cref="Byte2" /> and a <see cref="byte" />
        ///     value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator ^(Byte2 lhs, byte rhs)
        {
            return new Byte2(lhs.X ^ rhs, lhs.Y ^ rhs);
        }

        /// <summary>
        ///     Bitwise XOR a <see cref="byte" /> and a <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="byte" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>
        ///     Returns the result of a bitwise XOR operation on a <see cref="byte" /> and a <see cref="Byte2" />
        ///     value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte2 operator ^(byte lhs, Byte2 rhs)
        {
            return new Byte2(lhs ^ rhs.X, lhs ^ rhs.Y);
        }

        /// <summary>
        ///     Get a hash code from the <see cref="Byte2" />.
        /// </summary>
        /// <remarks>
        ///     This loosely based on the Fowler–Noll–Vo (FNV) hash function.
        /// </remarks>
        /// <returns>A <see cref="int" /> value.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                const int k_P = 16777619;
                int hash = (int)2166136261;

                hash = (hash ^ X) * k_P;
                hash = (hash ^ Y) * k_P;

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;

                return hash;
            }
        }

        /// <summary>
        ///     Returns a <see cref="string" /> representation of the <see cref="Byte2" />.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return $"Byte2({X.ToString()},{Y.ToString()})";
        }

        /// <summary>
        ///     Debug object used by IDEs for visibility of a <see cref="Byte2" />.
        /// </summary>
        internal sealed class DebuggerProxy
        {
            public DebuggerProxy(Byte2 v)
            {
                X = v.X;
                Y = v.Y;
            }
            /// <summary>
            ///     X <see cref="byte" />.
            /// </summary>
            public byte X;

            /// <summary>
            ///     Y <see cref="byte" />.
            /// </summary>
            public byte Y;
        }

        /// <summary>
        ///     Explicitly converts a <see cref="Unity.Mathematics.bool2" /> to a <see cref="Byte2" /> by conversion.
        /// </summary>
        /// <param name="v">The value to transcribe.</param>
        /// <returns>A new <see cref="Byte2" /> created from <paramref name="v" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Byte2(bool2 v)
        {
            return new Byte2(v);
        }

        /// <summary>
        ///     Explicitly converts a <see cref="Unity.Mathematics.bool2" /> to a <see cref="Byte2" /> by conversion.
        /// </summary>
        /// <param name="v">The value to transcribe.</param>
        /// <returns>A new <see cref="Byte2" /> created from <paramref name="v" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Byte2(float2 v)
        {
            return new Byte2(v);
        }

        /// <summary>
        ///     Explicitly converts a <see cref="Unity.Mathematics.double2" /> to a <see cref="Byte2" /> by
        ///     conversion.
        /// </summary>
        /// <param name="v">The value to transcribe.</param>
        /// <returns>A new <see cref="Byte2" /> created from <paramref name="v" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Byte2(double2 v)
        {
            return new Byte2(v);
        }

        /// <summary>
        ///     Determine if one <see cref="Byte2" /> is less than another <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>Returns the result of a LESS THAN operation on two <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator <(Byte2 lhs, Byte2 rhs)
        {
            return new bool2(lhs.X < rhs.X, lhs.Y < rhs.Y);
        }

        /// <summary>
        ///     Determine if <see cref="Byte2" /> is less than a <see cref="byte" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="byte" />.</param>
        /// <returns>
        ///     Returns the result of a LESS THAN operation on a <see cref="Byte2" /> and a <see cref="byte" />
        ///     value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator <(Byte2 lhs, byte rhs)
        {
            return new bool2(lhs.X < rhs, lhs.Y < rhs);
        }

        /// <summary>
        ///     Determine if <see cref="byte" /> is less than a <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="byte" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>
        ///     Returns the result of a LESS THAN operation on a <see cref="byte" /> and a <see cref="Byte2" />
        ///     value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator <(byte lhs, Byte2 rhs)
        {
            return new bool2(lhs < rhs.X, lhs < rhs.Y);
        }

        /// <summary>
        ///     Determine if one <see cref="Byte2" /> is less than or equal to another <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>Returns the result of a LESS THAN OR EQUAL operation on two <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator <=(Byte2 lhs, Byte2 rhs)
        {
            return new bool2(lhs.X <= rhs.X, lhs.Y <= rhs.Y);
        }

        /// <summary>
        ///     Determine if <see cref="Byte2 " /> is less than or equal a <see cref="byte" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="byte" />.</param>
        /// <returns>
        ///     Returns the result of a LESS THAN OR EQUAL operation on a <see cref="Byte2" /> and a
        ///     <see cref="byte" />
        ///     value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator <=(Byte2 lhs, byte rhs)
        {
            return new bool2(lhs.X <= rhs, lhs.Y <= rhs);
        }

        /// <summary>
        ///     Determine if <see cref="byte" /> is less than or equal a <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="byte" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>
        ///     Returns the result of a LESS THAN OR EQUAL operation on a <see cref="byte" /> and a
        ///     <see cref="Byte2" />
        ///     value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator <=(byte lhs, Byte2 rhs)
        {
            return new bool2(lhs <= rhs.X, lhs <= rhs.Y);
        }

        /// <summary>
        ///     Determine if one <see cref="Byte2" /> is less than another <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>Returns the result of a LESS THAN operation on two <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator >(Byte2 lhs, Byte2 rhs)
        {
            return new bool2(lhs.X > rhs.X, lhs.Y > rhs.Y);
        }

        /// <summary>
        ///     Determine if <see cref="Byte2 " /> is greater than a <see cref="byte" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="byte" />.</param>
        /// <returns>
        ///     Returns the result of a GREATER THAN operation on a <see cref="Byte2" /> and a <see cref="byte" />
        ///     value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator >(Byte2 lhs, byte rhs)
        {
            return new bool2(lhs.X > rhs, lhs.Y > rhs);
        }

        /// <summary>
        ///     Determine if <see cref="byte" /> is greater than a <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="byte" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>
        ///     Returns the result of a GREATER THAN operation on a <see cref="byte" /> and a <see cref="Byte2" />
        ///     value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator >(byte lhs, Byte2 rhs)
        {
            return new bool2(lhs > rhs.X, lhs > rhs.Y);
        }


        /// <summary>
        ///     Determine if <see cref="Byte2 " /> is greater than or equal a <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>
        ///     Returns the result of a GREATER THAN OR EQUAL operation on a <see cref="Byte2" /> and a
        ///     <see cref="Byte2" />
        ///     value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator >=(Byte2 lhs, Byte2 rhs)
        {
            return new bool2(lhs.X >= rhs.X, lhs.Y >= rhs.Y);
        }

        /// <summary>
        ///     Determine if <see cref="Byte2 " /> is greater than or equal a <see cref="byte" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="byte" />.</param>
        /// <returns>
        ///     Returns the result of a GREATER THAN OR EQUAL operation on a <see cref="Byte2" /> and a
        ///     <see cref="byte" />
        ///     value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator >=(Byte2 lhs, byte rhs)
        {
            return new bool2(lhs.X >= rhs, lhs.Y >= rhs);
        }

        /// <summary>
        ///     Determine if <see cref="byte " /> is greater than or equal a <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="byte" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>
        ///     Returns the result of a GREATER THAN OR EQUAL operation on a <see cref="byte" /> and a
        ///     <see cref="Byte2" />
        ///     value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator >=(byte lhs, Byte2 rhs)
        {
            return new bool2(lhs >= rhs.X, lhs >= rhs.Y);
        }

        /// <summary>
        ///     Determine if one <see cref="Byte2" /> is equal to another <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>Returns the result of an EQUALITY operation on two <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator ==(Byte2 lhs, Byte2 rhs)
        {
            return new bool2(lhs.X == rhs.X, lhs.Y == rhs.Y);
        }

        /// <summary>
        ///     Determine if both components of a <see cref="Byte2" /> are equal to a <see cref="byte" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="byte" />.</param>
        /// <returns>Returns the result of an EQUALITY operation on a <see cref="Byte2" /> and a <see cref="byte" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator ==(Byte2 lhs, byte rhs)
        {
            return new bool2(lhs.X == rhs, lhs.Y == rhs);
        }

        /// <summary>
        ///     Determine if both components of a <see cref="Byte2" /> are equal to a <see cref="byte" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="byte" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>Returns the result of an EQUALITY operation on a <see cref="byte" /> and a <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator ==(byte lhs, Byte2 rhs)
        {
            return new bool2(lhs == rhs.X, lhs == rhs.Y);
        }

        /// <summary>
        ///     Determine if one <see cref="Byte2" /> is not equal to another <see cref="Byte2" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>Returns the result of a NOT EQUAL operation on two <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator !=(Byte2 lhs, Byte2 rhs)
        {
            return new bool2(lhs.X != rhs.X, lhs.Y != rhs.Y);
        }

        /// <summary>
        ///     Determine if both components of a <see cref="Byte2" /> are not equal to a <see cref="byte" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="Byte2" />.</param>
        /// <param name="rhs">Right-hand side <see cref="byte" />.</param>
        /// <returns>Returns the result of a NOT EQUAL operation on a <see cref="Byte2" /> and a <see cref="byte" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator !=(Byte2 lhs, byte rhs)
        {
            return new bool2(lhs.X != rhs, lhs.Y != rhs);
        }

        /// <summary>
        ///     Determine if both components of a <see cref="Byte2" /> are not equal to a <see cref="byte" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="byte" />.</param>
        /// <param name="rhs">Right-hand side <see cref="Byte2" />.</param>
        /// <returns>Returns the result of a NOT EQUAL operation on a <see cref="byte" /> and a <see cref="Byte2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator !=(byte lhs, Byte2 rhs)
        {
            return new bool2(lhs != rhs.X, lhs != rhs.Y);
        }
    }
}