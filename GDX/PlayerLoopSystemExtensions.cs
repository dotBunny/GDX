// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Developer;
using UnityEngine.LowLevel;

namespace GDX
{
    /// <summary>
    ///     <see cref="UnityEngine.LowLevel.PlayerLoopSystem" /> Based Extension Methods
    /// </summary>
    public static class PlayerLoopSystemExtensions
    {
        /// <summary>
        ///     Adds a child (sub) system to the provided <paramref name="parentSystem"/>.
        /// </summary>
        /// <param name="parentSystem">The parent system which a child (sub) system should be added too.</param>
        /// <param name="subSystem">The child (sub) system that is to be added to the parent.</param>
        public static void AddSubSystem(this ref PlayerLoopSystem parentSystem, ref PlayerLoopSystem subSystem)
        {
            if (parentSystem.subSystemList != null)
            {
                ref PlayerLoopSystem[] previousSubSystems = ref parentSystem.subSystemList;
                int subSystemCount = previousSubSystems.Length;
                parentSystem.subSystemList = new PlayerLoopSystem[subSystemCount + 1];
                for (int i = 0; i < subSystemCount; i++)
                {
                    parentSystem.subSystemList[i] = previousSubSystems[i];
                }
                parentSystem.subSystemList[subSystemCount + 1] = subSystem;
            }
            else
            {
                parentSystem.subSystemList = new PlayerLoopSystem[1];
                parentSystem.subSystemList[0] = subSystem;
            }
        }

        /// <summary>
        ///     Adds a child (sub) system to the first found instance of a <paramref name="parentSystemType"/> system in
        ///     <paramref name="rootSystem"/>.
        /// </summary>
        /// <param name="rootSystem">The root system which the <paramref name="parentSystemType"/> will be searched recursively for.</param>
        /// <param name="parentSystemType">The system <see cref="Type"/> that will be searched for as the parent.</param>
        /// <param name="subSystemType">The type assigned when creating the <see cref="PlayerLoopSystem"/> to be added.</param>
        /// <param name="subSystemUpdateFunction">The method to invoke when the system is updated.</param>
        /// <returns>true/false if the <paramref name="parentSystemType"/> was found, and therefore the add could occur.</returns>
        public static bool AddSubSystemToFirstSubSystemOfType(this ref PlayerLoopSystem rootSystem,
            Type parentSystemType, Type subSystemType, PlayerLoopSystem.UpdateFunction subSystemUpdateFunction)
        {
            if (subSystemUpdateFunction == null || subSystemType == null)
            {
                return false;
            }

            PlayerLoopSystem newSubSystem = new PlayerLoopSystem()
            {
                updateDelegate = subSystemUpdateFunction, type = subSystemType
            };

            ref PlayerLoopSystem foundParent = ref rootSystem.TryGetFirstSubSystemOfType(parentSystemType, out bool foundTargetSystem);
            if (!foundTargetSystem)
            {
                return false;
            }

            AddSubSystem(ref foundParent, ref newSubSystem);
            return true;
        }


        /// <summary>
        ///     Adds a child (sub) system to the first found instance of a <paramref name="parentSystemType"/> system in
        ///     <paramref name="rootSystem"/>.
        /// </summary>
        /// <param name="rootSystem">The root system which the <paramref name="parentSystemType"/> will be searched recursively for.</param>
        /// <param name="parentSystemType">The system <see cref="Type"/> that will be searched for as the parent.</param>
        /// <param name="subSystem">The child (sub) system that is to be added to the parent.</param>
        /// <returns>true/false if the <paramref name="parentSystemType"/> was found, and therefore the add could occur.</returns>
        public static bool AddSubSystemToFirstSubSystemOfType(this ref PlayerLoopSystem rootSystem, Type parentSystemType, ref PlayerLoopSystem subSystem)
        {
            ref PlayerLoopSystem foundParent = ref rootSystem.TryGetFirstSubSystemOfType(parentSystemType, out bool foundTargetSystem);
            if (!foundTargetSystem)
            {
                return false;
            }

            AddSubSystem(ref foundParent, ref subSystem);
            return true;
        }

        /// <summary>
        ///     Populates a <see cref="TextGenerator"/> with a tree-like structure that represents the
        ///     <see cref="PlayerLoopSystem"/> found under the <paramref name="rootSystem"/>.
        /// </summary>
        /// <param name="rootSystem">The root system which the tree should be crafted based off of.</param>
        /// <param name="generator">Optionally, provide a generator to be populated.</param>
        /// <returns>A <see cref="TextGenerator"/> populated with the system output.</returns>
        public static TextGenerator GenerateSystemTree(this ref PlayerLoopSystem rootSystem, TextGenerator generator = null)
        {
            // We abuse this for recursion
            generator ??= new TextGenerator();

            if (rootSystem.type != null)
            {
                generator.AppendLine(rootSystem.type.Name);
            }
            else
            {
                generator.AppendLine(generator.GetIndentLevel() == 0 ? "_RootSystem_" : "_NullSystem_");
            }

            if (rootSystem.subSystemList == null || rootSystem.subSystemList.Length <= 0)
            {
                return generator;
            }

            int count = rootSystem.subSystemList.Length;
            if (count > 0)
            {
                generator.PushIndent();
            }
            for (int i = 0; i < count; i++)
            {
                GenerateSystemTree(ref rootSystem.subSystemList[i], generator);
            }
            if (count > 0)
            {
                generator.PopIndent();
            }

            return generator;
        }

