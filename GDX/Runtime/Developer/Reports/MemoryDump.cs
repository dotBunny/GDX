// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using GDX.Collections.Generic;
using GDX.Developer.Reports;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace GDX.Developer.Reports
{
    // TODO: Add way to maybe writee out whats referencing?
    // instead of just counts? bool flag
    public static class MemoryDump
    {
        // TODO configurable tyes ... command line names?
        // has special object info

        public enum ObjectInfoType
        {
            Default,
            Mesh,
            Shader,
            Texture
        }

        public struct HeapQuery
        {
            public string FullType;

            public string AssemblyName;

            public string Category;

            public string ObjectInfoType;

            public HeapQuery(string category, string assemblyName, string fullType)
            {
                Category = category;
                AssemblyName = assemblyName;
                FullType = fullType;
                ObjectInfoType = "GDX.Developer.Reports.ObjectInfo, GDX";
            }

            public HeapQuery(string category, string assemblyName, string fullType, string objectInfo)
            {
                Category = category;
                AssemblyName = assemblyName;
                FullType = fullType;
                ObjectInfoType = objectInfo;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <example>
        ///     "UnityEngine.Texture2D, UnityEngine"
        /// </example>
        /// <remarks>
        ///     This method uses reflection to generate the necessary typed parameters.
        /// </remarks>
        /// <param name="objectTypes"></param>
        /// <returns></returns>
        public static HeapState ManagedHeapSnapshot(HeapQuery[] queries)
        {
            // Create our collection object, this is going to effect memory based on its size
            HeapState heapState = new HeapState();

            // Types to actual?
            int count = queries.Length;
            for (int i = 0; i < count; i++)
            {
                // Attempt to try and make a type based on the full name (+namespace), and the assembly.
                // ex: UnityEngine.Texture2D, UnityEngine
                Type typeActual = Type.GetType($"{queries[i].FullType}, {queries[i].AssemblyName}", false);

                // If we actually got a valid type
                if (typeActual != null)
                {
                    // Create our ObjectInfo type, defaulting if invalid
                    Type objectInfoActual = Type.GetType(queries[i].ObjectInfoType, false);
                    if (objectInfoActual == null)
                    {
                        objectInfoActual = typeof(ObjectInfo);
                    }

                    // Build out using reflection (yes bad, but you choose this).
                    MethodInfo method = typeof(HeapState).GetMethod(nameof(HeapState.QueryForType));
                    MethodInfo generic = method.MakeGenericMethod(typeActual, objectInfoActual);

                    // Invoke the method on our container
                    generic.Invoke(heapState, new object[] {queries[i].Category});
                }
                else
                {
                    // TODO: Log something about type not parsed?
                }
            }

            return heapState;
        }

        //public enum
        public static HeapState ManagedHeapSnapshot()
        {
            // Create our collection object, this is going to effect memory based on its size
            HeapState heapState = new HeapState();

            // TODO : Make types configurable from GDX config?
            // Sections
            heapState.QueryForType<RenderTexture, TextureObjectInfo>("RenderTexture");
            heapState.QueryForType<Texture3D, TextureObjectInfo>("Texture3D");

            heapState.QueryForType<Texture2D, TextureObjectInfo>("Texture2D");
            heapState.QueryForType<Texture2DArray, TextureObjectInfo>("Texture2D");

            heapState.QueryForType<Cubemap, TextureObjectInfo>("Cubemap");
            heapState.QueryForType<CubemapArray, TextureObjectInfo>("Cubemap");


            heapState.QueryForType<Material, ObjectInfo>("Material");
            heapState.QueryForType<Shader, ObjectInfo>("Shader");
            heapState.QueryForType<AnimationClip, ObjectInfo>("AnimationClip");

            return heapState;
        }
    }
}