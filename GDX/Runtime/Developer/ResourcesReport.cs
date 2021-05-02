// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using GDX.Collections.Generic;
using GDX.Developer.ObjectInfos;
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
    public class ResourcesReport : Report
    {
        /// <summary>
        ///     A collection of known (loaded in memory) <see cref="UnityEngine.Object" /> keyed by type.
        /// </summary>
        public readonly Dictionary<Type, Dictionary<TransientReference, ObjectInfo>> KnownObjects =
            new Dictionary<Type, Dictionary<TransientReference, ObjectInfo>>();

        /// <summary>
        ///     A collection of known <see cref="UnityEngine.Object" /> types in memory and their total usage.
        /// </summary>
        public readonly Dictionary<Type, long> KnownUsage = new SerializableDictionary<Type, long>();

        /// <summary>
        ///     The last time that the <see cref="ResourcesReport" /> has had a query of types.
        /// </summary>
        public DateTime LastTouched = DateTime.Now;

        /// <summary>
        ///     The total number of objects which are known to the <see cref="ResourcesReport" />.
        /// </summary>
        public int ObjectCount;

        /// <summary>
        ///     Process to destroy a <see cref="ResourcesReport" />.
        /// </summary>
        ~ResourcesReport()
        {
            KnownObjects.Clear();
        }

        public override bool Output(ref StringBuilder builder)
        {
            // Create Header
            builder.AppendLine(ReportBuilder.CreateHeader("START: Resources Report"));
            ReportBuilder.AddInstanceInformation(this, ref builder);
            builder.AppendLine($"{"Last Touched:".PadRight(ReportBuilder.TwoColumnWidth)}{LastTouched.ToString(Localization.LocalTimestampFormat)}");
            builder.AppendLine();
            ReportBuilder.AddMemoryInformation(this, ref builder);
            builder.AppendLine();

            // We iterate over each defined type in the order they were added to the known objects
            foreach (var typeKVP in KnownObjects)
            {
                int count = typeKVP.Value.Count;

                builder.AppendLine(ReportBuilder.CreateHeader(typeKVP.Key.ToString(), '-'));
                builder.AppendLine($"{"Count:".PadRight(ReportBuilder.TwoColumnWidth)}{count.ToString()}");
                builder.AppendLine($"{"Total Size:".PadRight(ReportBuilder.TwoColumnWidth)}{Localization.GetHumanReadableFileSize(KnownUsage[typeKVP.Key])}");
                builder.AppendLine(ReportBuilder.CreateDivider());

                // Sort the known objects based on size as that's the most useful context to have them listed
                List<ObjectInfo> newList = new List<ObjectInfo>(count);
                foreach (var objectKVP in typeKVP.Value)
                {
                    newList.Add(objectKVP.Value);
                }

                newList.Sort();

                // Output each item
                for (int i = 0; i < count; i++)
                {
                    ReportBuilder.AddObjectInfoLine(newList[i], ref builder);
                }
            }

            // Footer
            builder.AppendLine(ReportBuilder.CreateHeader("END: Resources Report"));

            return true;
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
            MethodInfo method = typeof(ResourcesReport).GetMethod(nameof(QueryForType));

            // Did we find the method?
            if (method is null)
            {
                return;
            }

            MethodInfo generic = method.MakeGenericMethod(typeActual, objectInfoActual);

            // Invoke the method on our container
            generic.Invoke(this, null);

            LastTouched = DateTime.Now;
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
                    foundObjectInfo.TotalMemoryUsage += usage;
                    KnownUsage[typeClass] += usage;
                }
                else
                {
                    TObjectInfo objectInfo = new TObjectInfo();
                    objectInfo.Populate(foundObject, pseudoWeakReference);
                    typeObjects.Add(pseudoWeakReference, objectInfo);

                    // Add to size
                    KnownUsage[typeClass] += objectInfo.MemoryUsage;
                    ObjectCount++;
                }
            }

            LastTouched = DateTime.Now;
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
        ///     Generate a <see cref="ResourcesReport" /> for the provided <see cref="ResourcesQuery" /> array.
        /// </summary>
        /// <remarks>
        ///     This method uses reflection to generate the necessary typed parameters, its often more efficient to
        ///     create your own custom reports like in <see cref="GetCommon" />.
        /// </remarks>
        /// <param name="queries">A list of <see cref="ResourcesQuery" /> to generate a report from.</param>
        /// <returns>A <see cref="ResourcesReport" /> containing the outlined types.</returns>
        public static ResourcesReport Get(ResourcesQuery[] queries)
        {
            // Create our collection object, this is going to effect memory based on its size
            ResourcesReport resourcesReport = new ResourcesReport();

            // Types to actual?
            int count = queries.Length;
            for (int i = 0; i < count; i++)
            {
                resourcesReport.Query(queries[i]);
            }

            return resourcesReport;
        }

        /// <summary>
        ///     Get a <see cref="ResourcesReport" /> of all <see cref="UnityEngine.Object" />.
        /// </summary>
        /// <returns>A <see cref="ResourcesReport" /> of all objects.</returns>
        public static ResourcesReport GetAll()
        {
            ResourcesReport resourcesReport = new ResourcesReport();
            resourcesReport.QueryForType<Object, ObjectInfo>();
            return resourcesReport;
        }

        /// <summary>
        ///     Get a <see cref="ResourcesReport" /> of a common subset of data which usually eats a large portion of
        ///     memory, and often can reveal memory leaks.
        /// </summary>
        /// <returns>A <see cref="ResourcesReport" /> of textures, shaders, materials and animations.</returns>
        public static ResourcesReport GetCommon()
        {
            // Create our collection object, this is going to effect memory based on its size
            ResourcesReport resourcesReport = new ResourcesReport();

            // Sections
            resourcesReport.QueryForType<RenderTexture, TextureObjectInfo>();
            resourcesReport.QueryForType<Texture3D, TextureObjectInfo>();
            resourcesReport.QueryForType<Texture2D, TextureObjectInfo>();
            resourcesReport.QueryForType<Texture2DArray, TextureObjectInfo>();
            resourcesReport.QueryForType<Cubemap, TextureObjectInfo>();
            resourcesReport.QueryForType<CubemapArray, TextureObjectInfo>();
            resourcesReport.QueryForType<Shader, ShaderObjectInfo>();
            resourcesReport.QueryForType<Material, ObjectInfo>();
            resourcesReport.QueryForType<Mesh, MeshObjectInfo>();
            resourcesReport.QueryForType<AnimationClip, ObjectInfo>();

            return resourcesReport;
        }
    }
}