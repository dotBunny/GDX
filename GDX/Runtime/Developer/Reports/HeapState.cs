// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GDX.Developer.Reports
{
    public class HeapState
    {
        public readonly Dictionary<string, Dictionary<TransientReference, ObjectInfo>> KnownObjects =
            new Dictionary<string, Dictionary<TransientReference, ObjectInfo>>();

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TType">The object type.</typeparam>
        /// <typeparam name="TObjectInfo">The object information collector.</typeparam>
        /// <returns>A dictionary of objects, weakly referenced.</returns>
        public void QueryForType<TType, TObjectInfo>(string category)
            where TType : UnityEngine.Object
            where TObjectInfo : ObjectInfo, new()
        {

            // Find any matching resources
            TType[] foundLoadedObjects = Resources.FindObjectsOfTypeAll<TType>();

            // Make sure the category exists
            if (!KnownObjects.ContainsKey(category))
            {
                KnownObjects.Add(category, new Dictionary<TransientReference, ObjectInfo>());
            }

            // Get reference to the dictionary for the specified category
            Dictionary<TransientReference, ObjectInfo> categoryObjects = KnownObjects[category];

            int count = foundLoadedObjects.Length;
            for (int i = 0; i < count; i++)
            {
                UnityEngine.Object foundObject = foundLoadedObjects[i];
                TransientReference pseudoWeakReference = new TransientReference(foundObject);

                // TODO: Validate that the gethashcode is actually the same for transient / etc
                // (ie lets figure out if this count works)
                if (categoryObjects.ContainsKey(pseudoWeakReference))
                {
                    categoryObjects[pseudoWeakReference].CopyCount++;
                }
                else
                {
                    TObjectInfo objectInfo = new TObjectInfo();
                    objectInfo.Populate(foundObject);
                    categoryObjects.Add(pseudoWeakReference,objectInfo);
                }
            }
        }

        public void Clear()
        {
            KnownObjects.Clear();
        }
    }
}