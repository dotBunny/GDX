// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX
{
    public static class Space
    {
        public enum Axis
        {
            Undefined,
            X,
            Y,
            Z
        }

        public enum Direction
        {
            /// <summary>
            ///     An undetermined direction.
            /// </summary>
            Undefined,

            /// <summary>
            ///     Z axis.
            /// </summary>
            Forward,

            /// <summary>
            ///     -Z axis.
            /// </summary>
            Backward,

            /// <summary>
            ///     -X axis.
            /// </summary>
            Left,

            /// <summary>
            ///     X axis.
            /// </summary>
            Right,

            /// <summary>
            ///     Y axis.
            /// </summary>
            Up,

            /// <summary>
            ///     -Y axis.
            /// </summary>
            Down
        }

#if !UNITY_DOTSRUNTIME
        static readonly Quaternion k_RotationLeft = Quaternion.Euler(-90, 0, 0);
        static readonly Quaternion k_RotationRight = Quaternion.Euler(90, 0, 0);
        static readonly Quaternion k_RotationUp = Quaternion.Euler(0, 90, 0);
        static readonly Quaternion k_RotationDown = Quaternion.Euler(0, -90, 0);
        static readonly Quaternion k_RotationForward = Quaternion.Euler(0, 0, 90);
        static readonly Quaternion k_RotationBackward = Quaternion.Euler(0, 0, -90);

        /// <summary>
        ///     Get the corresponding <see cref="Direction" /> for a <see cref="Vector3" /> provided direction.
        /// </summary>
        /// <remarks>Only works with deterministic directions builtin to Unity.</remarks>
        /// <param name="direction">The given direction.</param>
        /// <returns>A qualified <see cref="Direction" /> equivalent to the <paramref name="direction" />.</returns>
        public static Direction GetDirection(Vector3 direction)
        {
            // Left/Right
            if (direction.y == 0 && direction.z == 0)
            {
                if (direction.x > 0)
                {
                    return Direction.Right;
                }

                if (direction.x < 0)
                {
                    return Direction.Left;
                }
            }

            // Forward / Backward
            if (direction.x == 0 && direction.y == 0)
            {
                if (direction.z > 0)
                {
                    return Direction.Forward;
                }

                if (direction.z < 0)
                {
                    return Direction.Backward;
                }
            }

            // Up / Down
            if (direction.x == 0 && direction.z == 0)
            {
                if (direction.y > 0)
                {
                    return Direction.Up;
                }

                if (direction.y < 0)
                {
                    return Direction.Down;
                }
            }

            return Direction.Undefined;
        }

        public static Quaternion ToRotation(this Axis axis)
        {
            return axis switch
            {
                Axis.Undefined => Quaternion.identity,
                Axis.X => k_RotationRight,
                Axis.Y => k_RotationUp,
                Axis.Z => k_RotationForward,
                _ => Quaternion.identity
            };
        }

        public static Quaternion ToRotation(this Direction direction)
        {
            return direction switch
            {
                Direction.Undefined => Quaternion.identity,
                Direction.Left => k_RotationLeft,
                Direction.Right => k_RotationRight,
                Direction.Up => k_RotationUp,
                Direction.Down => k_RotationDown,
                Direction.Backward => k_RotationBackward,
                Direction.Forward => k_RotationForward,
                _ => Quaternion.identity
            };
        }
#endif // !UNITY_DOTSRUNTIME
    }
}