        /// <summary>
        ///     Removes all child (sub) systems of the specified <paramref name="subSystemType"/> from the provided
        ///     <paramref name="parentSystem"/>.
        /// </summary>
        /// <remarks>
        ///     This is NOT recursive, and will not effect the child (sub) systems of the child (sub) systems of the
        ///     <paramref name="parentSystem"/>
        /// </remarks>
        /// <param name="parentSystem">The parent system which the child (sub) systems should be removed from.</param>
        /// <param name="subSystemType">The system <see cref="Type"/> that will be removed.</param>
        /// <returns>true/false, if a remove was done.</returns>
        public static bool RemoveSubSystemsOfType(this ref PlayerLoopSystem parentSystem, Type subSystemType)
        {
            if (parentSystem.subSystemList == null)
            {
                return false;
            }

            ref PlayerLoopSystem[] previousSubSystems = ref parentSystem.subSystemList;
            int subSystemCount = previousSubSystems.Length;

            // We need to actually make sure there is a sub system of type to remove, we will use the search as a place
            // to also figure out if there is multiple options.
            int foundCount = 0;
            for (int i = 0; i < subSystemCount; i++)
            {
                if (previousSubSystems[i].type == subSystemType)
                {
                    foundCount++;
                }
            }

            if (foundCount == 0)
            {
                return false;
            }

            int newIndex = 0;
            int newCount = subSystemCount - foundCount;
            parentSystem.subSystemList = new PlayerLoopSystem[newCount];
            for (int i = 0; i < newCount; i++)
            {
                if (previousSubSystems[i].type != subSystemType)
                {
                    parentSystem.subSystemList[newIndex] = previousSubSystems[i];
                    newIndex++;
                }
            }

            return true;

        }

        /// <summary>
        ///     Removes all the child (sub) systems of to the first found instance of a <paramref name="parentSystemType"/> system in <paramref name="rootSystem"/>
        /// </summary>
        /// <param name="rootSystem">The root system which the <paramref name="parentSystemType"/> will be searched recursively for.</param>
        /// <param name="parentSystemType">The system <see cref="Type"/> that will be searched for as the parent.</param>
        /// <param name="subSystemType">The child (sub) system <see cref="Type"/> that will be removed.</param>
        /// <returns>true/false, if a remove occured.</returns>
        public static bool RemoveSubSystemsOfTypeFromFirstSubSystemOfType(this ref PlayerLoopSystem rootSystem,
            Type parentSystemType, Type subSystemType)
        {
            ref PlayerLoopSystem foundParent = ref rootSystem.TryGetFirstSubSystemOfType(parentSystemType, out bool foundTargetSystem);
            if (!foundTargetSystem)
            {
                return false;
            }

            return RemoveSubSystemsOfType(ref foundParent, subSystemType);
        }

        /// <summary>
        ///     Replaces the first child (sub) system of the given <paramref name="rootSystem"/> of
        ///     <paramref name="subSystemType"/> with the provided <paramref name="updatedSystem"/>.
        /// </summary>
        /// <param name="rootSystem">The root system which the <paramref name="subSystemType"/> will be searched recursively for.</param>
        /// <param name="subSystemType">The child (sub) system <see cref="Type"/> that will be replaced.</param>
        /// <param name="updatedSystem">The system to replace the found <paramref name="subSystemType"/> with.</param>
        /// <returns>true/false if the replace occured.</returns>
        public static bool ReplaceFirstSubSystemOfType(this ref PlayerLoopSystem rootSystem, Type subSystemType,
            ref PlayerLoopSystem updatedSystem)
        {
            ref PlayerLoopSystem foundParent = ref rootSystem.TryGetFirstSystemWithSubSystemOfType(subSystemType, out bool foundTargetSystem, out int foundIndex);
            if (!foundTargetSystem)
            {
                return false;
            }
            foundParent.subSystemList[foundIndex] = updatedSystem;
            return true;
        }

