// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace GDX
{
    /// <summary>
    ///     A comparable weak reference to an object which will not prevent garbage collection. It will positively
    ///     compare against similar targeted reference trackers as well as the actual target object.
    /// </summary>
    /// <remarks>
    ///     There isn't a lot of great use cases for using this sort of thing; <see cref="WeakReference"/> on its own
    ///     is sufficient in most of the use cases, however this particular arrangement is useful for developer-ish
    ///     stuff.
    /// </remarks>
    public class TransientReference : WeakReference, IComparable, IComparable<TransientReference>,
        IComparable<WeakReference>, IEquatable<TransientReference>,
        IEquatable<WeakReference>
    {
        /// <inheritdoc />
        public TransientReference(object target) : base(target)
        {
        }

        /// <inheritdoc />
        public TransientReference(object target, bool trackResurrection) : base(target, trackResurrection)
        {
        }

        /// <inheritdoc />
        protected TransientReference([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public int CompareTo(object obj)
        {
            if (obj == Target || obj == this)
            {
                return 1;
            }

            return 0;
        }

        public int CompareTo(TransientReference obj)
        {
            if (obj.Target == Target || obj == this)
            {
                return 1;
            }

            return 0;
        }

        public int CompareTo(WeakReference obj)
        {
            if (obj.Target == Target || obj == this)
            {
                return 1;
            }

            return 0;
        }

        /// <inheritdoc />
        public bool Equals(TransientReference other)
        {
            if (other == null)
            {
                return false;
            }

            return Target == other.Target;
        }

        /// <inheritdoc />
        public bool Equals(WeakReference other)
        {
            if (other == null)
            {
                return false;
            }

            return Target == other.Target;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Target.GetHashCode();
        }

        public static bool operator ==(TransientReference left, TransientReference right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TransientReference left, TransientReference right)
        {
            return !Equals(left, right);
        }
    }
}