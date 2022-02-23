// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using GDX.Developer.Reports.Resource;
using GDX.Developer.Reports.Resource.Objects;
using GDX.Developer.Reports.Resource.Sections;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace GDX.Developer.Reports
{
    /// <summary>
    ///     An audit of loaded <see cref="UnityEngine.Object" /> for queried types.
    /// </summary>
    /// <remarks>
    ///     Information is referenced to the target objects by a modified weak reference (<see cref="TransientReference" />),
    ///     thus this will not prevent garbage collection.
    /// </remarks>
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
    public class ResourcesAuditReport : ResourceReport
    {
        public readonly ApplicationSection ApplicationContext = ApplicationSection.Get();

        /// <summary>
        ///     A collection of known (loaded in memory) <see cref="UnityEngine.Object" /> keyed by type.
        /// </summary>
        public readonly Dictionary<Type, Dictionary<TransientReference, ObjectInfo>> KnownObjects =
            new Dictionary<Type, Dictionary<TransientReference, ObjectInfo>>();

        /// <summary>
        ///     A collection of known <see cref="UnityEngine.Object" /> types in memory and their total usage.
        /// </summary>
        public readonly Dictionary<Type, long> KnownUsage = new Dictionary<Type, long>();

        public readonly MemorySection MemoryContext = MemorySection.Get();

        /// <summary>
        ///     The last time that the <see cref="ResourcesAuditReport" /> has had a query of types.
        /// </summary>
        public DateTime LastTouched = DateTime.Now;

        /// <summary>
        ///     The total number of objects which are known to the <see cref="ResourcesAuditReport" />.
        /// </summary>
        public int ObjectCount;

        /// <summary>
        ///     Process to destroy a <see cref="ResourcesAuditReport" />.
        /// </summary>
        ~ResourcesAuditReport()
        {
            KnownObjects.Clear();
        }

        /// <inheritdoc />
        public override bool Output(StringBuilder builder, ResourceReportContext context = null)
        {
            // We need to make the context if its not provided
            context ??= new ResourceReportContext();

            // Create header
            builder.AppendLine(CreateHeader(context,"START: Resources Audit Report"));

            // Add standard report information
            ApplicationContext.Output(context, builder);

            // Custom header information
            builder.AppendLine(CreateKeyValuePair(context,"Last Touched",
                LastTouched.ToString(Localization.LocalTimestampFormat)));
            builder.AppendLine(CreateKeyValuePair(context,"Total Objects", ObjectCount.ToString()));

            builder.AppendLine();

            // Add memory information
            MemoryContext.Output(context, builder);
            builder.AppendLine();

            // We iterate over each defined type in the order they were added to the known objects
            foreach (KeyValuePair<Type, Dictionary<TransientReference, ObjectInfo>> knownObject in KnownObjects)
            {
                int count = knownObject.Value.Count;

                builder.AppendLine(CreateHeader(context, knownObject.Key.ToString(), '-'));
                builder.AppendLine(CreateKeyValuePair(context,"Count", count.ToString()));
                builder.AppendLine(CreateKeyValuePair(context,"Total Size",
                    Localization.GetHumanReadableFileSize(KnownUsage[knownObject.Key])));
                builder.AppendLine();

                // Sort the known objects based on size as that's the most useful context to have them listed
                List<ObjectInfo> newList = new List<ObjectInfo>(count);
                foreach (KeyValuePair<TransientReference, ObjectInfo> objectInfo in knownObject.Value)
                {
                    newList.Add(objectInfo.Value);
                }

                newList.Sort();

                // Output each item
                for (int i = 0; i < count; i++)
                {
                    newList[i].Output(context, builder);
                }

                builder.AppendLine();
            }

            // Footer
            builder.AppendLine(CreateHeader(context,"END: Resources Audit Report"));

            return true;
        }

        /// <summary>
        ///     Identify loaded <see cref="UnityEngine.Object" /> of the description provided in <paramref cref="query" />.
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
            Type objectInfoActual = null;
            if (query.ObjectInfoTypeDefinition != null)
            {
                objectInfoActual = Type.GetType(query.ObjectInfoTypeDefinition, false);
            }

            objectInfoActual ??= ObjectInfoFactory.GetObjectInfoType(typeActual);


            // Build out using reflection (yes bad, but you choose this).
            MethodInfo method = typeof(ResourcesAuditReport).GetMethod(nameof(QueryForType));

            // Did we find the method?
            if (method is null)
            {
                return;
            }

            MethodInfo generic = method.MakeGenericMethod(typeActual, objectInfoActual);

            // Invoke the method on our container
            generic.Invoke(this, new object[] {query.NameFilter});

            LastTouched = DateTime.Now;
        }

        /// <summary>
        ///     Identify loaded <typeparamref name="TType" />, using <typeparamref name="TObjectInfo" /> for report generation.
        /// </summary>
        /// <typeparam name="TType">The object type to query for.</typeparam>
        /// <typeparam name="TObjectInfo">The <see cref="ObjectInfo" /> used to generate report entries.</typeparam>
        public void QueryForType<TType, TObjectInfo>(string nameFilter = null)
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

            bool evaluateNames = !string.IsNullOrEmpty(nameFilter);
            int count = foundLoadedObjects.Length;
            for (int i = 0; i < count; i++)
            {
                Object foundObject = foundLoadedObjects[i];

                // If we have a provided filter, and our objects name doesnt contain the filter, skip
                if (evaluateNames && !foundObject.name.Contains(nameFilter))
                {
                    continue;
                }

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
        ///     Generate a <see cref="ResourcesAuditReport" /> for the provided <see cref="ResourcesQuery" /> array.
        /// </summary>
        /// <remarks>
        ///     This method uses reflection to generate the necessary typed parameters, its often more efficient to
        ///     create your own custom reports like in <see cref="GetCommon" />.
        /// </remarks>
        /// <param name="queries">A list of <see cref="ResourcesQuery" /> to generate a report from.</param>
        /// <returns>A <see cref="ResourcesAuditReport" /> containing the outlined types.</returns>
        public static ResourcesAuditReport Get(ResourcesQuery[] queries)
        {
            // Create our collection object, this is going to effect memory based on its size
            ResourcesAuditReport resourcesAuditReport = new ResourcesAuditReport();

            // Types to actual?
            int count = queries.Length;
            for (int i = 0; i < count; i++)
            {
                resourcesAuditReport.Query(queries[i]);
            }

            return resourcesAuditReport;
        }

        /// <summary>
        ///     Get a <see cref="ResourcesAuditReport" /> of all <see cref="UnityEngine.Object" />.
        /// </summary>
        /// <returns>A <see cref="ResourcesAuditReport" /> of all objects.</returns>
        public static ResourcesAuditReport GetAll()
        {
            ResourcesAuditReport resourcesAuditReport = new ResourcesAuditReport();
            resourcesAuditReport.QueryForType<Object, ObjectInfo>();
            return resourcesAuditReport;
        }

        /// <summary>
        ///     Get a <see cref="ResourcesAuditReport" /> of a common subset of data which usually eats a large portion of
        ///     memory, and often can reveal memory leaks.
        /// </summary>
        /// <returns>A <see cref="ResourcesAuditReport" /> of textures, shaders, materials and animations.</returns>
        public static ResourcesAuditReport GetCommon()
        {
            // Create our collection object, this is going to effect memory based on its size
            ResourcesAuditReport resourcesAuditReport = new ResourcesAuditReport();

            // Sections
            resourcesAuditReport.QueryForType<RenderTexture, TextureObjectInfo>();
            resourcesAuditReport.QueryForType<Texture3D, TextureObjectInfo>();
            resourcesAuditReport.QueryForType<Texture2D, TextureObjectInfo>();
            resourcesAuditReport.QueryForType<Texture2DArray, TextureObjectInfo>();
            resourcesAuditReport.QueryForType<Cubemap, TextureObjectInfo>();
            resourcesAuditReport.QueryForType<CubemapArray, TextureObjectInfo>();
            resourcesAuditReport.QueryForType<Shader, ShaderObjectInfo>();
            resourcesAuditReport.QueryForType<Material, ObjectInfo>();
            resourcesAuditReport.QueryForType<Mesh, MeshObjectInfo>();
            resourcesAuditReport.QueryForType<AnimationClip, ObjectInfo>();
            resourcesAuditReport.QueryForType<AssetBundle, AssetBundleObjectInfo>();

            return resourcesAuditReport;
        }

        /// <summary>
        ///     A structure that defines the string inputs necessary to query for loaded resources of a specific type.
        /// </summary>
        /// <remarks>
        ///     This forces the path through reflection when querying; there are faster methods available. These queries are
        ///     built ideally to support dynamic developer console calls.
        /// </remarks>
        public readonly struct ResourcesQuery
        {
            /// <summary>
            ///     The fully qualified type that is going to be evaluated.
            /// </summary>
            /// <example>
            ///     UnityEngine.Texture2D,UnityEngine
            /// </example>
            public readonly string TypeDefinition;

            /// <summary>
            ///     The fully qualified type that is going to be used to generate a report on the type.
            /// </summary>
            /// <example>
            ///     GDX.Developer.Reports.ObjectInfo,GDX
            /// </example>
            public readonly string ObjectInfoTypeDefinition;


            /// <summary>
            ///     A <see cref="string" /> check against a <see cref="UnityEngine.Object" /> name.
            /// </summary>
            /// <example>
            ///     Armor
            /// </example>
            public readonly string NameFilter;

            /// <summary>
            ///     Create a <see cref="ResourcesQuery" /> for the given <paramref name="typeDefinition" />, while
            ///     attempting to use the provided <paramref name="objectInfoTypeDefinition" /> for report generation.
            /// </summary>
            /// <remarks>
            ///     Uses the default <see cref="ObjectInfo" /> for report generation if
            ///     <paramref name="objectInfoTypeDefinition" /> fails to qualify.
            /// </remarks>
            /// <param name="typeDefinition">The fully qualified type that is going to be evaluated.</param>
            /// <param name="objectInfoTypeDefinition">
            ///     The fully qualified type that is going to be used to generate a report on the
            ///     type. If left null, system will attempt to find an appropriate info generator.
            /// </param>
            /// <param name="nameFilter">
            ///     A string that must be contained in an objects name for it to be valid in the query.
            /// </param>
            /// <example>
            ///     var queryTexture2D = new ResourcesQuery("UnityEngine.Texture2D,UnityEngine",
            ///     "GDX.Developer.Reports.ObjectInfos.TextureObjectInfo,GDX", "Armor");
            /// </example>
            public ResourcesQuery(string typeDefinition, string objectInfoTypeDefinition = null,
                string nameFilter = null)
            {
                TypeDefinition = typeDefinition;
                ObjectInfoTypeDefinition = objectInfoTypeDefinition;
                NameFilter = nameFilter;
            }
        }
    }
}
#endif // !UNITY_DOTSRUNTIME