        /// <summary>
        ///     Replaces all child (sub) systems of the specified <paramref name="subSystemType"/> from the provided
        ///     <paramref name="parentSystem"/>.
        /// </summary>
        /// <remarks>
        ///     This is NOT recursive, and will not effect the child (sub) systems of the child (sub) systems of the
        ///     <paramref name="parentSystem"/>
        /// </remarks>
        /// <param name="parentSystem">The parent system which the child (sub) systems should be replaced.</param>
        /// <param name="subSystemType">The system <see cref="Type"/> that will be replaced.</param>
        /// <param name="updatedSystem">The system to replace the found <paramref name="subSystemType"/> with.</param>
        /// <returns>true/false if any replacement occured.</returns>
        public static bool ReplaceSubSystemsOfType(this ref PlayerLoopSystem parentSystem, Type subSystemType,
            ref PlayerLoopSystem updatedSystem)
        {
            if (parentSystem.subSystemList == null)
            {
                return false;
            }

            int count = parentSystem.subSystemList.Length;
            bool replaced = false;
            for (int i = 0; i < count; i++)
            {
                if (parentSystem.subSystemList[i].type != subSystemType)
                {
                    continue;
                }

                parentSystem.subSystemList[i] = updatedSystem;
                replaced = true;
            }
            return replaced;
        }

        /// <summary>
        ///     Searches the provided <paramref name="rootSystem"/> child (sub) systems for the first instance of a
        ///     <paramref name="subSystemType"/> system.
        /// </summary>
        /// <param name="rootSystem">
        ///     The root system which the <paramref name="subSystemType"/> will be searched recursively for.
        /// </param>
        /// <param name="subSystemType">The child (sub) system <see cref="Type"/> that will be searched for recursively.</param>
        /// <param name="foundSubSystem">Was an appropriate system found?</param>
        /// <returns>
        ///     The found system, or the root system. Check <paramref name="foundSubSystem"/> to determine if the system
        ///     was actually found. This pattern is used to preserve the reference.
        /// </returns>
        public static ref PlayerLoopSystem TryGetFirstSubSystemOfType(this ref PlayerLoopSystem rootSystem, Type subSystemType, out bool foundSubSystem)
        {
            if (rootSystem.subSystemList != null)
            {
                int subCount = rootSystem.subSystemList.Length;
                for (int i = 0; i < subCount; i++)
                {
                    // Wishful thinking
                    if (rootSystem.subSystemList[i].type == subSystemType)
                    {
                        foundSubSystem = true;
                        return ref rootSystem.subSystemList[i];
                    }

                    // Evaluate children
                    if (rootSystem.subSystemList[i].subSystemList != null &&
                        rootSystem.subSystemList[i].subSystemList.Length > 0)
                    {
                        ref PlayerLoopSystem child = ref rootSystem.subSystemList[i]
                            .TryGetFirstSubSystemOfType(subSystemType, out foundSubSystem);
                        if (foundSubSystem)
                        {
                            return ref child;
                        }
                    }
                }
            }

            foundSubSystem = false;
            return ref rootSystem;
        }

        /// <summary>
        ///     Searches the provided <paramref name="rootSystem"/> child (sub) systems for the first instance of a
        ///     <paramref name="subSystemType"/> and returns the parent system, with <paramref name="foundSystemIndex"/>
        ///     of the found child (sub) system.
        /// </summary>
        /// <param name="rootSystem">
        ///     The root system which the <paramref name="subSystemType"/> will be searched recursively for.
        /// </param>
        /// <param name="subSystemType">The child (sub) system <see cref="Type"/> that will be searched for recursively.</param>
        /// <param name="foundSubSystem">Was an appropriate child (sub) system found?</param>
        /// <param name="foundSystemIndex">The index of the found sub (child) system.</param>
        /// <returns>
        ///     The found parent system, or the root system. Check <paramref name="foundSubSystem"/> to determine if the
        ///     child (sub) system was actually found. This pattern is used to preserve the reference.
        /// </returns>
        public static ref PlayerLoopSystem TryGetFirstSystemWithSubSystemOfType(this ref PlayerLoopSystem rootSystem,
            Type subSystemType, out bool foundSubSystem, out int foundSystemIndex)
        {
            if (rootSystem.subSystemList != null)
            {
                int subCount = rootSystem.subSystemList.Length;
                for (int i = 0; i < subCount; i++)
                {
                    // Wishful thinking
                    if (rootSystem.subSystemList[i].type == subSystemType)
                    {
                        foundSubSystem = true;
                        foundSystemIndex = i;
                        return ref rootSystem;
                    }

                    // Evaluate children
                    if (rootSystem.subSystemList[i].subSystemList != null &&
                        rootSystem.subSystemList[i].subSystemList.Length > 0)
                    {
                        ref PlayerLoopSystem child = ref rootSystem.subSystemList[i]
                            .TryGetFirstSystemWithSubSystemOfType(subSystemType, out foundSubSystem,
                                out foundSystemIndex);
                        if (foundSubSystem)
                        {
                            return ref child;
                        }
                    }
                }
            }

            foundSubSystem = false;
            foundSystemIndex = -1;
            return ref rootSystem;
        }
    }
}