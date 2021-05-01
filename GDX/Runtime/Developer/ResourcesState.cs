// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using GDX.Collections.Generic;
using GDX.Developer.ObjectInfos;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
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
        public readonly string ActiveScene;
        public readonly DateTime Created;

        /// <summary>
        ///     A collection of known (loaded in memory) <see cref="UnityEngine.Object" /> keyed by type.
        /// </summary>
        public readonly Dictionary<Type, Dictionary<TransientReference, ObjectInfo>> KnownObjects =
            new Dictionary<Type, Dictionary<TransientReference, ObjectInfo>>();

        public readonly Dictionary<Type, long> KnownUsage = new SerializableDictionary<Type, long>();
        public readonly long MonoHeapSize;
        public readonly long MonoUsedSize;

        public readonly RuntimePlatform Platform;
        public readonly long UnityGraphicsDriverAllocatedMemory;
        public readonly long UnityTotalAllocatedMemory;
        public readonly long UnityTotalReservedMemory;
        public readonly long UnityTotalUnusedReservedMemory;
        public readonly long UnityUsedHeapSize;


        public DateTime LastQueryCalled = DateTime.Now;

        public int ObjectCount;

        public ResourcesState()
        {
            ActiveScene = SceneManager.GetActiveScene().name;
            Platform = Application.platform;
            MonoUsedSize = Profiler.GetMonoUsedSizeLong();
            MonoHeapSize = Profiler.GetMonoHeapSizeLong();
            Created = DateTime.Now;

            UnityUsedHeapSize = Profiler.usedHeapSizeLong;
            UnityTotalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong();
            UnityTotalReservedMemory = Profiler.GetTotalReservedMemoryLong();
            UnityGraphicsDriverAllocatedMemory = Profiler.GetAllocatedMemoryForGraphicsDriver();
            UnityTotalUnusedReservedMemory = Profiler.GetTotalUnusedReservedMemoryLong();
        }

        ~ResourcesState()
        {
            KnownObjects.Clear();
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
            LastQueryCalled = DateTime.Now;
        }

        /// <summary>
        ///     Identify loaded <typeparamref name="TType" />, using <typeparamref name="TObjectInfo" /> for report generation.
        /// </summary>
        /// <typeparam name="TType">The object type to query for.</typeparam>
        /// <typeparam name="TObjectInfo">The <see cref="ObjectInfo" /> used to generate report entries.</typeparam>
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
                    ObjectCount++;
                }
            }

            LastQueryCalled = DateTime.Now;
        }

        /// <summary>
        ///     Remove all information regarding a specific <paramref name="type" /> from the <see cref="KnownObjects" />.
        /// </summary>
        /// <param name="type">The type to remove from the <see cref="KnownObjects" />.</param>
        public void Remove(Type type)
        {
            if (KnownObjects.ContainsKey(type))
            {
                // Decrement the object count
                ObjectCount -= KnownObjects.Count;

                // Remove the known objects type
                KnownObjects.Remove(type);
            }

            // Remove the size data
            if (KnownUsage.ContainsKey(type))
            {
                KnownUsage.Remove(type);
            }
        }

        /// <summary>
        ///     Generate a <see cref="ResourcesState" /> for the provided <see cref="ResourcesQuery" /> array.
        /// </summary>
        /// <remarks>
        ///     This method uses reflection to generate the necessary typed parameters, its often more efficient to
        ///     create your own custom reports like in <see cref="GetGeneralState" />.
        /// </remarks>
        /// <param name="queries">A list of <see cref="ResourcesQuery" /> to generate a report from.</param>
        /// <returns>A <see cref="ResourcesState" /> containing the outlined types.</returns>
        public static ResourcesState Get(ResourcesQuery[] queries)
        {
            // Create our collection object, this is going to effect memory based on its size
            ResourcesState resourcesState = new ResourcesState();

            // Types to actual?
            int count = queries.Length;
            for (int i = 0; i < count; i++)
            {
                resourcesState.Query(queries[i]);
            }

            return resourcesState;
        }

        /// <summary>
        ///     Get a <see cref="ResourcesState" /> of a common subset of data which usually eats a large portion of
        ///     memory, and often can reveal memory leaks.
        /// </summary>
        /// <returns>A <see cref="ResourcesState" /> of textures, shaders, materials and animations.</returns>
        public static ResourcesState GetGeneralState()
        {
            // Create our collection object, this is going to effect memory based on its size
            ResourcesState resourcesState = new ResourcesState();

            // Sections
            resourcesState.QueryForType<RenderTexture, TextureObjectInfo>();
            resourcesState.QueryForType<Texture3D, TextureObjectInfo>();
            resourcesState.QueryForType<Texture2D, TextureObjectInfo>();
            resourcesState.QueryForType<Texture2DArray, TextureObjectInfo>();
            resourcesState.QueryForType<Cubemap, TextureObjectInfo>();
            resourcesState.QueryForType<CubemapArray, TextureObjectInfo>();
            resourcesState.QueryForType<Material, ObjectInfo>();
            resourcesState.QueryForType<Shader, ObjectInfo>();
            resourcesState.QueryForType<AnimationClip, ObjectInfo>();

            return resourcesState;
        }
    }
}