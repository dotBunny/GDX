// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;

namespace GDX.Developer
{
    /// <summary>
    ///     A comparable weak reference to an object which will not prevent garbage collection. It will positively
    ///     compare against similar targeted reference trackers as well as the actual target object.
    /// </summary>
    /// <remarks>
    ///     There isn't a lot of great use cases for using this sort of thing; <see cref="WeakReference" /> on its own
    ///     is sufficient in most of the use cases, however this particular arrangement is useful for developer-ish
    ///     stuff.
    /// </remarks>
    public class TransientReference : WeakReference, IComparable, IComparable<TransientReference>,
        IComparable<WeakReference>, IEquatable<TransientReference>,
        IEquatable<WeakReference>
    {
        /// <summary>
        ///     Create a <see cref="TransientReference" /> referencing the <paramref name="target" />.
        /// </summary>
        /// <param name="target">The target <see cref="object" /> to reference.</param>
        public TransientReference(object target) : base(target)
        {
        }

        /// <summary>
        ///     Create a <see cref="TransientReference" /> referencing the <paramref name="target" />.
        /// </summary>
        /// <param name="target">The target <see cref="object" /> to reference.</param>
        /// <param name="trackResurrection">Should the object remain tracked after it has been finalized.</param>
        public TransientReference(object target, bool trackResurrection) : base(target, trackResurrection)
        {
        }

        /// <summary>
        ///     Create a <see cref="TransientReference" /> from the <paramref name="info" />.
        /// </summary>
        /// <param name="info">A <see cref="SerializationInfo" /> representation of a <see cref="TransientReference" />.</param>
        /// <param name="context">Describes the source of the <see cref="SerializationInfo" />.</param>
        protected TransientReference(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        ///     Compare this <see cref="TransientReference" /> to the target <see cref="object" />.
        /// </summary>
        /// <param name="obj">The target <see cref="object" /> to compare against.</param>
        /// <returns>1 if the same, 0 otherwise.</returns>
        public int CompareTo(object obj)
        {
            if (obj == Target || (TransientReference) obj == this)
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        ///     Compare this <see cref="TransientReference" /> to the target <see cref="TransientReference" />.
        /// </summary>
        /// <param name="obj">The target <see cref="TransientReference" /> to compare against.</param>
        /// <returns>1 if the same, 0 otherwise.</returns>
        public int CompareTo(TransientReference obj)
        {
            if (obj.Target == Target || obj == this)
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        ///     Compare this <see cref="TransientReference" /> to the target <see cref="WeakReference" />.
        /// </summary>
        /// <param name="obj">The target <see cref="WeakReference" /> to compare against.</param>
        /// <returns>1 if the same, 0 otherwise.</returns>
        public int CompareTo(WeakReference obj)
        {
            if (obj.Target == Target || (TransientReference) obj == this)
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        ///     Does this <see cref="TransientReference" /> equal the target <see cref="TransientReference" />.
        /// </summary>
        /// <param name="other">The target <see cref="TransientReference" /> to compare with.</param>
        /// <returns>true if it is the same, false otherwise.</returns>
        public bool Equals(TransientReference other)
        {
            if (other == null)
            {
                return false;
            }

            return Target == other.Target;
        }

        /// <summary>
        ///     Does this <see cref="TransientReference" /> equal the target <see cref="WeakReference" />.
        /// </summary>
        /// <param name="other">The target <see cref="WeakReference" /> to compare with.</param>
        /// <returns>true if it is the same, false otherwise.</returns>
        public bool Equals(WeakReference other)
        {
            if (other == null)
            {
                return false;
            }

            return Target == other.Target;
        }

        /// <summary>
        ///     Does this <see cref="TransientReference" /> equal the target <see cref="object" />.
        /// </summary>
        /// <param name="obj">The target <see cref="object" /> to compare with.</param>
        /// <returns>true if it is the same, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj) || ReferenceEquals(this, obj))
            {
                return false;
            }

            if (obj == Target)
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((TransientReference)obj);
        }

        /// <summary>
        ///     Return the hashcode of the <see cref="WeakReference.Target"/>.
        /// </summary>
        /// <returns>Returns the <see cref="WeakReference.Target"/>'s hash code, or -1 if null.</returns>
        public override int GetHashCode()
        {
            if (Target == null)
            {
                return -1;
            }

            return Target.GetHashCode();
        }

        /// <summary>
        ///     Compare <see cref="TransientReference" />s to see if they are equal.
        /// </summary>
        /// <param name="left">Left-side <see cref="TransientReference" />.</param>
        /// <param name="right">Right-side <see cref="TransientReference" />.</param>
        /// <returns>true/false if they are equal.</returns>
        public static bool operator ==(TransientReference left, TransientReference right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     Compare <see cref="TransientReference" />s to see if they are not equal.
        /// </summary>
        /// <param name="left">Left-side <see cref="TransientReference" />.</param>
        /// <param name="right">Right-side <see cref="TransientReference" />.</param>
        /// <returns>true/false if they are not equal.</returns>
        public static bool operator !=(TransientReference left, TransientReference right)
        {
            return !Equals(left, right);
        }
    }
}