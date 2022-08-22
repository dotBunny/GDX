// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using UnityEngine.LowLevel;

namespace GDX
{
    public static class PlayerLoopSystemExtensions
    {
        public static ref PlayerLoopSystem TryGetChildSystem(this ref PlayerLoopSystem rootSystem, Type systemType, out bool foundSystem)
        {
            int subCount = rootSystem.subSystemList.Length;

            for (int i = 0; i < subCount; i++)
            {
                // Wishful thinking
                if (rootSystem.subSystemList[i].type == systemType)
                {
                    foundSystem = true;
                    return ref rootSystem.subSystemList[i];
                }

                // Evaluate children
                if (rootSystem.subSystemList[i].subSystemList.Length > 0)
                {
                    ref PlayerLoopSystem child = ref rootSystem.subSystemList[i].TryGetChildSystem(systemType, out foundSystem);
                    return ref child;
                }
            }

            foundSystem = false;
            return ref rootSystem;
        }

        public static bool ReplaceChildSystem(this ref PlayerLoopSystem rootSystem, Type systemType,
            ref PlayerLoopSystem updatedSystem)
        {
            return false;
        }

        public static bool RemoveChildSystem(this ref PlayerLoopSystem rootSystem, Type parentSystemType, Type childSystemType)
        {
            ref PlayerLoopSystem foundParent = ref rootSystem.TryGetChildSystem(parentSystemType, out bool foundTargetSystem);
            if (!foundTargetSystem) return false;

            return RemoveChildSystem(ref foundParent, childSystemType);
        }

        public static bool RemoveChildSystem(this ref PlayerLoopSystem parentSystem, Type childSystemType)
        {
            ref PlayerLoopSystem[] previousSubSystems = ref parentSystem.subSystemList;
            int subSystemCount = previousSubSystems.Length;
            parentSystem.subSystemList = new PlayerLoopSystem[subSystemCount - 1];

            int childCount = 0;
            for (int i = 0; i < subSystemCount; i++)
            {
                if (previousSubSystems[i].type == childSystemType)
                {
                    childCount++;
                    continue;
                }
                parentSystem.subSystemList[i] = previousSubSystems[i];
            }

            // The terrible case where there is multiple of the same system type? I question why anyone would do this
            if (childCount > 1)
            {

            }

            return childCount > 0;
        }

        /// <summary>
        ///     Adds a child system to the first found instance of a <paramref name="parentSystemType"/> system.
        /// </summary>
        /// <param name="rootSystem">The root system which the <paramref name="parentSystemType"/> will be searched recursively for.</param>
        /// <param name="parentSystemType">The system <see cref="Type"/> that will be searched for as the parent.</param>
        /// <param name="childSystem">The child system that is to be added to the parent.</param>
        /// <returns></returns>
        public static bool AddChildSystem(this ref PlayerLoopSystem rootSystem, Type parentSystemType, ref PlayerLoopSystem childSystem)
        {
            ref PlayerLoopSystem foundParent = ref rootSystem.TryGetChildSystem(parentSystemType, out bool foundTargetSystem);
            if (!foundTargetSystem) return false;

            AddChildSystem(ref foundParent, ref childSystem);
            return true;
        }

        /// <summary>
        ///     Adds a child system to the provided <paramref name="parentSystem"/>.
        /// </summary>
        /// <param name="parentSystem">The parent system which a child (sub) system should be added too.</param>
        /// <param name="childSystem">The child system that is to be added to the parent.</param>
        public static void AddChildSystem(this ref PlayerLoopSystem parentSystem, ref PlayerLoopSystem childSystem)
        {
            ref PlayerLoopSystem[] previousSubSystems = ref parentSystem.subSystemList;
            int subSystemCount = previousSubSystems.Length;
            parentSystem.subSystemList = new PlayerLoopSystem[subSystemCount + 1];

            for (int i = 0; i < subSystemCount; i++)
            {
                parentSystem.subSystemList[i] = previousSubSystems[i];
            }

            // Append
            parentSystem.subSystemList[subSystemCount + 1] = childSystem;
        }

    }
}