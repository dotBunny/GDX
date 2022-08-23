// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using UnityEngine.LowLevel;

namespace GDX
{
    /// <summary>
    ///     <see cref="UnityEngine.LowLevel.PlayerLoopSystem" /> Based Extension Methods
    /// </summary>
    public static class PlayerLoopSystemExtensions
    {
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

        /// <summary>
        ///     Adds a child system to the first found instance of a <paramref name="parentSystemType"/> system in
        ///     <paramref name="rootSystem"/>.
        /// </summary>
        /// <param name="rootSystem">The root system which the <paramref name="parentSystemType"/> will be searched recursively for.</param>
        /// <param name="parentSystemType">The system <see cref="Type"/> that will be searched for as the parent.</param>
        /// <param name="childSystem">The child system that is to be added to the parent.</param>
        /// <returns>true/false if the <paramref name="parentSystemType"/> was found, and therefore the add could occur.</returns>
        public static bool AddChildSystemToFirstOfType(this ref PlayerLoopSystem rootSystem, Type parentSystemType, ref PlayerLoopSystem childSystem)
        {
            ref PlayerLoopSystem foundParent = ref rootSystem.TryGetFirstChildSystemOfType(parentSystemType, out bool foundTargetSystem);
            if (!foundTargetSystem) return false;

            AddChildSystem(ref foundParent, ref childSystem);
            return true;
        }

        /// <summary>
        ///     Searches the provided <paramref name="rootSystem"/> for the first instance of a
        ///     <paramref name="systemType"/> system.
        /// </summary>
        /// <param name="rootSystem">
        ///     The root system which the <paramref name="systemType"/> will be searched
        ///     recursively for.
        /// </param>
        /// <param name="systemType">The system <see cref="Type"/> that will be searched for recursively.</param>
        /// <param name="foundSystem">Was an appropriate system found?</param>
        /// <returns>
        ///     The found system, or the root system. Check <paramref name="foundSystem"/> to determine if the system
        ///     was actually found. This pattern is used to preserve the reference.
        /// </returns>
        public static ref PlayerLoopSystem TryGetFirstChildSystemOfType(this ref PlayerLoopSystem rootSystem, Type systemType, out bool foundSystem)
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
                    ref PlayerLoopSystem child = ref rootSystem.subSystemList[i].TryGetFirstChildSystemOfType(systemType, out foundSystem);
                    return ref child;
                }
            }

            foundSystem = false;
            return ref rootSystem;
        }


        public static bool RemoveChildSystem(this ref PlayerLoopSystem parentSystem, ref PlayerLoopSystem childSystem)
        {
            return false;
        }



        public static bool RemoveAllChildSystemOfTypeFromType(this ref PlayerLoopSystem rootSystem, Type parentSystemType, Type childSystemType)
        {
            ref PlayerLoopSystem foundParent =
                ref rootSystem.TryGetFirstChildSystemOfType(parentSystemType, out bool foundTargetSystem);
            if (!foundTargetSystem) return false;
            return RemoveAllChildSystemOfType(ref foundParent, childSystemType);
        }

        public static bool RemoveAllChildSystemOfType(this ref PlayerLoopSystem parentSystem, Type childSystemType)
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




        public static bool ReplaceChildSystem(this ref PlayerLoopSystem rootSystem, Type systemType,
            ref PlayerLoopSystem updatedSystem)
        {
            return false;
        }

    }
}