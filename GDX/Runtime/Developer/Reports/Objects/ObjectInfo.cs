// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace GDX.Developer.Reports.Objects
{
    /// <summary>
    ///     An information storage object for a target <see cref="object" />.
    /// </summary>
    public class ObjectInfo : IComparable
    {
        /// <summary>
        ///     The fully qualified reflection definition of the <see cref="ObjectInfo" />.
        /// </summary>
        public const string TypeDefinition = "GDX.Developer.Reports.Objects.ObjectInfo,GDX";

        /// <summary>
        ///     The number of copies of the <see cref="Reference" /> object known by Unity.
        /// </summary>
        public uint CopyCount;

        /// <summary>
        ///     The memory usage of the actual <see cref="Reference" /> object (in bytes).
        /// </summary>
        public long MemoryUsage;

        /// <summary>
        ///     The name of the <see cref="Reference" /> object.
        /// </summary>
        public string Name;

        /// <summary>
        ///     A <see cref="TransientReference" /> to the target object.
        /// </summary>
        public TransientReference Reference;

        /// <summary>
        ///     The total memory usage of the <see cref="Reference" /> object (in bytes).
        /// </summary>
        public long TotalMemoryUsage;

        /// <summary>
        ///     The cached <see cref="Type" /> of the <see cref="Reference" /> object.
        /// </summary>
        public Type Type;

        /// <summary>
        /// Create a clone of this object.
        /// </summary>
        /// <returns></returns>
        public virtual ObjectInfo Clone()
        {
            return new ObjectInfo()
            {
                CopyCount = this.CopyCount,
                MemoryUsage = this.MemoryUsage,
                Name = this.Name,
                Reference = this.Reference,
                TotalMemoryUsage = this.TotalMemoryUsage,
                Type = this.Type
            };
        }

        /// <summary>
        ///     Get additional information about the specific <see cref="Reference" />.
        /// </summary>
        /// <returns>A <see cref="string" /> of additional information.</returns>
        public virtual string GetDetailedInformation(int maximumWidth)
        {
            return null;
        }

        /// <summary>
        ///     Populate an <see cref="ObjectInfo" /> from the <paramref name="targetObject" />. Optionally providing an existing
        ///     <paramref name="reference" /> created prior.
        /// </summary>
        /// <param name="targetObject">The <see cref="object" /> which to cache information about.</param>
        /// <param name="reference">An existing <see cref="TransientReference" /> targeting the <paramref name="targetObject" />.</param>
        public virtual void Populate(Object targetObject, TransientReference reference = null)
        {
            // Basic info
            Name = targetObject.name;
            Type = targetObject.GetType();

            // Assign initial memory usage stats
            MemoryUsage = Profiler.GetRuntimeMemorySizeLong(targetObject);
            TotalMemoryUsage = MemoryUsage;

            // Assign or create the transient reference to the target object
            Reference = reference != null ? reference : new TransientReference(targetObject);

            // It's new, so there's only one.
            CopyCount = 1;
        }

        /// <summary>
        /// Evaluate if the compared <see cref="ObjectInfo"/> utilizes more memory then this one.
        /// </summary>
        /// <param name="obj">The <see cref="ObjectInfo"/> to compare against.</param>
        /// <returns>1 if larger, otherwise 0.</returns>
        public int CompareTo(object obj)
        {
            if (!(obj is ObjectInfo info))
            {
                return 0;
            }

            if (TotalMemoryUsage > info.TotalMemoryUsage)
            {
                return -1;
            }
            return 1;
        }


        public void Output(ReportContext context, StringBuilder builder)
        {
            string typeName = Type.Name.PadRight(context.ObjectTypeWidth);
            if (typeName.Length > context.ObjectTypeWidth)
            {
                typeName = typeName.Substring(0, context.ObjectTypeWidth);
            }

            string objectName = Name.PadRight(context.ObjectNameWidth);
            if (objectName.Length > context.ObjectNameWidth)
            {
                objectName = objectName.Substring(0, context.ObjectNameWidth);
            }

            string sizeInfo =
                $"{Localization.GetHumanReadableFileSize(MemoryUsage)} x {CopyCount.ToString()}".PadRight(
                    context.ObjectSizeWidth);
            if (sizeInfo.Length > context.ObjectSizeWidth)
            {
                sizeInfo = sizeInfo.Substring(0, context.ObjectSizeWidth);
            }


            // Additional information
            string additionalInfo = GetDetailedInformation(context.ObjectInfoWidth);

            // Add to builder
            builder.AppendLine( additionalInfo != null
                ? $"{typeName} {objectName} {sizeInfo} {additionalInfo}"
                : $"{typeName} {objectName} {sizeInfo}");
        }


    }
}