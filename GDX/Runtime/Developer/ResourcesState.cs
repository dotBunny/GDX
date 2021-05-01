// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using GDX.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace GDX.Developer
{
    /// <summary>
    ///     An understanding of loaded <see cref="UnityEngine.Object" />.
    /// </summary>
    /// <remarks>
    ///     Information is referenced to the target objects by a modified weak reference (<see cref="TransientReference" />),
    ///     thus this will not prevent garbage collection.
    /// </remarks>
    public class ResourcesState
    {
        /// <summary>
        ///     A collection of known (loaded in memory) <see cref="UnityEngine.Object" /> keyed by type.
        /// </summary>
        public readonly Dictionary<Type, Dictionary<TransientReference, ObjectInfo>> KnownObjects =
            new Dictionary<Type, Dictionary<TransientReference, ObjectInfo>>();

        public readonly Dictionary<Type, long> KnownUsage = new SerializableDictionary<Type, long>();

        /// <summary>
        ///     Remove all information regarding all types from <see cref="KnownObjects" />.
        /// </summary>
        public void Clear()
        {
            KnownObjects.Clear();
            KnownUsage.Clear();
        }

        /// <summary>
        ///     Remove all information regarding a specific <paramref name="type" /> from the <see cref="KnownObjects" />.
        /// </summary>
        /// <param name="type">The type to remove from the <see cref="KnownObjects" />.</param>
        public void Clear(Type type)
        {
            if (KnownObjects.ContainsKey(type))
            {
                KnownObjects.Remove(type);
            }

            if (KnownUsage.ContainsKey(type))
            {
                KnownUsage.Remove(type);
            }
        }

        /// <summary>
        ///     Identify loaded <see cref="UnityEngine.Object" /> of the description provided in <see cref="query" />.
        /// </summary>
        /// <remarks>
        ///     This method of querying uses reflection to allow for dynamic developer console calls,
        ///     <see cref="QueryForType{TType,TObjectInfo}" /> for a much faster typed operation.
        /// </remarks>
        /// <param name="query">Description of <see cref="UnityEngine.Object" /> type to search for.</param>
        public void Query(ResourcesQuery query)
        {
            // Attempt to try and make a type based on the full name (+namespace), and the assembly.
            // ex: UnityEngine.Texture2D,UnityEngine
            Type typeActual = Type.GetType(query.TypeDefinition, false);

            // If we actually got a valid type
            if (typeActual == null)
            {
                return;
            }

            // Create our ObjectInfo type, defaulting if invalid
            Type objectInfoActual = Type.GetType(query.ObjectInfoTypeDefinition, false);
            if (objectInfoActual == null)
            {
                objectInfoActual = typeof(ObjectInfo);
            }

            // Build out using reflection (yes bad, but you choose this).
            MethodInfo method = typeof(ResourcesState).GetMethod(nameof(QueryForType));
            MethodInfo generic = method.MakeGenericMethod(typeActual, objectInfoActual);

            // Invoke the method on our container
            generic.Invoke(this, null);
        }

        /// <summary>
        ///     Identify loaded <typeparamref name="TType"/>, using <typeparamref name="TObjectInfo"/> for report generation.
        /// </summary>
        /// <typeparam name="TType">The object type to query for.</typeparam>
        /// <typeparam name="TObjectInfo">The <see cref="ObjectInfo"/> used to generate report entries.</typeparam>
        public void QueryForType<TType, TObjectInfo>()
            where TType : Object
            where TObjectInfo : ObjectInfo, new()
        {
            // Find any matching resources
            TType[] foundLoadedObjects = Resources.FindObjectsOfTypeAll<TType>();
            Type typeClass = typeof(TType);

            // Make sure the category exists
            if (!KnownObjects.ContainsKey(typeClass))
            {
                KnownObjects.Add(typeClass, new Dictionary<TransientReference, ObjectInfo>());
            }

            if (!KnownUsage.ContainsKey(typeClass))
            {
                KnownUsage.Add(typeClass, 0);
            }

            // Get reference to the dictionary for the specified category
            Dictionary<TransientReference, ObjectInfo> typeObjects = KnownObjects[typeClass];

            int count = foundLoadedObjects.Length;
            for (int i = 0; i < count; i++)
            {
                Object foundObject = foundLoadedObjects[i];
                TransientReference pseudoWeakReference = new TransientReference(foundObject);

                // We can use the hashcode of a specific type at this level to determine duplication
                if (typeObjects.ContainsKey(pseudoWeakReference))
                {
                    ObjectInfo foundObjectInfo = typeObjects[pseudoWeakReference];

                    // Increment copy count
                    foundObjectInfo.CopyCount++;

                    // We actively not going to use the existing size, in case the copy is different.
                    long usage = Profiler.GetRuntimeMemorySizeLong(foundObject);
                    foundObjectInfo.TotalMemoryUsageBytes += usage;
                    KnownUsage[typeClass] += usage;
                }
                else
                {
                    TObjectInfo objectInfo = new TObjectInfo();
                    objectInfo.Populate(foundObject, pseudoWeakReference);
                    typeObjects.Add(pseudoWeakReference, objectInfo);

                    // Add to size
                    KnownUsage[typeClass] += objectInfo.MemoryUsageBytes;
                }
            }
        }
    }
}